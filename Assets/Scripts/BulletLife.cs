using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLife : MonoBehaviour
{
    public string EnemyTag;
    public Transform explosion;
    private Rigidbody2D _rb2D;
    private GameGridManager _manager;

    // Use this for initialization
    void Start () {
        _rb2D = GetComponent<Rigidbody2D>();
        _manager = GetComponentInParent<GameGridManager>();

        if (EnemyTag == "Player")
        {
            _rb2D.AddForce(new Vector2(0, -4), ForceMode2D.Impulse);
        }
        else
        {
            _rb2D.AddForce(new Vector2(0, 4), ForceMode2D.Impulse);
        }

    }

    // Update is called once per frame
    void Update () {
        if (_rb2D.transform.position.y > 6f || _rb2D.transform.position.y < -6f)
        {
            Debug.Log("Detroy bullet!");
            if (_manager != null)
            {
                _manager.ReleaseBulletCount();
            }
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == EnemyTag)
        {
            Debug.Log("Bullet collision");
            var v3 = other.gameObject.transform.position;
            Destroy(other.gameObject);
            Instantiate(explosion, v3, Quaternion.identity);
            Destroy(gameObject);
            if (_manager != null)
            {
                _manager.ReleaseBulletCount();
            }
        }

        if (other.tag == "Player")
        {
            Debug.Log("Setup out of player view");
            _manager.FlyOutOfView();
        }
    }
}
