using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCount : MonoBehaviour
{
    public int wallCounts;
    public GameObject[] paintedWalls;

    public void removeWall()
    {
        wallCounts -= 1;

        if (wallCounts <= 0)
        {
            paintedWalls = GameObject.FindGameObjectsWithTag("RedWall");

            foreach (GameObject paintedWall in paintedWalls)
            {
                Destroy(paintedWall);
            }

            paintedWalls = GameObject.FindGameObjectsWithTag("BlueWall");

            foreach (GameObject paintedWall in paintedWalls)
            {
                Destroy(paintedWall);
            }
            GameManager.wallDestroyed = true;
        }
    }

}
