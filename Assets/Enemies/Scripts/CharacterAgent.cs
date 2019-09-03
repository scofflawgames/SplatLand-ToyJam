using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class CharacterAgent : MonoBehaviour
{
    public GameObject characterDestination;
    public GameObject characterDestination2;
    public Transform redShootOrigin;
    public GameObject redPaintBall;
    public int redBallSpeed;
    NavMeshAgent theAgent;
    public bool goRight = false;

        // Start is called before the first frame update
    void Start()
    {
        theAgent = GetComponent<NavMeshAgent>();
        InvokeRepeating("ShootPaintBall", 2.0f, 0.25f);
    }

       // Update is called once per frame
    void Update()
    {
        if (!goRight)
        {
            redShootOrigin.rotation = new Quaternion(0, 90, 0, 0);
            theAgent.SetDestination(characterDestination.transform.position);
        }
        else if (goRight)
        {
            redShootOrigin.rotation = new Quaternion(0, -90, 0, 0);
            theAgent.SetDestination(characterDestination2.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       if (other.gameObject.CompareTag("LeftBox"))
        {
            goRight = true;
        }
        else if (other.gameObject.CompareTag("RightBox"))
        {          
            goRight = false;
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

