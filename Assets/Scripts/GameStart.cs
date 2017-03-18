using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameStart : MonoBehaviour
{
    public enum GameState
    {
        GameOver = 1,
        GameRunning = 2
    };

    private GameState _state;
    private GameGridManager _manager;
    public GameObject Playership;

    // Use this for initialization
    void Start ()
    {
        _manager = GetComponent<GameGridManager>();
        _manager.marchSpeed = .01f;

	    _state = GameState.GameOver;
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Alpha1) && _state == GameState.GameOver)
	    {
	        _state = GameState.GameRunning;
	        StartLevel1();
	    }
	}

    private void StartLevel1()
    {
        Instantiate(Playership, new Vector3(0, -4, 0), Quaternion.identity);
        _manager.SetGrid();
    }
}
