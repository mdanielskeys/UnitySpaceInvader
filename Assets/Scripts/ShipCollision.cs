using UnityEngine;

public class ShipCollision : MonoBehaviour
{

    public GameObject ShipExplosion;
    private GameGridManager _manager;

    void Start()
    {
        _manager = GetComponentInParent<GameGridManager>();
    }


    public void PlayerHit(GameGridManager manager)
    {
        manager.FlyOutOfView();
        var v3 = gameObject.transform.position;
        Instantiate(ShipExplosion, v3, Quaternion.identity);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "EnemyFire" || other.tag == "Enemy")
        {
            if (_manager != null)
            {
                //Debug.Log("Call SetGameOver");
                // _manager.FlyOutOfView();
            }

            var v3 = other.gameObject.transform.position;
            Destroy(other.gameObject);
            Instantiate(ShipExplosion, v3, Quaternion.identity);
            Destroy(gameObject);

        }
    }   
}
