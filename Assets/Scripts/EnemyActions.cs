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

    public void Update()
    {
        if (_manager.GetGameState() == GameGridManager.GameState.GameRunning)
        {

            _lastPos = transform.position;
            if (_lastPos.x + _manager.marchSpeed > 4.5 || _lastPos.x + _manager.marchSpeed < -4.5)
            {
                _manager.marchSpeed *= -1;
                _manager.AdvanceEnemies();
            }
            _lastPos.x += _manager.marchSpeed;
        }
        transform.position = _lastPos;
    }

    public void DestroyedByPlayer()
    {
        if (_manager != null)
        {
            _manager.UpdatePlayerScore(150);
        }
        Destroy(gameObject);
    }

    public void Advance(float y)
    {
        _lastPos = transform.position;
        _lastPos.y += y;
        transform.position = _lastPos;
    }

}
