using System.Collections;
using System.Collections.Generic;
using Boo.Lang.Runtime.DynamicDispatching;
using UnityEngine;

public class MoveShip : MonoBehaviour
{
    public Transform playerBullet;
    public float Speed = 0f;
    private float movex;
    private Rigidbody2D rb2d;

	// Use this for initialization
	void Start ()
	{
	    rb2d = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {        
        movex = Input.GetAxis("Horizontal");
	    var v3 = rb2d.transform.position;
	    v3.x = Mathf.Clamp(v3.x + (movex * Speed), -4.5f, 4.5f);
	    rb2d.transform.position = v3;

	    if (Input.GetKeyUp(KeyCode.Space))
	    {
	        Instantiate(playerBullet, GetComponent<Transform>().position, Quaternion.identity);
	    }
	}

}
