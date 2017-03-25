using UnityEngine;

public class EnemyActions : MonoBehaviour
{
    public GameObject EnemyBullet;
    private GameGridManager _manager;
    private Vector2 _lastPos;

    public void Start()
    {
        _manager = GetComponentInParent<GameGridManager>();
        _lastPos = transform.position;
    }

    public void DestroyedByPlayer()
    {
        if (_manager != null)
        {
            _manager.UpdatePlayerScore(150);
            _manager.KillEnemy(gameObject);
        }
    }
}
