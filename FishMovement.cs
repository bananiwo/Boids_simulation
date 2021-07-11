using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : BasicMovement
{

    private float m_rotation = 0f;
    private float m_weightAllign = 0.1f;
    private float m_weightCenter = 0.1f;
    private float m_weightKeepDistance = 0.1f;

    /*override protected void FixedUpdate()
    {   
        base.FixedUpdate();
        if(closebyFish.Count > 0){
            allignToFlock();
            centerToFlock();
            keepDistance();
        }
    }*/

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



    public void setWeightAllignment(float weightAllignment){
        m_weightAllign = weightAllignment;
    }
    public void setWeightCenter(float weightCenter){
        m_weightCenter = weightCenter;
    }
    public void setWeightKeepDistance(float weightKeepDistance){
        m_weightKeepDistance = weightKeepDistance;
    }


}
