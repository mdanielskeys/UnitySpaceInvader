using System.Runtime.InteropServices;
using UnityEngine;

public class EnemyActions : MonoBehaviour
{
    public GameObject EnemyBullet;
    private GameGridManager _manager;
    private Vector2 _lastPos;
    public int ShipValue;

    public void Start()
    {
        _manager = GetComponentInParent<GameGridManager>();
        _lastPos = transform.position;
    }

    public void DestroyedByPlayer()
    {
        if (_manager != null)
        {
            _manager.UpdatePlayerScore(ShipValue);
            _manager.KillEnemy(gameObject);
        }
    }
}
