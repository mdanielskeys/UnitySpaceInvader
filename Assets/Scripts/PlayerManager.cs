using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
	public static PlayerManager Instance;
	public int NumberOfLives;
	public float Speed;
	public GameObject LaserPulse;
    public GameObject ShipExplosion;
    public bool IsAlive;

    private float _playerBounds = 4.7f;
    private static int LiveCount;

    public float GetXPos()
    {
        return Instance.transform.position.x;
    }

    void Awake() 
	{
		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);
		}

	    LiveCount = NumberOfLives;

		DontDestroyOnLoad (gameObject);

		gameObject.SetActive (false);

	    IsAlive = false;
	}

	void Start()
	{

	}

	// Update is called once per frame
	void Update () {
		MoveShip ();
		FireWeapons ();
	}

	public void PlayerStart()
	{
		transform.position = new Vector3 (0, transform.position.y);

	    IsAlive = true;
		gameObject.SetActive (true);
	}

    public int GetLivesLeft()
    {
        return LiveCount;
    }

    private void MoveShip()
	{
        var movex = Input.GetAxis("Horizontal");
        var pos = transform.position;
        pos.x += movex * Speed;
        if (IsMoveable(pos))
	    {
	        transform.Translate(new Vector3(movex * Speed, 0, 0));
	    }
	}

    private bool IsMoveable(Vector3 newPosition)
    {
        return (newPosition.x <= _playerBounds && newPosition.x >= -_playerBounds);
    }

    private void FireWeapons()
	{
		if (Input.GetButtonUp("Fire1"))
		{
			var laserPulseStartPosition = transform.position;
			laserPulseStartPosition.y += .5f;

			Instantiate(LaserPulse, laserPulseStartPosition, Quaternion.identity);
		}
	}

    void OnTriggerEnter2D(Component other)
    {
        if (other.tag == "PlayerFire") return;

        var v3 = other.gameObject.transform.position;
        Instantiate(ShipExplosion, v3, Quaternion.identity);
        gameObject.SetActive(false);
        IsAlive = false;
        LiveCount -= 1;
    }
}
