using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCollision : MonoBehaviour
{

    public GameObject ShipExplosion;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "EnemyFire" || other.tag == "Enemy")
        {
            var v3 = other.gameObject.transform.position;
            Destroy(other.gameObject);
            Instantiate(ShipExplosion, v3, Quaternion.identity);
            Destroy(gameObject);
        }
    }   
}
