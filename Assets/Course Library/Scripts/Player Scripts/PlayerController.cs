using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Script references
    private SpawnManager spawnManager;

    //Component references
    [SerializeField] GameObject player;
    private Rigidbody playerRb;
    private GameObject focalPoint;
    public Joystick joystick;

    //Gameplay variables
    public float speed = 12.5f; //default speed of player
    private float despawnLimit = -20f;
    public bool hasPowerup = false; //Player does not start with a powerup
    public float powerupStrength = 10.0f;
    public float powerupTimer = 7.5f;
    public float currentTime;
    public GameObject powerupIndicator;
    [SerializeField] float timeLeft;
    [SerializeField] float timerReset = 7.5f;
    
    //End Game Trigger Variables (from no movement)
    [SerializeField] Vector3 tempRecordedPosition;
    [SerializeField] Vector3 getPlayerPosition;
    [SerializeField] float checkPlayerPositionTime; //This is equal to whatever endGameTimeReset is.
    [SerializeField] float checkPlayerPositionTimeReset = 3f;

    // Awake is called before the game starts. Best used to initialise variables and get references before the game begins.
    void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        tempRecordedPosition = Vector3.zero;

        spawnManager.fallenOffLevelInt = PlayerPrefs.GetInt("FallenOffLevel");
        spawnManager.samePlaceInt = PlayerPrefs.GetInt("SamePlace");

        timeLeft = timerReset;
        checkPlayerPositionTime = checkPlayerPositionTimeReset;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(spawnManager.isGameActive && !spawnManager.isGamePaused)
        {
            //Movement of player. Mobile Controls
            float forwardInput = joystick.Vertical;
            float sideInput = joystick.Horizontal;
            
            playerRb.AddForce(focalPoint.transform.forward * speed * forwardInput);
            playerRb.AddForce(focalPoint.transform.right * speed * sideInput);

            
            RepeatPositionCheckTimer();
        }
        
        //Keeps indicator attached to the player
        Vector3 indicatorOffset = new Vector3(0, -0.4f, 0);
        powerupIndicator.transform.position = transform.position + indicatorOffset; //Position of powerup indicator, relative to player

        if(transform.position.y < despawnLimit) //If player falls off the scene, end the game.
        {
            Destroy(player);
            Destroy(powerupIndicator);

            spawnManager.fallenOffLevelInt++;
            PlayerPrefs.SetInt("FallenOffLevel", spawnManager.fallenOffLevelInt);
            Debug.Log(spawnManager.fallenOffLevelInt);
            spawnManager.fallenOffLevelStat.text = "Times Fallen off Level: " + spawnManager.fallenOffLevelInt;

            spawnManager.GameOver();
            spawnManager.outOfLevelReason.gameObject.SetActive(true);
        }

        if(hasPowerup) //If player has a powerup, begin a timer for powerup duration
        {
            PowerupTimer();
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Powerup") && !hasPowerup) //If player collides with a powerup & a powerup is not already active, activate the powerup
        {
            hasPowerup = true;
            Destroy(other.gameObject);

            powerupIndicator.gameObject.SetActive(true); //Powerup indicator is visible when collided with powerup
            spawnManager.powerupActiveText.gameObject.SetActive(true);

            StartCoroutine(PowerupCountdownRoutine()); //Call co-routine to stop powerup after a period of time.
        }

        //If player collides with a nuke powerup & nuke powerup is not active, destroy all enemies and activate co-routines and UI elements.
        if(other.CompareTag("Nuke") && !spawnManager.isNukeActive)
        {
            spawnManager.isNukeActive = true;
            Destroy(other.gameObject);
            GameObject[] currentEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            Debug.Log("Collided with: " + other.gameObject.name);

            spawnManager.nukeActiveText.gameObject.SetActive(true);
            spawnManager.nukeDetonatedText.gameObject.SetActive(true);
            spawnManager.nukeOverlay.gameObject.SetActive(true);
            
            StartCoroutine(NukeNotificationRoutine());

            foreach(GameObject ce in currentEnemies) //"ce" stands for current enemies
            {
                spawnManager.UpdateScore(10);
                Destroy(ce);
            }
        }
    }

    IEnumerator PowerupCountdownRoutine() //Timer to expire powerup
    {
        yield return new WaitForSeconds(powerupTimer);
        hasPowerup = false; //Reset the powerup status to "false"

        spawnManager.powerupActiveText.gameObject.SetActive(false);
        powerupIndicator.gameObject.SetActive(false); //Powerup indicator becomes invisible after co-routine is finished.
    }

    IEnumerator NukeNotificationRoutine()
    {
        yield return new WaitForSeconds(3f);

        spawnManager.isNukeActive = false;
        spawnManager.nukeActiveText.gameObject.SetActive(false);
        spawnManager.nukeDetonatedText.gameObject.SetActive(false);
        spawnManager.nukeOverlay.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy") && hasPowerup)
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position; //Moves enemy away from player's current position.

            enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);

            Debug.Log("Collided with: " + collision.gameObject.name + " with powerup set to " + hasPowerup);
        }
    }

    public void PowerupTimer() //Timer for powerup
    {
        timeLeft -= Time.deltaTime;
        spawnManager.powerupActiveText.text = "Powerup Active: " + timeLeft.ToString("0.0");

        if(timeLeft <= 0)
        {
            timeLeft = timerReset;
        }
    }

    public void RepeatPositionCheck() //Triggers a game over event if the player has been in the same place for certain amount of time
    {
        getPlayerPosition = player.transform.position;
        Debug.Log("Current Position = " + getPlayerPosition);
        Debug.Log("Last Recorded Position = " + tempRecordedPosition);
        
        if(tempRecordedPosition == getPlayerPosition) //If player's last position is the same as their current position, end the game.
        {
            spawnManager.GameOver();
            spawnManager.playerTrappedReason.gameObject.SetActive(true);
            
            spawnManager.samePlaceInt++;
            PlayerPrefs.SetInt("SamePlace", spawnManager.samePlaceInt);
            Debug.Log(spawnManager.samePlaceInt);
            spawnManager.samePlaceStat.text = "Times Fallen off Level: " + spawnManager.samePlaceInt;
        }
        else //If it is not, update the player's last recorded position to their current position and reset the timer 
        {
            tempRecordedPosition = player.transform.position;
            checkPlayerPositionTime = checkPlayerPositionTimeReset;
        }
    }

    public void RepeatPositionCheckTimer()
    {
        checkPlayerPositionTime -= Time.deltaTime;

        if(checkPlayerPositionTime <= 0)
        {
            Debug.Log("Timer expired. Checking player's position...");
            RepeatPositionCheck();
        }
    }
}