using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paintball : MonoBehaviour
{
    public string paintBallColor;
    public GameObject specificSplat;

    private void Awake()
    {
        //Fetch the Renderer from the GameObject
        Renderer rend = GetComponent<Renderer>();

        if (paintBallColor == "blue")
        {
            //Set the main Color of the Material to green
            rend.material.shader = Shader.Find("_Color");
            rend.material.SetColor("_Color", Color.blue);

            //Find the Specular shader and change its Color to red
            rend.material.shader = Shader.Find("Specular");
            rend.material.SetColor("_SpecColor", Color.blue);
        }
        else if (paintBallColor == "red")
        {
            //Set the main Color of the Material to green
            rend.material.shader = Shader.Find("_Color");
            rend.material.SetColor("_Color", Color.red);

            //Find the Specular shader and change its Color to red
            rend.material.shader = Shader.Find("Specular");
            rend.material.SetColor("_SpecColor", Color.red);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        FindClosestSplat();
        specificSplat.transform.parent = collision.gameObject.transform;
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
