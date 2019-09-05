using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float timer;
    public bool randomizeTime;
    public float timerMin;
    public float timerMax;

    private void OnCollisionEnter(Collision collision)
    {
        if (!randomizeTime)
        {
            Destroy(gameObject, timer);
        }
        else
        {
            timer = Random.Range(timerMin, timerMax);
            Destroy(gameObject, timer);
        }
    }


}
