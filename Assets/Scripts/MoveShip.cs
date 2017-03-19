using System.Collections;
using System.Collections.Generic;
using Boo.Lang.Runtime.DynamicDispatching;
using UnityEngine;

public class MoveShip : MonoBehaviour
{
    public GameObject playerBullet;
    public float Speed = 0f;
    private float movex;
    private Rigidbody2D rb2d;
    private GameGridManager _manager;

	// Use this for initialization
	void Start ()
	{
	    _manager = GetComponentInParent<GameGridManager>();
	    rb2d = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {        
        movex = Input.GetAxis("Horizontal");
	    var v3 = rb2d.transform.position;
	    v3.x = Mathf.Clamp(v3.x + (movex * Speed), -4.5f, 4.5f);
	    rb2d.transform.position = v3;

	    if (Input.GetButtonUp("Fire1"))
	    {
	        var start = transform.position;
	        start.y += .5f;
	        Instantiate(playerBullet, start, Quaternion.identity);
	    }

	}

}
