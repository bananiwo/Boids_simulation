using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankWalls : MonoBehaviour
{
    [Header("Tank settings")]
    public float m_width = 15f;
    public float m_height = 3.75f;
    public int fishCount = 10;
    [Header("Fish settings")]
    [SerializeField]private float m_maxSpeed = 7f;
    [SerializeField]private float m_minSpeed = 1f;
    [SerializeField]private float m_minDistance = 1f;
    [SerializeField]private float m_rotation = 0f;
    [SerializeField][Range(0f, 0.5f)] private float m_weightAllign = 0.1f;
    [SerializeField][Range(0f, 0.5f)]private float m_weightCenter = 0.1f;
    [SerializeField][Range(0f, 0.5f)]private float m_weightKeepDistance = 0.1f;
    [SerializeField][Range(0f, 0.5f)]private float m_weightObstacles = 0.3f;
    [SerializeField][Range(0f, 0.5f)]private float m_speedNoiseIntensity = 0.5f;
    [SerializeField][Range(0f, 1f)]private float m_speedNoisePeriod = 0.3f;
    [Space]
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
