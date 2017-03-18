using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGridManager : MonoBehaviour {
    public float marchSpeed;

    public GameObject DoomdayShip;
    public GameObject Enemy1Ship;
    public GameObject Enemy2Ship;
    public GameObject Enemy3Ship;

    public void SetGrid()
    {
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

}
