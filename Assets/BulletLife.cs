using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLife : MonoBehaviour
{
    public Transform explosion;
    private Rigidbody2D _rb2D;

    // Use this for initialization
    void Start () {
        _rb2D = GetComponent<Rigidbody2D>();
        _rb2D.AddForce(new Vector2(0, 4), ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update () {
        if (_rb2D.transform.position.y > 6f)
        {
            Debug.Log("Detroy bullet!");
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Bullet collision");
        var v3 = other.gameObject.transform.position;
        Destroy(other.gameObject);
        Instantiate(explosion, v3, Quaternion.identity);
        Destroy(gameObject);
    }
}
