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

    public GameObject Playership;
    public GameObject DoomdayShip;
    public GameObject Enemy1Ship; 
    public GameObject Enemy2Ship;
    public GameObject Enemy3Ship;


    // Use this for initialization
    void Start ()
	{
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

        GameObject shipObject;
        for (var y = 4; y >= -1; y--)
        {
            switch (y)
            {
                case 4:
                    shipObject = DoomdayShip;
                    break;
                case 3:
                case 2:
                    shipObject = Enemy1Ship;
                    break;
                case 1:
                case 0:
                    shipObject = Enemy2Ship;
                    break;
                case -1:
                    shipObject = Enemy3Ship;
                    break;
                default:
                    shipObject = DoomdayShip;
                    break;
            }

            for (var x = -4; x <= 4; x++)
            {
                Instantiate(shipObject, new Vector3(x, y, 0), Quaternion.identity);
            }
        }
    }
}
