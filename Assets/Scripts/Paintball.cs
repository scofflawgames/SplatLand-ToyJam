using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paintball : MonoBehaviour
{
    public string paintBallColor;
    public GameObject specificSplat;

    private void Awake()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        FindClosestSplat();
        if (specificSplat != null)
        {
            specificSplat.transform.parent = collision.gameObject.transform;
        }
    }

    public GameObject FindClosestSplat()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Splat");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                specificSplat = closest;
                distance = curDistance;
            }
        }
        return closest;
        
    }
}
