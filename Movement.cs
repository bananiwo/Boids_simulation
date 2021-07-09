﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private float m_maxSpeed = 7f;
    private float m_minSpeed = 1f;
    private float m_minDistance = 1f;
    private float m_rotation = 0f;
    private float m_weightAllign = 0.1f;
    private float m_weightCenter = 0.1f;
    private float m_weightKeepDistance = 0.1f;
    private float m_weightObstacles = 0.3f;
    private float m_speedNoiseIntensity = 0.5f;
    private float m_speedNoisePeriod = 0.3f;
    private List<GameObject> closebyFish = new List<GameObject>();
    private List<GameObject> closebyObstacles = new List<GameObject>();

    private Rigidbody2D m_rb;
    private float speedNoiseTimer = 0f;


    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {   
        if(closebyFish.Count > 0){
            allignToFlock();
            centerToFlock();
            keepDistance();
        }

        if(closebyObstacles.Count > 0){
            avoidObstacles();
        }
        if(speedNoiseTimer > m_speedNoisePeriod + Random.Range(0f, 0.5f * m_speedNoisePeriod)){
            speedNoise();
        }
        regulateSpeedMinMax();
        rotateTowardsVelocity();
        speedNoiseTimer += Time.fixedDeltaTime;
    }

    private void avoidObstacles(){
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

   private void OnTriggerEnter2D(Collider2D other)
   {
       if(other.tag == "Fish"){
            closebyFish.Add(other.gameObject);
       }
        if(other.tag == "Shark"){
            closebyObstacles.Add(other.gameObject);
       }
   }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Fish"){
            closebyFish.Remove(other.gameObject);
        }
        if(other.tag == "Shark"){
            closebyObstacles.Remove(other.gameObject);
       }
    }

    private void regulateSpeedMinMax(){
        if(m_rb.velocity.magnitude < m_minSpeed){
            m_rb.velocity *= 1.25f;
        }
        if(m_rb.velocity.magnitude > m_maxSpeed){
            m_rb.velocity *= 0.75f;
        }
    }

    private void speedNoise(){
            float newVX = m_rb.velocity.x + (m_rb.velocity.x * Random.Range(-m_speedNoiseIntensity, m_speedNoiseIntensity));
            float newVY = m_rb.velocity.y + (m_rb.velocity.y * Random.Range(-m_speedNoiseIntensity, m_speedNoiseIntensity));
            m_rb.velocity = new Vector2(newVX, newVY);
            speedNoiseTimer = 0f;
    }

    private void allignToFlock(){
        float newVX = m_rb.velocity.x + m_weightAllign * (avgFlockVelX() - m_rb.velocity.x);
        float newVY = m_rb.velocity.y + m_weightAllign * (avgFlockVelY() - m_rb.velocity.y);
        m_rb.velocity = new Vector2(newVX, newVY);
    }

    private void centerToFlock(){
        float newVX = m_rb.velocity.x + m_weightCenter * (avgFlockPosX() - transform.position.x);
        float newVY = m_rb.velocity.y + m_weightCenter * (avgFlockPosY() - transform.position.y);
        m_rb.velocity = new Vector2(newVX, newVY);
    }

    private void keepDistance(){
        float targetPosX = 0;
        float targetPosY = 0;
        float distanceToClosest = 9999999;
        float myPosX = transform.position.x;
        float myPosY = transform.position.y;

        foreach(var fish in closebyFish){
            float tempX = fish.transform.position.x;
            float tempY = fish.transform.position.y;
            float distance = Mathf.Sqrt((tempX-myPosX)*(tempX-myPosX)+(tempY-myPosY)*(tempY-myPosY));
            if(distance < distanceToClosest){
                distanceToClosest = distance;
                targetPosX = fish.transform.position.x;
                targetPosY = fish.transform.position.y;
            }
        }

        float newVX = m_rb.velocity.x - m_weightCenter * ((targetPosX-myPosX)*m_minDistance/distanceToClosest - (targetPosX-myPosX));
        float newVY = m_rb.velocity.y - m_weightCenter * ((targetPosY-myPosY)*m_minDistance/distanceToClosest - (targetPosY-myPosY));
        m_rb.velocity = new Vector2(newVX, newVY);      
    }

    private float avgFlockVelX(){
        float avgVX = 0f;
        foreach(var fish in closebyFish){
            avgVX += fish.GetComponent<Rigidbody2D>().velocity.x;
        }
        return avgVX / closebyFish.Count;
    }

    private float avgFlockVelY(){
        float avgVY = 0f;
        foreach(var fish in closebyFish){
            avgVY += fish.GetComponent<Rigidbody2D>().velocity.y;
        }
        return avgVY / closebyFish.Count;
    }

    private float avgFlockPosX(){
        float avgX = 0f;
        foreach(var fish in closebyFish){
            avgX += fish.transform.position.x;
        }
        return avgX / closebyFish.Count;
    }

    private float avgFlockPosY(){
        float avgY = 0f;
        foreach(var fish in closebyFish){
            avgY += fish.transform.position.y;
        }
        return avgY / closebyFish.Count;
    }

    private void rotateTowardsVelocity(){
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
    public void setWeightAllignment(float weightAllignment){
        m_weightAllign = weightAllignment;
    }
    public void setWeightCenter(float weightCenter){
        m_weightCenter = weightCenter;
    }
    public void setWeightKeepDistance(float weightKeepDistance){
        m_weightKeepDistance = weightKeepDistance;
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
