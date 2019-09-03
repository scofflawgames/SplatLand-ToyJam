using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPaintball : MonoBehaviour
{
    [Header("Public References")]
    public GameObject projectile;
    public Transform originPoint;

    [Header("Public Variables")]
    public float speed;
    

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !GameManager.isPaused)
        {
            GameObject paintBall = Instantiate(projectile, originPoint.position, Quaternion.identity) as GameObject;
            paintBall.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        }
    }
}
