using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementShark : MonoBehaviour
{
    public float m_speed = 5f;
    public float m_turnNoiseIntensity = 0.5f;
    public float m_turnNoisePeriod = 1f;

    private Rigidbody2D m_rb;
    private float turnNoiseTimer = 0f;
    private float m_randomAngle;

    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_rb.velocity = new Vector2(Random.Range(-m_speed, m_speed), Random.Range(-m_speed, m_speed));
    }

    void FixedUpdate()
    {
        moveForward();

        if(turnNoiseTimer > m_turnNoisePeriod){
            turnNoise();
        }
        
        m_rb.MoveRotation(m_rb.rotation + m_randomAngle * Time.fixedDeltaTime);
        turnNoiseTimer += Time.fixedDeltaTime;
    }

    private void moveForward()
    {
        m_rb.MovePosition(transform.position + 0.01f * m_speed * transform.right);

    }

    private void turnNoise()
    {
        m_randomAngle = Random.Range(-m_turnNoiseIntensity, m_turnNoiseIntensity);
        turnNoiseTimer = 0f;
    }
}
