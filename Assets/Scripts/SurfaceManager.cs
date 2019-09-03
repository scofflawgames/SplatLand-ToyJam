using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceManager : MonoBehaviour
{
    private WallManager wallManager;

    private void Start()
    {
        wallManager = transform.parent.gameObject.GetComponent<WallManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("BluePaintBall"))
        {
            wallManager.DamageWall(10, "blue");
            //collision.gameObject.transform.parent = this.transform;
        }
        else if (collision.gameObject.CompareTag("RedPaintBall"))
        {
            print("RED HIT!!");
            wallManager.DamageWall(10, "red");
        }
    }

}
