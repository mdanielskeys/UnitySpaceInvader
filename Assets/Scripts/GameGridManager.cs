using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameGridManager : MonoBehaviour
{
    private const float SMOOTH_ADVANCE = -0.06f;
    private const float REGULAR_ADVANCE = -0.2f;
    private const float PAUSE = 4.0f;
    private const float SMOOTH_TIME = .01f;
    private const float LEVEL_DISPLAY_TIME = 2.0f;
    private const float HIGH_SPEED_MARCH = 0.04f;
    private const float MEDIUM_SPEED_MARCH = 0.02f;
    private const float REG_SPEED_MARCH = 0.01f;
    private const float HIGH_FIRE = 0.5f;
    private const float MEDIUM_FIRE = 0.7f;
    private const float REG_FIRE = 0.8f;
    private const int HIGH_FIRE_RATE = 7;
    private const int MED_FIRE_RATE = 5;
    private const int LOW_FIRE_RATE = 3;
    public float marchSpeed;
    public int maxFireCount;
    public int NumberOfLives;

    public GameObject DoomdayShip;
    public GameObject Enemy1Ship;
    public GameObject Enemy2Ship;
    public GameObject Enemy3Ship;
    public Canvas gameOver;
    public Text gameOverText;
    public Text instructionText;
    public Text playerScoreText;
    public Text levelDisplay;
    public Text playerCount;

    private float fireThreshold;
    private int gameLevel;
    private int playerScore;
    private float elaspedTimeThresh;
    private float elapsedTime;
    private int fireCount;
    private float fireInterval;

    private GameObject topShip;
    private GameObject playerShipInstance;
    private EnemyController enemyController;

    public enum GameState
    {
        GameOver = 1,
        GameRunning = 2,
        FlyInView = 3,
        FlyOutOfView,
        DisplayLevel
    };

    private GameState _state;
    private GameGridManager _manager;
    public GameObject Playership;

    // Use this for initialization
    void Start()
    {
        levelDisplay.enabled = false;

        _manager = GetComponent<GameGridManager>();

        enemyController = new EnemyController
        {
            GameManager = gameObject,
            DoomdayShip = DoomdayShip,
            Enemy1Ship = Enemy1Ship,
            Enemy2Ship = Enemy2Ship,
            Enemy3Ship = Enemy3Ship,
            MarchSpeed = marchSpeed
        };

        SetGameOver();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && _state == GameState.GameOver)
        {
            StartLevel1();
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();    
        }

        elapsedTime += Time.deltaTime;

        if (_state == GameState.GameRunning)
        {
            if (enemyController.ShouldAdvance())
            {                
                enemyController.AdvanceEnemies();
            }

            var ecount = EnemyCount();
            if (ecount <= 0)
            {
                if (elapsedTime > 5.0f)
                {
                    AddGameLevel();
                }
            }
            else if (ecount < 10)
            {
                enemyController.MarchSpeed = HIGH_SPEED_MARCH;
                maxFireCount = HIGH_FIRE_RATE;
                fireThreshold = HIGH_FIRE;
            }
            else if (ecount < 20)
            {
                enemyController.MarchSpeed = MEDIUM_SPEED_MARCH;
                maxFireCount = MED_FIRE_RATE;
                fireThreshold = MEDIUM_FIRE;
            }
            else
            {
                enemyController.MarchSpeed = REG_SPEED_MARCH;
                maxFireCount = LOW_FIRE_RATE;
                fireThreshold = REG_FIRE;
            }

            fireInterval += Time.deltaTime;
            if (fireInterval > 1 && fireCount < maxFireCount)
            {
                CheckWeapons();
            }
            enemyController.MarchEnemies();
        }

        if (_state == GameState.FlyInView)
        {
            if (enemyController.AreEnemiesInStartPos() && elapsedTime > 1f)
            {
                enemyController.EnemyAdvanceSpeed = REGULAR_ADVANCE;
                _state = GameState.GameRunning;
            }
            else
            {
                if (!enemyController.AreEnemiesInStartPos() && elapsedTime > elaspedTimeThresh)
                {
                    elapsedTime = 0f;
                    AdvanceEnemies();
                }
            }
        }

        if (_state == GameState.FlyOutOfView)
        {
            //Debug.Log(string.Format("topship.position {0}", topShip.transform.position.y));
            if (enemyController.AreEnemiesInView() && elapsedTime > elaspedTimeThresh)
            {
                //Debug.Log("Call Advance");
                elaspedTimeThresh = SMOOTH_TIME; 
                elapsedTime = 0f;
                AdvanceEnemies();
            }
            else
            {
                if (enemyController.AreEnemiesInOffscreenPos())
                {
                    if (NumberOfLives > 0)
                    {
                        ResetShipsToTop();
                        InstantiateNewPlayer();
                        FlyShipsInView();
                    }
                    else
                    {
                        SetGameOver();
                    }
                }
            }
        }

        if (_state == GameState.DisplayLevel)
        {
            if (elapsedTime > elaspedTimeThresh)
                FlyShipsInView();
        }
    }

    private void AddGameLevel()
    {
        gameLevel += 1;
        SetGrid();
    }

    private void SetDisplayLevel()
    {
        _state = GameState.DisplayLevel;
        elaspedTimeThresh = LEVEL_DISPLAY_TIME;
        elapsedTime = 0f;
        levelDisplay.enabled = true;
        WriteGameLevel();
    }

    private void SetGameOverText(bool isOn)
    {
        gameOverText.enabled = isOn;
        instructionText.enabled = isOn;
    }

    public void SetGameOver()
    {
        SetGameOverText(true);
        _state = GameState.GameOver;
    }

    public GameState GetGameState()
    {
        return _state;
    }

    public void ResetPlayerScore()
    {
        playerScore = 0;
        gameLevel = 1;
        WritePlayerScore();
        WriteGameLevel();
    }

    public void UpdatePlayerScore(int pointValue)
    {
        playerScore += pointValue;
        WritePlayerScore();
    }

    private void WritePlayerCount()
    {
        playerCount.text = string.Format("Player Ships: {0:d2}", NumberOfLives);
    }
    private void WritePlayerScore()
    {
        playerScoreText.text = string.Format("Score: {0:d8}", playerScore);
    }

    private void WriteGameLevel()
    {
        _state = GameState.DisplayLevel;
        elapsedTime = 0;
        elaspedTimeThresh = LEVEL_DISPLAY_TIME;
        levelDisplay.enabled = true;

        levelDisplay.text = string.Format("Level: {0:d3}", gameLevel);
    }

    private void StartLevel1()
    {
        NumberOfLives = 3;
        WritePlayerCount();
        SetGameOverText(false);
        InstantiateNewPlayer();

        _manager.SetGrid();
        ResetPlayerScore();
    }

    private void InstantiateNewPlayer()
    {
        playerShipInstance = Instantiate(Playership, new Vector3(0, -4, 0), Quaternion.identity);
        playerShipInstance.transform.parent = gameObject.transform;
    }

    public void ReleaseBulletCount()
    {
        fireCount = Math.Max(fireCount - 1, 0);
    }


    public void SetGrid()
    {
        elapsedTime = 0f;
        fireCount = 0;
        fireInterval = 0f;

        enemyController.InitializeGameGrid();

        SetDisplayLevel();
    }

    private void FlyShipsInView()
    {
        elapsedTime = 0f;
        elaspedTimeThresh = SMOOTH_TIME;
        _state = GameState.FlyInView;
        enemyController.EnemyAdvanceSpeed = SMOOTH_ADVANCE;

        levelDisplay.enabled = false;
        SetGameOverText(false);
    }

    public void FlyOutOfView()
    {
        NumberOfLives -= 1;
        WritePlayerCount();
        FindTopShip();
        elapsedTime = 0f;
        elaspedTimeThresh = PAUSE;
        _state = GameState.FlyOutOfView;
        enemyController.EnemyAdvanceSpeed = SMOOTH_ADVANCE;
    }

    public void EarthDestroyed()
    {
        var shipManager = playerShipInstance.GetComponent<ShipCollision>();
        if (shipManager != null)
        {
            shipManager.PlayerHit(this);
        }
    }

    // ship controller functions
    public void AdvanceEnemies()
    {
        enemyController.AdvanceEnemies();
    }

    public void KillEnemy(GameObject enemyShip)
    {
        enemyController.KillEnemy(enemyShip);
    }

    private void FindTopShip()
    {
        topShip = enemyController.FindTopShip();
    }

    private void ResetShipsToTop()
    {
        enemyController.ResetShipsToTop();
    }
    private int EnemyCount()
    {
        return enemyController.EnemyCount();
    }
    private void CheckWeapons()
    {
        fireInterval = 0f;
        if (enemyController.FireRandomWeapon())
        {
            fireCount += 1;
        }
    }

}
