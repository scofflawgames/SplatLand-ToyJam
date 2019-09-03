using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBallSplat : MonoBehaviour
{
    public Transform splatObjectTransform;
    public GameObject splatObject;

    private Rigidbody paintBallRB;

    private void Awake()
    {
        splatObject = GameObject.FindGameObjectWithTag("Splat");
        splatObjectTransform = splatObject.transform;
        paintBallRB = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            ContactPoint contact = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;
            
            MeshRenderer splatMesh = splatObject.GetComponent<MeshRenderer>();

            Instantiate(splatObjectTransform, pos, rot);
            splatMesh.enabled = true;
        }
        Destroy(gameObject);
    }

}
