using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct2 : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 5);   
    }
}
