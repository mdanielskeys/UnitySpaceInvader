using System;
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
    public int MaxScreenBullets;
    private int bulletCount;

    // Use this for initialization
    void Start ()
    {
        bulletCount = 0;
	    _manager = GetComponentInParent<GameGridManager>();
	    rb2d = GetComponent<Rigidbody2D>();
	}

    public void BulletCallback()
    {
        Debug.Log("BulletCallback");
        bulletCount -= 1;
    }

    private bool CanFire()
    {
        return bulletCount < MaxScreenBullets;
    }


	// Update is called once per frame
	void Update () {        
        movex = Input.GetAxis("Horizontal");
	    var v3 = rb2d.transform.position;
	    v3.x = Mathf.Clamp(v3.x + (movex * Speed), -4.5f, 4.5f);
	    rb2d.transform.position = v3;

	    if (Input.GetButtonUp("Fire1") && CanFire())
	    {
	        var start = transform.position;
	        start.y += .5f;
	        var child = Instantiate(playerBullet, start, Quaternion.identity);
	        child.gameObject.GetComponent<BulletLife>()._moveShip = this;
	        bulletCount += 1;
	    }

	}

    private void BulletDiedDelagate()
    {
        bulletCount -= 1;
    }
}
