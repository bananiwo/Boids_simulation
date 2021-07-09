using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    protected float m_maxSpeed = 7f;
    protected float m_minSpeed = 1f;
    protected float m_minDistance = 1f;
    protected float m_weightObstacles = 0.2f;
    protected float m_speedNoiseIntensity = 0.5f;
    protected float m_speedNoisePeriod = 0.3f;
    protected float speedNoiseTimer = 0f;

    protected List<GameObject> closebyObstacles = new List<GameObject>();
    protected Rigidbody2D m_rb;

    virtual protected void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();

    }

    virtual protected void FixedUpdate()
    {   
        avoidObstacles();
        speedNoise();
        regulateSpeedMinMax();
        faceVelocity();
    }

   virtual protected void OnTriggerEnter2D(Collider2D other)
   {
    if(other.tag == "Obstacle"){
           closebyObstacles.Add(other.gameObject);
       }
   }
   
    virtual protected void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Obstacle"){
           closebyObstacles.Remove(other.gameObject);
       }
    }


    protected void avoidObstacles(){
        if(closebyObstacles.Count == 0) return;

        float targetPosX = 0;
        float targetPosY = 0;
        float distanceToClosest = 9999999;
        float myPosX = transform.position.x;
        float myPosY = transform.position.y;

        foreach(var entity in closebyObstacles){
            float tempX = entity.transform.position.x;
            float tempY = entity.transform.position.y;
            float distance = Mathf.Sqrt((tempX-myPosX)*(tempX-myPosX)+(tempY-myPosY)*(tempY-myPosY));
            if(distance < distanceToClosest){
                distanceToClosest = distance;
                targetPosX = entity.transform.position.x;
                targetPosY = entity.transform.position.y;
            }
        }

        float newVX = m_rb.velocity.x - m_weightObstacles * ((targetPosX-myPosX)*m_minDistance/distanceToClosest + (targetPosX-myPosX));
        float newVY = m_rb.velocity.y - m_weightObstacles * ((targetPosY-myPosY)*m_minDistance/distanceToClosest + (targetPosY-myPosY));
        m_rb.velocity = new Vector2(newVX, newVY); 
    }

    protected void regulateSpeedMinMax(){
        if(m_rb.velocity.magnitude < 0.1f * m_minSpeed){
                m_rb.velocity = new Vector2(Random.Range(-m_minSpeed, m_minSpeed),Random.Range(-m_minSpeed, m_minSpeed));
        }
        if(m_rb.velocity.magnitude < m_minSpeed){
            m_rb.velocity *= 1.25f;
        }
        if(m_rb.velocity.magnitude > m_maxSpeed){
            m_rb.velocity *= 0.75f;
        }
    }

    protected void speedNoise(){
        speedNoiseTimer += Time.fixedDeltaTime;
        if(speedNoiseTimer > m_speedNoisePeriod + Random.Range(0f, 0.5f * m_speedNoisePeriod)){
            float newVX = m_rb.velocity.x + (m_rb.velocity.x * Random.Range(-m_speedNoiseIntensity, m_speedNoiseIntensity));
            float newVY = m_rb.velocity.y + (m_rb.velocity.y * Random.Range(-m_speedNoiseIntensity, m_speedNoiseIntensity));
            m_rb.velocity = new Vector2(newVX, newVY);
            speedNoiseTimer = 0f;
        }
    }

    protected void faceVelocity(){
        Vector2 v = m_rb.velocity;
        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void setMaxSpeed(float maxSpeed){
        m_maxSpeed = maxSpeed;
    }
    public void setMinSpeed(float minSpeed){
        m_minSpeed = minSpeed;
    }
    public void setMinDistance(float minDistance){
        m_minDistance = minDistance;
    }
    public void setWeightObstacles(float weightObstacles){
        m_weightObstacles = weightObstacles;
    }
    public void setSpeedNoiseIntensity(float speedNoiseIntensity){
        m_speedNoiseIntensity = speedNoiseIntensity;
    }   
    public void setSpeedNoisePeriod(float speedNoisePeriod){
        m_speedNoisePeriod = speedNoisePeriod;
    }
}
