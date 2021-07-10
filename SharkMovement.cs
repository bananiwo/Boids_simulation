using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkMovement : BasicMovement
{

    private float turnTimer = 0f;
    private float baseTurnDuration = 2f;
    private float turnSpeed = 0.05f;

    private float angle = 0f;
    private bool right = true;

    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
////////////////////////// turn
        turnTimer += Time.fixedDeltaTime;
        if(turnTimer >= Random.Range(0.6f, 1.3f) * baseTurnDuration){
            turnTimer = 0f;
            angle = Random.Range(30f, 80f);
            right = Random.Range(0, 2) != 1;
            if(right){
                angle *= -1;
            }
        }
        if(turnTimer < 0.5f * baseTurnDuration * Random.Range(0.2f, 1.3f)){
            float deltaAngle = angle * turnSpeed / baseTurnDuration;
            m_rb.MoveRotation(m_rb.rotation + deltaAngle);
        }

////////////////////////// go forwards
        Vector2 direction = transform.right;
        m_rb.velocity = direction *  2f;

//////////////////////////
        //avoidObstacles();
        //regulateSpeedMinMax();
        // faceVelocity();
    }

    override protected void OnTriggerEnter2D(Collider2D other)
   {
       base.OnTriggerEnter2D(other);
   }

    override protected void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
    }


}
