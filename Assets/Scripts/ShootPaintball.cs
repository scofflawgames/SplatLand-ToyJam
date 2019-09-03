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
    public float coolDown = 0.25f;
    private bool canShoot = true;

    private void Start()
    {
        
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !GameManager.isPaused && canShoot)
        {
            canShoot = false;
            GameObject paintBall = Instantiate(projectile, originPoint.position, Quaternion.identity) as GameObject;
            paintBall.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
            StartCoroutine(shootCoolDown(coolDown));
        }
    }


    IEnumerator shootCoolDown(float coolDownTime)
    {
        yield return new WaitForSeconds(coolDownTime);
        canShoot = true;
    }

}
