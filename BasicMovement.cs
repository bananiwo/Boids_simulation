using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    [SerializeField]
    protected float m_maxSpeed = 3;
    [SerializeField]
    protected float m_minSpeed = 1f;
    [SerializeField] 
    protected float m_constantSpeed = 2f;
    [SerializeField] 
    protected float m_minDistance = 1f;
    [SerializeField] [Range(0f, 0.3f)]
    protected float m_weightObstacles = 0.2f;
    [SerializeField] [Range(0f, 0.5f)]
    protected float m_speedNoiseIntensity = 0.5f;
    [SerializeField]
    protected float m_speedNoisePeriod = 0.3f;
    [SerializeField]
    protected float m_directionNoiseIntensity = 0.3f;
    [SerializeField]
    protected bool m_keepConstantSpeed;
    [SerializeField]
    protected float rateOfVelChangeDuration;

    protected float velocityNoiseTimer = 0f;
    protected float directionNoiseTimer = 0f;
    protected float rateOfVelChangeTimer = 0f;
    protected float directionChangeTimer = 0f;
    float newVX = 0f;
    float newVY = 0f;

    protected List<GameObject> closebyObstacles = new List<GameObject>();
    protected Rigidbody2D m_rb;

    virtual protected void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        if(m_constantSpeed < m_minSpeed) m_constantSpeed = m_minSpeed;
        if(m_constantSpeed > m_maxSpeed) m_constantSpeed = m_maxSpeed;

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
        
        rateOfVelChangeDuration = 1f;
        velocityNoiseTimer += Time.fixedDeltaTime;
        if(velocityNoiseTimer > m_speedNoisePeriod * Random.Range(0.8f, 1.2f)){
            rateOfVelChangeTimer = 0f;
            newVX = m_rb.velocity.x + (m_maxSpeed * Random.Range(-m_speedNoiseIntensity, m_speedNoiseIntensity));
            if(m_keepConstantSpeed){
                float maxSpeedSquared = m_maxSpeed * m_maxSpeed;
                newVY = maxSpeedSquared - newVX * newVX;
                if(newVY < 0.05f){
                    newVY = m_minSpeed;
                }
                newVY /= newVY;
            }
            if(!m_keepConstantSpeed){
                newVY = m_rb.velocity.y + (m_rb.velocity.y * Random.Range(-m_speedNoiseIntensity, m_speedNoiseIntensity));
            }
            velocityNoiseTimer = 0f;
        }
        rateOfVelChangeTimer += Time.fixedDeltaTime;
        if(rateOfVelChangeTimer > rateOfVelChangeDuration) {
            rateOfVelChangeTimer = 0f;
        }
        m_rb.velocity = Vector2.Lerp(m_rb.velocity, new Vector2(newVX, newVY), rateOfVelChangeTimer/rateOfVelChangeDuration);
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
    public void setVelocityNoiseIntensity(float speedNoiseIntensity){
        m_speedNoiseIntensity = speedNoiseIntensity;
    }   
    public void setVelocityNoisePeriod(float speedNoisePeriod){
        m_speedNoisePeriod = speedNoisePeriod;
    }
    public void setKeepConstantSpeed(bool keepConstantSpeed){
        m_keepConstantSpeed = keepConstantSpeed;
    }
    public void setRateOfVelChangeDuration(float rate){
        rateOfVelChangeDuration = rate;
    }



}
