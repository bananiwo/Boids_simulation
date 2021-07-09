using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankWalls : MonoBehaviour
{
    [Header("Tank settings")]
    public float m_width = 15f;
    public float m_height = 3.75f;
    [Range(5, 150)] public int fishCount = 10;

    public GameObject fishPrefab;
    public GameObject m_shark;

    private List<Transform> m_objects;
    
    void Start()
    {
        m_objects = new List<Transform>();

        for(int i=0; i<fishCount; i++){
            GameObject fish = spawnEntity(fishPrefab);
            m_objects.Add(fish.transform);
        }

         m_shark = spawnEntity(m_shark);
         m_objects.Add(m_shark.transform);
    }

    
    void FixedUpdate()
    {
        teleportOnBorder();
    }



    private GameObject spawnEntity(GameObject prefab){
        Vector2 spawnPos = new Vector2(Random.Range(-m_width, m_width), Random.Range(-m_height, m_height));
        GameObject entity = Instantiate(prefab, spawnPos, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        return entity;
    }

    private void teleportOnBorder(){
       foreach(Transform fishTransform in m_objects){
            if(fishTransform.position.x > m_width){
                fishTransform.position = new Vector2(-m_width, fishTransform.position.y);
            }
            else if(fishTransform.position.x < -m_width){
                fishTransform.position = new Vector2(m_width, fishTransform.position.y);
            }
            else if(fishTransform.position.y > m_height){
                fishTransform.position = new Vector2(fishTransform.position.x, -m_height);
            }
            else if(fishTransform.position.y < -m_height){
                fishTransform.position = new Vector2(fishTransform.position.x, m_height);
            }
        }

            float sharkPosX = m_shark.transform.position.x;
            float sharkPosY = m_shark.transform.position.y;
            if(sharkPosX > m_width){
                m_shark.transform.position = new Vector2(-m_width, sharkPosY);
            }
            else if(sharkPosX < -m_width){
                m_shark.transform.position = new Vector2(m_width, sharkPosY);
            }
            else if(sharkPosY > m_height){
                m_shark.transform.position = new Vector2(sharkPosX, -m_height);
            }
            else if(sharkPosY < -m_height){
                m_shark.transform.position = new Vector2(sharkPosX, m_height);
            }
        }
    
}
