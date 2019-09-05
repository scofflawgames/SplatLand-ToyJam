using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    public int blueHP = 100; //may need hp for blue and red separate
    public int redHP = 100;
    public GameObject blueWall;
    public GameObject redWall;

    private bool wallCounted = false;
    void Start()
    {
        
    }

    private void Update()
    {
        if (wallCounted)
        {
            GameManager.wallDestroyed = true;
        }
    }

    public void DamageWall(int damageAmount, string damageType)
    {
        if (damageType == "blue" && blueHP > 0)
        {
            blueHP -= 10;
        }
        else if (damageType == "red" && redHP > 0)
        {
            redHP -= 10;
        }

        //wallHP -= damageAmount;
        if (blueHP == 0 && !wallCounted)
        {
            //print("Block is blue!!");
            Instantiate(blueWall, transform.position, transform.rotation);
            if (!wallCounted)
            {
                WallCount wallCount = GetComponentInParent<WallCount>();
                wallCount.removeWall();
                wallCounted = true;
            }
            Destroy(gameObject);
            //broadcast message to all children to delete all of their children and turn to specific color
        }
        else if (redHP == 0 && !wallCounted)
        {
            //print("Block is red!!");
            Instantiate(redWall, transform.position, transform.rotation);
            if (!wallCounted)
            {
                WallCount wallCount = GetComponentInParent<WallCount>();
                wallCount.removeWall();
                wallCounted = true;
            }
            Destroy(gameObject);
        }
    }
}
