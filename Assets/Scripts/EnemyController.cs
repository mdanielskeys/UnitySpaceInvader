using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : ScriptableObject {
    private readonly int[] _gameGrid =
    {
        0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0,
        0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
        0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
    };

    private List<GameObject> _enemyShips;

    public int StartYPos = 6;
    public float StartingColumn = -4.0f;
    public int MaxColumnCount = 11;
    public float ShipHSpacing = 0.8f;
    public float ShipVSpacing = 0.7f;
    public float EnemyAdvanceSpeed { get; set; }
    public float FireThreshold = .8f;
    public float MarchSpeed = .7f;
    public int MarchDirection = 1;

    public GameObject DoomdayShip;
    public GameObject Enemy3Ship;
    public GameObject Enemy1Ship;
    public GameObject Enemy2Ship;
    public GameObject GameManager;

    private readonly float _topConst = 23f;

    public bool AreEnemiesInStartPos()
    {
        return FindTopShip().transform.position.y <= 4.0f;
    }

    public bool AreEnemiesInOffscreenPos()
    {
        return FindTopShip().transform.position.y <= -6.0f;
    }

    public bool AreEnemiesInView()
    {
        return FindTopShip().transform.position.y > -6.0f;
    }

    private void ClearExistingEnemies()
    {
        if (_enemyShips != null)
        {
            foreach (var e in _enemyShips)
            {
                Destroy(e);
            }

            _enemyShips.Clear();
        }
        else
        {
            _enemyShips = new List<GameObject>();
        }
    }

    public void KillEnemy(GameObject enemy)
    {
        _enemyShips.Remove(enemy);
        Destroy(enemy);
    }

    public GameObject FindTopShip()
    {
        if (_enemyShips == null) return null;

        GameObject topShip = null;
        foreach (var enemy in _enemyShips)
        {
            if (topShip == null)
            {
                topShip = enemy;
            }
            else if (enemy.transform.position.y > topShip.transform.position.y)
            {
                topShip = enemy;
            }
        }

        return topShip;
    }

    public int EnemyCount()
    {
        return _enemyShips.Count;
    }

    public void AdvanceEnemies()
    {
        if (_enemyShips == null) return;
        MarchDirection *= -1;

        foreach (var enemy in _enemyShips)
        {
            var pos = enemy.transform.position;
            pos.y += EnemyAdvanceSpeed;
            enemy.transform.position = pos;
        }
    }

    public void ResetShipsToTop()
    {
        if (_enemyShips == null) return;

        foreach (var enemy in _enemyShips)
        {
            var pos = enemy.transform.position;
            pos.y += _topConst;
            enemy.transform.position = pos;
        }
    }

    public bool FireRandomWeapon()
    {
        if (_enemyShips == null) return false;

        if (Random.value > FireThreshold)
        {
            var idx = (int) (Random.value * _enemyShips.Count);
            var enemy = _enemyShips[idx];
            if (enemy != null)
            {
                var enemyScript = enemy.GetComponent<EnemyActions>();
                if (enemyScript != null)
                {
                    var tranform = enemy.transform;
                    var bullet = Instantiate(enemyScript.EnemyBullet, tranform.position, Quaternion.identity);
                    bullet.transform.parent = GameManager.transform;
                    return true;
                }
            }
        }

        return false;
    }

    public bool ShouldAdvance()
    {
        return _enemyShips != null && _enemyShips.Any(enemy => Mathf.Abs(enemy.transform.position.x + MarchSpeed) > 4.5f);
    }

    public void MarchEnemies()
    {
        if (_enemyShips == null) return;

        foreach (var enemy in _enemyShips)
        {
            var pos = enemy.transform.position;
            pos.x += (MarchSpeed * MarchDirection);
            enemy.transform.position = pos;
        }
    }

    public void InitializeGameGrid()
    {
        ClearExistingEnemies();

        var starty = StartYPos;
        var column = StartingColumn;
        var columnCount = 0;
        var rowCount = 0;

        foreach (var elem in _gameGrid)
        {
            GameObject shipObject;
            switch (rowCount)
            {
                case 0:
                    shipObject = DoomdayShip;
                    break;
                case 1:
                    shipObject = Enemy3Ship;
                    break;
                case 2:
                    shipObject = Enemy1Ship;
                    break;
                case 3:
                case 4:
                    shipObject = Enemy2Ship;
                    break;
                default:
                    shipObject = DoomdayShip;
                    break;
            }
            if (elem == 1)
            {
                var newShip = Instantiate(shipObject, new Vector3(column, starty - (rowCount * ShipVSpacing), 0), Quaternion.identity);
                newShip.transform.parent = GameManager.transform;
                _enemyShips.Add(newShip);
            }
            column += ShipHSpacing;
            columnCount += 1;
            if (columnCount >= MaxColumnCount)
            {
                // Debug.Log(string.Format("Column {0}", column));
                column = StartingColumn;
                columnCount = 0;
                rowCount += 1;
            }
        }
    }
}
