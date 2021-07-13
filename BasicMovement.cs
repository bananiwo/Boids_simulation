using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum Obstacle {N, L, R}; // No, (obstacle to the) Left, Right

public class BasicMovement : MonoBehaviour
{


    public float newVY = 0f;
    public float newVX = 0f;
    public float velocityNoiseTimer = 0f;
    public float rateOfVelChangeTimer = 0f;
    public float m_wanderTimer;
    protected float m_minDistance = 1f;
    public List<GameObject> detectedObstaclesList = new List<GameObject>();
    protected List<GameObject> closebyFish = new List<GameObject>();
    
    // [HideInInspector]
    // public enum Obstacle {N, L, R}; // No, (obstacle to the) Left, Right

    ///////////////// OBSTACLE AVOIDANCE
    // [HideInInspector]
    public Obstacle m_isObstacleDetected = Obstacle.N; 
    private Vector2 m_initialVelocityOnObstacleDetection = Vector2.zero;
    private float m_obstacleAvoidanceTimer = 0;
    public EdgeCollider2D m_colLeft;
    public EdgeCollider2D m_colRight;
    private CircleCollider2D m_circleCol;


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
        m_circleCol = GetComponent<CircleCollider2D>();
        m_wanderTimer = Random.Range(0f, 10f);

    }

    protected void FixedUpdate()
    {
        m_wanderTimer += Time.fixedDeltaTime;
    }

   virtual protected void OnTriggerEnter2D(Collider2D other)
   {
       Collider2D targetCol = other.gameObject.GetComponent<Collider2D>();
        if((m_isObstacleDetected == Obstacle.N) && (targetCol.tag == "Obstacle"))
        {
            if(m_colLeft.IsTouching(targetCol))
            {
                m_isObstacleDetected = Obstacle.L;
            }
            else if(m_colRight.IsTouching(targetCol))
            {
                m_isObstacleDetected = Obstacle.R;
            }
            m_initialVelocityOnObstacleDetection = GetComponent<Rigidbody2D>().velocity;
        }

        if((m_circleCol.IsTouching(targetCol)) && (targetCol.tag == "Obstacle")){
            //  detectedObstaclesList.Add(other.gameObject);
             Debug.Log("COLLISION WITH OBSTACLE! BAM BAM!");
        }

        if(other.tag == "RimTop"){
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
        Collider2D targetCol = other.gameObject.GetComponent<Collider2D>();
        if((targetCol.tag == "Obstacle") && (m_isObstacleDetected != Obstacle.N))
        {
           Debug.Log("KONIEC PRZESZKODY");
           m_isObstacleDetected = Obstacle.N;
        }
        
        if(other.tag == "Obstacle"){
           detectedObstaclesList.Remove(other.gameObject);
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
    public void setObstacleDetected(Obstacle o)
    {
        m_isObstacleDetected = o;
    }
    public Obstacle getObstacleDetected()
    {
        return m_isObstacleDetected;
    }
    public Vector2 getInitialVelocityOnTargetDetection()
    {
        return m_initialVelocityOnObstacleDetection;
    }
    public float getObstacleAvoidanceTimer()
    {
    return m_obstacleAvoidanceTimer;
    }
    public void incrementObstacleAvoidanceTimer()
    {
        m_obstacleAvoidanceTimer += Time.fixedDeltaTime;
    }
    public void resetObstacleAvoidanceTimer()
    {
        m_obstacleAvoidanceTimer = 0;
    }
}
