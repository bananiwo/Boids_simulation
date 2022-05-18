using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float m_viewRadius;
    [Range (0,360)]
    public float m_viewAngle;
    public float m_meshResolution = 0.3f;

    public LayerMask m_obstacleMask;


    public Vector2 dirFromAngle(float angleInDeg, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleInDeg += transform.eulerAngles.z;
        }
        return new Vector2(Mathf.Cos(angleInDeg * Mathf.Deg2Rad), Mathf.Sin(angleInDeg * Mathf.Deg2Rad));
    }
    
    // Not used - just for debugging
    void drawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(m_viewAngle * m_meshResolution);
        float stepAngleSize = m_viewAngle / stepCount;

        for(int i=0; i<=stepCount; i++)
        {
            float angle = 0;
            if(i%2 == 0){
                angle = transform.eulerAngles.z - (stepAngleSize * i) / 2;
            }
            else{
                angle = transform.eulerAngles.z + (stepAngleSize * (i - 1)) / 2;
            }
            angle = transform.eulerAngles.z - m_viewAngle / 2 + stepAngleSize * i;
            Debug.DrawLine(transform.position, (Vector2)transform.position + dirFromAngle(angle, true) * m_viewRadius, Color.black);
        }
    }

    public bool checkForCollisionAhead()
    {
        return Physics2D.Raycast(transform.position, transform.right, m_viewRadius, m_obstacleMask);
    }

    public Vector2 findUnobstructedDirection()
    {
        Vector2 bestDir = transform.right;
        float furthestUnobstructedDst = 0;
        RaycastHit2D hit;

        int stepCount = Mathf.RoundToInt(m_viewAngle * m_meshResolution);
        float stepAngleSize = m_viewAngle / stepCount;

        for(int i=0; i<=stepCount; i++)
        {
            float angle = 0;
            if(i%2 == 0){
                angle = transform.eulerAngles.z - (stepAngleSize * i) / 2;
            }
            else{
                angle = transform.eulerAngles.z + (stepAngleSize * (i - 1)) / 2;
            }
            // Vector2 dir = (Vector2)transform.position + dirFromAngle(angle, true);

            hit = Physics2D.Raycast(transform.position, dirFromAngle(angle, true), m_viewRadius, m_obstacleMask);
            if(hit == true)
            {
                Debug.Log("OBSTACLE HIT");
                if(hit.distance > furthestUnobstructedDst)
                {
                    furthestUnobstructedDst = hit.distance;
                    bestDir = dirFromAngle(angle, true);
                    
                }
            }else{
                // Debug.DrawLine(transform.position, (Vector2)transform.position + dirFromAngle(angle, true) * m_viewRadius, Color.blue);
                return dirFromAngle(angle, true);
            }
        }
        // Debug.DrawLine(transform.position, (Vector2)transform.position + bestDir* m_viewRadius, Color.yellow);
        return bestDir;
    }
}
