using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameGridManager : MonoBehaviour {
    public float marchSpeed;
    public int maxFireCount;

    public GameObject DoomdayShip;
    public GameObject Enemy1Ship;
    public GameObject Enemy2Ship;
    public GameObject Enemy3Ship;

    private float elapsedTime;
    private int fireCount;
    private float fireInterval;

    public void Update()
    {
        if (EnemyCount() <= 0)
        {
            elapsedTime += Time.deltaTime;
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

    public void SetGrid()
    {
        elapsedTime = 0f;
        fireCount = 0;
        fireInterval = 0f;
        for (var y = 5; y >= 0; y--)
        {
            GameObject shipObject;
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
                var newShip = Instantiate(shipObject, new Vector3(x, y *.7f, 0), Quaternion.identity);
                newShip.transform.parent = gameObject.transform;
            }
        }

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
                    enemyScript.Advance(-0.2f);
                }
            }
        }
    }

}
