using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceManager : MonoBehaviour
{
    private WallManager wallManager;
    private GameObject audioManager;
    private AudioSource audioSource;
    private AudioClip splat;

    private void Start()
    {
        wallManager = transform.parent.gameObject.GetComponent<WallManager>();
        audioManager = GameObject.FindGameObjectWithTag("SoundManager");
        splat = (AudioClip)Resources.Load("Sounds/splash001");
    }

    private void OnCollisionEnter(Collision collision)
    {
        audioSource = audioManager.GetComponent<AudioSource>();
        audioSource.clip = splat;
        float audioPitch = Random.Range(0.65f, 1.25f);

        if (collision.gameObject.CompareTag("BluePaintBall"))
        {
            audioSource.pitch = audioPitch;
            audioSource.Play();
            wallManager.DamageWall(10, "blue");
            //collision.gameObject.transform.parent = this.transform;
        }
        else if (collision.gameObject.CompareTag("RedPaintBall"))
        {
            //print("RED HIT!!");
            wallManager.DamageWall(10, "red");
        }
    }

}
