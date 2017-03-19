using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameGridManager : MonoBehaviour
{
    private const float SMOOTH_ADVANCE = -0.06f;
    private const float REGULAR_ADVANCE = -0.2f;
    private const float PAUSE = 4.0f;
    private const float SMOOTH_TIME = .01f;
    public float marchSpeed;
    public int maxFireCount;

    public GameObject DoomdayShip;
    public GameObject Enemy1Ship;
    public GameObject Enemy2Ship;
    public GameObject Enemy3Ship;
    public Canvas gameOver;
    public Text gameOverText;
    public Text instructionText;
    public Text playerScoreText;

    private int playerScore;
    private float elaspedTimeThresh;
    private float elapsedTime;
    private int fireCount;
    private float fireInterval;

    private int[] gameGrid =
    {
        0, 0, 1, 1, 0, 1, 1, 0, 0,
        0, 1, 1, 1, 1, 1, 1, 1, 0,
        1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1
    };

    private GameObject topShip;
    private float _enemyAdvanceSpeed;

    public enum GameState
    {
        GameOver = 1,
        GameRunning = 2,
        FlyInView = 3,
        FlyOutOfView
    };

    private GameState _state;
    private GameGridManager _manager;
    public GameObject Playership;

    // Use this for initialization
    void Start()
    {
        _manager = GetComponent<GameGridManager>();

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
            if (EnemyCount() <= 0)
            {
                if (elapsedTime > 5.0f)
                {
                    SetGrid();
                }
            }

            fireInterval += Time.deltaTime;
            if (fireInterval > 1 && fireCount < maxFireCount)
            {
                CheckWeapons();
            }
        }

        if (_state == GameState.FlyInView)
        {
            if (topShip.transform.position.y <= 4.0f && elapsedTime > 1f)
            {
                _enemyAdvanceSpeed = REGULAR_ADVANCE;
                _state = GameState.GameRunning;
            }
            else
            {
                if (topShip.transform.position.y > 4.0f && elapsedTime > elaspedTimeThresh)
                {
                    // Debug.Log("Call Advance");
                    elapsedTime = 0f;
                    AdvanceEnemies();
                }
            }
        }

        if (_state == GameState.FlyOutOfView)
        {
            //Debug.Log(string.Format("topship.position {0}", topShip.transform.position.y));
            if (topShip.transform.position.y > -6.0f && elapsedTime > elaspedTimeThresh)
            {
                //Debug.Log("Call Advance");
                elaspedTimeThresh = SMOOTH_TIME; 
                elapsedTime = 0f;
                AdvanceEnemies();
            }
            else
            {
                if (topShip.transform.position.y <= -6.0f)
                {
                    SetGameOver();
                }
            }
        }

    }

    public void SetGameState(GameState state)
    {
        _state = state;
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
        WritePlayerScore();
    }

    public void UpdatePlayerScore(int pointValue)
    {
        playerScore += pointValue;
        WritePlayerScore();
    }

    private void WritePlayerScore()
    {
        playerScoreText.text = string.Format("Score: {0:d8}", playerScore);
    }

    private void StartLevel1()
    {
        SetGameOverText(false);
        var player = Instantiate(Playership, new Vector3(0, -4, 0), Quaternion.identity);
        player.transform.parent = gameObject.transform;

        _manager.SetGrid();
        ResetPlayerScore();
    }

    public void ReleaseBulletCount()
    {
        fireCount = Math.Max(fireCount - 1, 0);
    }

    private void CheckWeapons()
    {
        fireInterval = 0f;
        for (var idx = transform.childCount -1; idx > 0; --idx)
        {
            var enemy = transform.GetChild(idx);

            if (fireCount < maxFireCount && Random.value > 0.8f)
            {
                var enemyScript = enemy.GetComponent<EnemyActions>();
                if (enemyScript != null)
                {
                    var tranform = enemy.transform;
                    var bullet = Instantiate(enemyScript.EnemyBullet, tranform.position, Quaternion.identity);
                    bullet.transform.parent = gameObject.transform;
                    fireCount += 1;
                    break;
                }
            }
        }
    }

    private void ClearExistingEnemies()
    {
        topShip = null;
        for (var idx = 0; idx < transform.childCount; ++idx)
        {
            var enemy = transform.GetChild(idx);
            if (enemy.tag == "Enemy")
            {
                Destroy(enemy.gameObject);
            }
        }
    }


    public void SetGrid()
    {
        elapsedTime = 0f;
        fireCount = 0;
        fireInterval = 0f;
        ClearExistingEnemies();

        var starty = 12;
        var rowCount = 0;
        var column = -4.5f;

        foreach (var elem in gameGrid)
        {
            GameObject shipObject;
            switch (rowCount)
            {
                case 0:
                    shipObject = DoomdayShip;
                    break;
                case 1:
                    shipObject = Enemy3Ship;
                    break;
                case 2:
                    shipObject = Enemy1Ship;
                    break;
                case 3:
                case 4:
                    shipObject = Enemy2Ship;
                    break;
                default:
                    shipObject = DoomdayShip;
                    break;
            }
            if (elem == 1)
            {
                var newShip = Instantiate(shipObject, new Vector3(column, starty - (rowCount * .7f), 0), Quaternion.identity);
                newShip.transform.parent = gameObject.transform;
                if (topShip == null)
                {
                    topShip = newShip;
                }
            }
            column += 1f;
            if (column >= 4.5f)
            {
                // Debug.Log(string.Format("Column {0}", column));
                column = -4.5f;
                rowCount += 1;
            }
        }
        FlyShipsInView();
    }

    private int EnemyCount()
    {
        var count = 0;
        for (var idx = 0; idx < transform.childCount; ++idx)
        {
            var enemy = transform.GetChild(idx);
            if (enemy.tag == "Enemy")
            {
                count += 1;
            }
        }

        return count;
    }

    private void FlyShipsInView()
    {
        elapsedTime = 0f;
        elaspedTimeThresh = SMOOTH_TIME;
        _state = GameState.FlyInView;
        _enemyAdvanceSpeed = SMOOTH_ADVANCE;
    }

    private void FindTopShip()
    {
        for (var idx = 0; idx < transform.childCount; ++idx)
        {
            var enemy = transform.GetChild(idx);
            if (enemy.tag == "Enemy")
            {
                if (topShip == null)
                {
                    topShip = enemy.gameObject;
                }
                else if (enemy.transform.position.y > topShip.transform.position.y)
                {
                    topShip = enemy.gameObject;
                }
            }
        }

    }
    public void FlyOutOfView()
    {
        FindTopShip();
        elapsedTime = 0f;
        elaspedTimeThresh = PAUSE;
        _state = GameState.FlyOutOfView;
        _enemyAdvanceSpeed = SMOOTH_ADVANCE;
    }

    public void AdvanceEnemies()
    {
        for (var idx = 0; idx < transform.childCount; ++idx)
        {
            var enemy = transform.GetChild(idx);
            if (enemy.tag == "Enemy")
            {
                var enemyScript = enemy.GetComponent<EnemyActions>();
                if (enemyScript != null)
                {
                    enemyScript.Advance(_enemyAdvanceSpeed);
                }
            }
        }
    }

}
