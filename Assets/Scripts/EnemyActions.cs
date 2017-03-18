using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActions : MonoBehaviour
{

    private GameGridManager _manager;

    public void Start()
    {
        _manager = GetComponentInParent<GameGridManager>();
        
    }

    public void Update()
    {
        var pos = transform.position;
        if (pos.x + _manager.marchSpeed > 4.5 || pos.x + _manager.marchSpeed < -4.5)
        {
            _manager.marchSpeed *= -1;
        }
        pos.x += _manager.marchSpeed;
        transform.position = pos;
    }
}
