﻿using System;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameGridManager : MonoBehaviour
{
    private const int FIRST_BONUS_AMOUNT = 20000;
    private const int SECOND_BONUS_AMOUNT = 40000;
    private const float SMOOTH_ADVANCE = -0.06f;
    private const float REGULAR_ADVANCE = -0.2f;
    private const float SMOOTH_TEXT_ADVANCE = 10.0f;
    private const float PAUSE = 4.0f;
    private const float SMOOTH_TIME = .01f;
    private const float LEVEL_DISPLAY_TIME = 2.0f;
    public float marchSpeed;
    public int maxFireCount;

    public GameObject DoomdayShip;
    public GameObject Enemy1Ship;
    public GameObject Enemy2Ship;
    public GameObject Enemy3Ship;
    public Canvas PlayScreen;
    public AudioEvent BonusShipAudio;
    public AudioEvent GameMusic;
    public GameObject GameMusicSource;

    private Canvas PlayScreenInstance;
    private Text ScoreText;
    private Text LevelDisplayText;
    private Text PlayerCount;

    private float fireThreshold;
    private int gameLevel;
    private int playerScore;
    private float elaspedTimeThresh;
    private float elapsedTime;
    private int fireCount;
    private float fireInterval;
    private bool displayCredits;

    private EnemyController enemyController;

    public enum GameState
    {
        GameOver = 1,
        GameRunning = 2,
        FlyInView = 3,
        FlyOutOfView,
        DisplayLevel,
        AnimateCredits
    };

    private GameState _state;
    private GameGridManager _manager;

    // Use this for initialization
    void Start()
    {
        GameMusicSource = Instantiate(GameMusicSource);

        PlayScreenInstance = Instantiate(PlayScreen);
        PlayScreenInstance.gameObject.SetActive(false);
        var playText = PlayScreenInstance.GetComponentsInChildren<Text>();
        foreach (var text in playText)
        {
            if (text.name == "ScoreText")
            {
                ScoreText = text;
            }
            else if (text.name == "LevelDisplayText")
            {
                LevelDisplayText = text;
            }
            else if (text.name == "PlayerCount")
            {
                PlayerCount = text;
            }
        }

        _manager = GetComponent<GameGridManager>();

        enemyController = new EnemyController
        {
            GameManager = gameObject,
            DoomdayShip = DoomdayShip,
            Enemy1Ship = Enemy1Ship,
            Enemy2Ship = Enemy2Ship,
            Enemy3Ship = Enemy3Ship,
            MarchSpeed = marchSpeed,
            MaxNumberOfShots = maxFireCount
        };

        displayCredits = false;
        StartLevel1();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();    
        }


        if (_state == GameState.GameRunning)
        {
            if (!PlayerManager.Instance.IsAlive)
            {
                FlyOutOfView();
                return;
            }

            if (EnemyCount() <= 0)
            {
                if (elapsedTime > 5.0f)
                {
                    AddGameLevel();
                }
            }

            CheckWeapons();

            enemyController.UpdateShips();
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
                    if (PlayerManager.Instance.GetLivesLeft() > 0)
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

    private void ToggleCredits()
    {
        displayCredits = !displayCredits;
    }

    private void AddGameLevel()
    {
        gameLevel += 1;
        if (gameLevel == 3)
        {
            enemyController.MaxNumberOfShots += 1;
        }
        else if (gameLevel == 5)
        {
            enemyController.MaxNumberOfShots += 1;
        }

        SetGrid();
    }

    private void SetDisplayLevel()
    {
        _state = GameState.DisplayLevel;
        elaspedTimeThresh = LEVEL_DISPLAY_TIME;
        elapsedTime = 0f;
        LevelDisplayText.enabled = true;
        WriteGameLevel();
    }

    private void SetGameOverText(bool isOn)
    {
        PlayScreenInstance.gameObject.SetActive(!isOn);
    }

    public void SetGameOver()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
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

		/*
        if (playerScore > PlayerShipBonus)
        {
            PlayerShipBonus += SECOND_BONUS_AMOUNT;
            NumberOfLives += 1;
            BonusShipAudio.Play(GetComponent<AudioSource>());
            WritePlayerCount();
        }
        */
    }

    private void WritePlayerCount()
    {
        PlayerCount.text = string.Format("Player Ships: {0:d2}", PlayerManager.Instance.GetLivesLeft());
    }
    private void WritePlayerScore()
    {
        ScoreText.text = string.Format("Score: {0:d8}", playerScore);
    }

    private void WriteGameLevel()
    {
        _state = GameState.DisplayLevel;
        elapsedTime = 0;
        elaspedTimeThresh = LEVEL_DISPLAY_TIME;
        LevelDisplayText.enabled = true;

        LevelDisplayText.text = string.Format("Level: {0:d3}", gameLevel);
    }

    private void StartLevel1()
    {
        enemyController.MaxNumberOfShots = maxFireCount;
        enemyController.MarchSpeed = marchSpeed;

        //PlayerShipBonus = FIRST_BONUS_AMOUNT;
        WritePlayerCount();
        SetGameOverText(false);
        InstantiateNewPlayer();
        GameMusic.Play(GameMusicSource.GetComponent<AudioSource>());

        _manager.SetGrid();
        ResetPlayerScore();
    }

    private void InstantiateNewPlayer()
    {
		Debug.Log ("InstantiateNewPlayer");
		PlayerManager.Instance.PlayerStart();
        //playerShipInstance = Instantiate(Playership, new Vector3(0, -4, 0), Quaternion.identity);
        //playerShipInstance.transform.parent = gameObject.transform;
    }

    public void ReleaseBulletCount()
    {
        enemyController.CurrentShotCount = Math.Max(enemyController.CurrentShotCount - 1, 0);
    }

    public void SetGrid()
    {
        elapsedTime = 0f;
        enemyController.CurrentShotCount = 0;

        enemyController.InitializeGameGrid();

        SetDisplayLevel();
    }

    private void FlyShipsInView()
    {
        elapsedTime = 0f;
        elaspedTimeThresh = SMOOTH_TIME;
        _state = GameState.FlyInView;
        enemyController.EnemyAdvanceSpeed = SMOOTH_ADVANCE;


        LevelDisplayText.enabled = false;
        SetGameOverText(false);
    }

    public void FlyOutOfView()
    {
        WritePlayerCount();
        FindTopShip();
        elapsedTime = 0f;
        elaspedTimeThresh = PAUSE;
        _state = GameState.FlyOutOfView;
        enemyController.EnemyAdvanceSpeed = SMOOTH_ADVANCE;
    }

    public void EarthDestroyed()
    {
		/*
        var shipManager = playerShipInstance.GetComponent<ShipCollision>();
        if (shipManager != null)
        {
            shipManager.PlayerHit(this);
        }
        */
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
        enemyController.FindTopShip();
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
        enemyController.FireRandomWeapon(PlayerManager.Instance.GetXPos());
    }

}
