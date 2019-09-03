using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float timer;


    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject, timer);
    }


}
