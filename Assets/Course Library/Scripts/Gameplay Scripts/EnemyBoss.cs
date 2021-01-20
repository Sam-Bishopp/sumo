using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    private SpawnManager spawnManager;
    public float speed = 10f; //default speed of enemy boss
    private Rigidbody enemyBossRb;
    public GameObject bossPowerupIndicator;
    public float powerupStrength = 7.5f;
    private GameObject player;
    private float despawnLimit = -20f;
    public int pointValue = 50;


    // Start is called before the first frame update. Initialise rigidbody and find player's location
    void Start()
    {
        enemyBossRb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        
        //Instantiate(bossPowerupIndicator, transform.position, transform.rotation);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(spawnManager.isGameActive && !spawnManager.isGamePaused)
        {
            // ".normalized" keeps speed the same across any distance
            Vector3 lookDirection = (player.transform.position - transform.position).normalized;
            enemyBossRb.AddForce(lookDirection * speed);
        }

        /*Vector3 indicatorOffset = new Vector3(0, -0.4f, 0);
        bossPowerupIndicator.transform.position = transform.position + indicatorOffset;*/ 

        if(transform.position.y < despawnLimit)
        {
            Destroy(gameObject);
        }
        
        AddPointsIfActive();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position; //Moves enemy away from player's current position.

            enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);

            Debug.Log("Enemy Boss collided with: " + collision.gameObject.name + "!");
        }
    }

    public void AddPointsIfActive()
    {
        if(spawnManager.isGameActive)
        {
            if(transform.position.y < despawnLimit)
            {
                spawnManager.UpdateScore(pointValue);
            }   
        }
    }
}