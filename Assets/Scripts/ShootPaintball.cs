using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPaintball : MonoBehaviour
{
    public GameObject projectile;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject paintBall = Instantiate(projectile, this.transform.position, Quaternion.identity) as GameObject;
            paintBall.GetComponent<Rigidbody>().AddForce(transform.forward * 200);
        }
    }
}
