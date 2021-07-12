using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlockMovement : MonoBehaviour

{
    public Text textfield;
    
    [Header("Flock settings")]
    [Range(0, 150)] public int m_entitiesCount = 1;
    public GameObject m_prefab;
    public float m_width = 5.5f;
    public float m_height = 4.2f;
    [Header("Entity settings")]
    [SerializeField][Range(0f, 5f)]
    protected float m_maxSpeed = 3;
    [SerializeField][Range(0f, 5f)]
    protected float m_minSpeed = 1f;
    [SerializeField] [Range(0f, 3f)]
    protected float m_rimAvoidanceCoeff = 0.5f;

    [Header("Wander settings")]
    [SerializeField] [Range(0.1f, 10f)]
    private float m_wanderSmoothness = 4f;
    [SerializeField] [Range(0f, 0.7f)]
    private float m_weightWander = 1f;
    private float m_wanderTimer = 0f;

    [Header("Flock settings")]
    [SerializeField] [Range(0.3f, 1.3f)]
    private float m_minDistance = 1f;
    [SerializeField] [Range(0f, 1f)]
    private float m_weightAllign = 0.1f;
    [SerializeField] [Range(0f, 1f)]
    private float m_weightCenter = 0.1f;
    [SerializeField] [Range(0f, 1f)]
    private float m_weightKeepDistance = 0.1f;

    private List<GameObject> m_entities;
        BasicMovement movementScript;

    void Start()
    {
        m_entities = new List<GameObject>();
        for(int i=0; i<m_entitiesCount; i++)
        {
            GameObject entity = spawnEntity(m_prefab);
            entity.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-m_maxSpeed, m_maxSpeed),Random.Range(-m_maxSpeed, m_maxSpeed));
            m_entities.Add(entity);
        }
    }
    
    ////////////////////////////// LICZNIK KIERUNKU RYB
    void Update()
    {
        int left=0;
        int right=0;
        int up=0;
        int down=0;
        foreach(var e in m_entities){
            Rigidbody2D rb = e.GetComponent<Rigidbody2D>();
            if(rb.velocity.x > 0) right++;
            if(rb.velocity.x < 0) left++;
            if(rb.velocity.y > 0) up++;
            if(rb.velocity.y < 0) down++;
        }
        textfield.text = "left " + left + " | right " + right + " | up " + up + " | down " + down;
    }
    //////////////////////////////

    void FixedUpdate()
    {
        foreach(var entity in m_entities)
        {
            // avoidObstacles(entity);
            // avoidOuterRim(entity);
            Rigidbody2D rb = entity.GetComponent<Rigidbody2D>();
            Vector2 newVelocity = rb.velocity + m_weightWander * wander(entity) + m_weightAllign * allignToFlock(entity) + m_weightCenter * centerToFlock(entity) + m_weightKeepDistance * keepDistanceInFlock(entity);
            newVelocity = regulateVelocityMinMax(newVelocity);
            rb.velocity = newVelocity;
            faceTowardsVelocity(entity);
            stayInCamera(entity);
        }
    }

    private void stayInCamera(GameObject entity) // teleport
    {
        float coeff = 0.9f;
        Transform trans = entity.transform;
        Vector2 position = entity.transform.position;
        if(position.x > m_width * coeff)
        {
            trans.position = new Vector2(-m_width * coeff, position.y);
        }
        if(position.x < -m_width * coeff)
        {
            trans.position = new Vector2(m_width * coeff, position.y);
        }
        if(position.y > m_height * coeff)
        {
            trans.position = new Vector2(position.x, -m_height * coeff);
        }
        if(position.y < -m_height * coeff)
        {
            trans.position = new Vector2(position.x, m_height * coeff);
        }
    }

    private Vector2 allignToFlock(GameObject entity)
    {
        if(entity.GetComponent<BasicMovement>().getClosebyFishCount() == 0)
        {
            return new Vector2(0,0);
        }
        Rigidbody2D rb = entity.GetComponent<Rigidbody2D>();
        float velXCorrection = rb.velocity.x + m_weightAllign * (avgFlockVelX(entity) - rb.velocity.x);
        float velYCorrection = rb.velocity.y + m_weightAllign * (avgFlockVelY(entity) - rb.velocity.y);
        return new Vector2(velXCorrection, velYCorrection);
    }

    private Vector2 centerToFlock(GameObject entity)
    {
        if(entity.GetComponent<BasicMovement>().getClosebyFishCount() == 0)
        {
            return new Vector2(0,0);
        }
        Rigidbody2D rb = entity.GetComponent<Rigidbody2D>();
        float velXCorrection = rb.velocity.x + m_weightCenter * (avgFlockPosX(entity) - entity.transform.position.x);
        float velYCorrencion = rb.velocity.y + m_weightCenter * (avgFlockPosY(entity) - entity.transform.position.y);
        return new Vector2(velXCorrection, velYCorrencion);
    }

    private Vector2 keepDistanceInFlock(GameObject entity){
        if(entity.GetComponent<BasicMovement>().getClosebyFishCount() == 0)
        {
            return new Vector2(0,0);
        }
        BasicMovement bm = entity.GetComponent<BasicMovement>();
        Rigidbody2D rb = entity.GetComponent<Rigidbody2D>();
        float targetPosX = 0;
        float targetPosY = 0;
        float distanceToClosest = 9999999;
        float myPosX = entity.transform.position.x;
        float myPosY = entity.transform.position.y;

        foreach(var neighbour in bm.getClosebyFish()){
            float tempX = neighbour.transform.position.x;
            float tempY = neighbour.transform.position.y;
            float distance = Mathf.Sqrt((tempX-myPosX)*(tempX-myPosX)+(tempY-myPosY)*(tempY-myPosY));
            if(distance < distanceToClosest){
                distanceToClosest = distance;
                targetPosX = neighbour.transform.position.x;
                targetPosY = neighbour.transform.position.y;
            }
        }

        float velXCorrection = rb.velocity.x - m_weightCenter * ((targetPosX-myPosX)*m_minDistance/distanceToClosest - (targetPosX-myPosX));
        float velYCorrection = rb.velocity.y - m_weightCenter * ((targetPosY-myPosY)*m_minDistance/distanceToClosest - (targetPosY-myPosY));
        return new Vector2(velXCorrection, velYCorrection);      
    }

    private float avgFlockVelX(GameObject entity){
        BasicMovement bm = entity.GetComponent<BasicMovement>();
        if(bm.getClosebyFishCount() == 0)
        {
            return 0;
        }
        float avgVX = 0f;
        foreach(var fish in bm.getClosebyFish()){
            avgVX += fish.GetComponent<Rigidbody2D>().velocity.x;
        }
        return avgVX / bm.getClosebyFishCount();
    }

    private float avgFlockVelY(GameObject entity){
        BasicMovement bm = entity.GetComponent<BasicMovement>();
        if(bm.getClosebyFishCount() == 0)
        {
            return 0;
        }
        float avgVY = 0f;
        foreach(var fish in bm.getClosebyFish()){
            avgVY += fish.GetComponent<Rigidbody2D>().velocity.y;
        }
        return avgVY / bm.getClosebyFishCount();
    }

    private float avgFlockPosX(GameObject entity){
        BasicMovement bm = entity.GetComponent<BasicMovement>();
        if(bm.getClosebyFishCount() == 0)
        {
            return 0;
        }
        float avgX = 0f;
        foreach(var fish in bm.getClosebyFish()){
            avgX += fish.transform.position.x;
        }
        return avgX / bm.getClosebyFishCount();
    }

    private float avgFlockPosY(GameObject entity){
        BasicMovement bm = entity.GetComponent<BasicMovement>();
        if(bm.getClosebyFishCount() == 0)
        {
            return 0;
        }
        float avgY = 0f;
        foreach(var fish in bm.getClosebyFish()){
            avgY += fish.transform.position.y;
        }
        return avgY / bm.getClosebyFishCount();
    }

    protected Vector2 wander(GameObject entity)
    {
        BasicMovement bm = entity.GetComponent<BasicMovement>();
        float regulatedPerinNoiseX = (Mathf.PerlinNoise((1/m_wanderSmoothness) * bm.m_wanderTimer, 0) - 0.5f);
        float regulatedPerinNoiseY = (Mathf.PerlinNoise(0, (1/m_wanderSmoothness) * bm.m_wanderTimer) - 0.5f);
        
        return new Vector2(-regulatedPerinNoiseX, regulatedPerinNoiseY).normalized;
    }

    protected Vector2 avoidOuterRim(GameObject entity)
    {
        Rigidbody2D rb = entity.GetComponent<Rigidbody2D>();
        BasicMovement script = entity.GetComponent<BasicMovement>();
        float velocityX = rb.velocity.x;
        float velocityY = rb.velocity.y;
        Vector2 result = new Vector2(0f, 0f);
        if(script.isTouchingOuterRimTop())
        {
            float vY = velocityY - m_rimAvoidanceCoeff;
            result = new Vector2(vY, velocityX);
        }
        if(script.isTouchingOuterRimBottom())
        {
            float vY = velocityY + m_rimAvoidanceCoeff;
            result = new Vector2(vY, velocityX);
        }
        

        return result;
    }

    protected Vector2 avoidObstacles(GameObject entity)
    {
        movementScript = entity.GetComponent<BasicMovement>();
        Rigidbody2D rb = entity.GetComponent<Rigidbody2D>(); 
        if(movementScript.closebyObstacles.Count == 0) return new Vector2(0f ,0f);

        float targetPosX = 0;
        float targetPosY = 0;
        float distanceToClosest = 9999999;
        float myPosX = transform.position.x;
        float myPosY = transform.position.y;

        foreach(var obstacle in movementScript.closebyObstacles){
            float tempX = obstacle.transform.position.x;
            float tempY = obstacle.transform.position.y;
            float distance = Mathf.Sqrt((tempX-myPosX)*(tempX-myPosX)+(tempY-myPosY)*(tempY-myPosY));
            if(distance < distanceToClosest){
                distanceToClosest = distance;
                targetPosX = obstacle.transform.position.x;
                targetPosY = obstacle.transform.position.y;
            }
        }
        float newVX = - ((targetPosX-myPosX)*m_minDistance/distanceToClosest + (targetPosX-myPosX));
        float newVY = - ((targetPosY-myPosY)*m_minDistance/distanceToClosest + (targetPosY-myPosY));
        return new Vector2(newVX, newVY); 
    }

    protected Vector2 regulateVelocityMinMax(Vector2 vel)
    {
        if(vel.magnitude < 0.7f * m_minSpeed){
            vel = vel.normalized * m_minSpeed;
        }
        if(vel.magnitude < m_minSpeed){
            vel *= 1.25f;
        }
        if(vel.magnitude > m_maxSpeed){
            vel *= 0.75f;
        }
        if(vel.magnitude > 1.3f * m_maxSpeed){
            vel = vel.normalized * 1.3f * m_maxSpeed;
        }
        return vel;
    }

    private void faceTowardsVelocity(GameObject entity)
    {
        Vector2 v = entity.GetComponent<Rigidbody2D>().velocity;
        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        entity.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private GameObject spawnEntity(GameObject prefab)
    {
        float avoidRimSpawnCoefficient = 0.8f;
        Vector2 spawnPos = new Vector2(Random.Range(-m_width, m_width), Random.Range(-m_height, m_height)) * avoidRimSpawnCoefficient;
        GameObject entity = Instantiate(prefab, spawnPos, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        return entity;
    }
}
