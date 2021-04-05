using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private SpawnManager spawnManager;
    public float speed = 7.5f; //default speed of enemy
    private Rigidbody enemyRb;
    private GameObject player;
    private float despawnLimit = -20f;
    public int pointValue = 10;


    // Start is called before the first frame update. Initialise rigidbody and find player's location
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(spawnManager.isGameActive && !spawnManager.isGamePaused) //Track towards the player constantly.
        {
            // ".normalized" keeps speed the same across any distance
            Vector3 lookDirection = (player.transform.position - transform.position).normalized;
            enemyRb.AddForce(lookDirection * speed);
        }

        if(transform.position.y < despawnLimit)
        {
            Destroy(gameObject);
        }
        
        AddPointsIfActive();
    }

    public void AddPointsIfActive() //Add points for each enemy that falls off the level.
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