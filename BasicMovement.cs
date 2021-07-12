using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{


    public float newVY = 0f;
    public float newVX = 0f;
    public float velocityNoiseTimer = 0f;
    public float rateOfVelChangeTimer = 0f;
    public float m_wanderTimer;
    protected float m_minDistance = 1f;
    public List<GameObject> closebyObstacles = new List<GameObject>();
    protected List<GameObject> closebyFish = new List<GameObject>();
    public Rigidbody2D m_rb;

    private bool touchOuterRimLeft = false;
    private bool touchOuterRimRight = false;
    private bool touchOuterRimTop = false;
    private bool touchOuterRimBottom = false;

    public bool isTouchingOuterRimLeft() { return touchOuterRimLeft; }
    public bool isTouchingOuterRimRight() { return touchOuterRimRight; }
    public bool isTouchingOuterRimTop() { return touchOuterRimTop; }
    public bool isTouchingOuterRimBottom() { return touchOuterRimBottom; }

    protected void Start()
    {
        m_wanderTimer = Random.Range(0f, 10f);
    }

    protected void FixedUpdate()
    {
        m_wanderTimer += Time.fixedDeltaTime;
    }

   virtual protected void OnTriggerEnter2D(Collider2D other)
   {
        if(other.tag == "Obstacle"){
             closebyObstacles.Add(other.gameObject);
        }
        else if(other.tag == "RimTop"){
            touchOuterRimTop = true;
        }
        else if(other.tag == "RimBottom"){
            touchOuterRimBottom = true;
        }
        else if(other.tag == "RimLeft"){
            touchOuterRimLeft = true;
        }
        else if(other.tag == "RimRight"){
            touchOuterRimRight = true;
        }
        else if(other.tag == "Fish"){
            closebyFish.Add(other.gameObject);
       }
   }
   
    virtual protected void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Obstacle"){
           closebyObstacles.Remove(other.gameObject);
        }
        else if(other.tag == "RimTop"){
            touchOuterRimTop = false;
        }
        else if(other.tag == "RimBottom"){
            touchOuterRimBottom = false;
        }
        else if(other.tag == "RimLeft"){
            touchOuterRimLeft = false;
        }
        else if(other.tag == "RimRight"){
            touchOuterRimRight = false;
        }
       if(other.tag == "Fish"){
            closebyFish.Remove(other.gameObject);
        }
    }
    public List<GameObject> getClosebyFish()
    {
        return closebyFish;
    }
    public int getClosebyFishCount()
    {
        return closebyFish.Count;
    }
}
