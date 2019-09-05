using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class CharacterAgent : MonoBehaviour
{
    public GameObject characterDestination;
    public GameObject characterDestination2;
    public GameObject characterDestination3;
    public GameObject characterDestination4;
    public Transform redShootOrigin;
    public GameObject redPaintBall;
    public int redBallSpeed;
    //NavMeshAgent theAgent;
    public bool goRight = false;
    public bool goBack = false;
    public float coolDownTime;

        // Start is called before the first frame update
    void Start()
    {
        //theAgent = GetComponent<NavMeshAgent>();
        InvokeRepeating("ShootPaintBall", 1.0f, coolDownTime);
    }

       // Update is called once per frame
    void Update()
    {
       // NavMeshCode();
    }

    private void NavMeshCode()
    {
        if (!goRight && !goBack)
        {
            redShootOrigin.rotation = new Quaternion(0, 90, 20, 0);
            //theAgent.SetDestination(characterDestination.transform.position);
        }
        else if (goRight && !goBack)
        {
            redShootOrigin.rotation = new Quaternion(0, -90, 20, 0);
            //theAgent.SetDestination(characterDestination2.transform.position);
        }
        else if (goBack && !goRight)
        {
            redShootOrigin.rotation = new Quaternion(0, 90, 20, 0);
            //theAgent.SetDestination(characterDestination3.transform.position);
        }
        else if (goBack && goRight)
        {
            redShootOrigin.rotation = new Quaternion(0, 90, 20, 0);
            //theAgent.SetDestination(characterDestination4.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LeftBox"))
        {
            goRight = true;
            goBack = false;
        }
        else if (other.gameObject.CompareTag("RightBox"))
        {
            goRight = false;
            goBack = true;
        }
        else if (other.gameObject.CompareTag("BackBox"))
        {
            goRight = true;
            goBack = true;
        }
        else if (other.gameObject.CompareTag("FrontBox"))
        {
            goRight = false;
            goBack = false;
        }

    }

    void ShootPaintBall()
    {
        if (!goRight)
        {
            GameObject paintBall = Instantiate(redPaintBall, redShootOrigin.position, Quaternion.identity) as GameObject;
            paintBall.GetComponent<Rigidbody>().AddForce(transform.right * redBallSpeed);
        }
        else
        {
            GameObject paintBall = Instantiate(redPaintBall, redShootOrigin.position, Quaternion.identity) as GameObject;
            paintBall.GetComponent<Rigidbody>().AddForce(-transform.right * redBallSpeed);
        }
    }

 }

