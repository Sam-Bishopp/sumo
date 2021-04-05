using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    //Script References
    private AudioManager audioManager;

    //Game Object references
    public GameObject enemyPrefab;
    public GameObject bossEnemyPrefab;
    public GameObject powerupPrefab;
    public GameObject nukePrefab;
    public GameObject titleScreen;
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject statisticsMenu;
    public GameObject highScoreResetMenu;
    public GameObject statsResetMenu;
    public GameObject highScoreConfirmMessage;
    public GameObject AllStatResetConfirmMessage;
    public GameObject changeBGMConfirmMessage;
    public GameObject nukeOverlay;
    public GameObject endGameScreen;
    public GameObject hud;

    //UI references for Gameplay
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI waveCounterText;
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] TextMeshProUGUI finalRoundText;
    [SerializeField] TextMeshProUGUI finalTimeText;
    [SerializeField] TextMeshProUGUI waveOverBonusText;
    [SerializeField] TextMeshProUGUI gameOverText;
    public TextMeshProUGUI nukeActiveText;
    public TextMeshProUGUI nukeDetonatedText;
    public TextMeshProUGUI outOfLevelReason;
    public TextMeshProUGUI playerTrappedReason;
    public TextMeshProUGUI playerEndedReason;
    public Text highScore;
    public Text highestRound;
    public Text powerupActiveText;
    private DateTime startTime;
    private TimeSpan gameLength;

    //Statistics UI
    public Text longestTimeStat;
    public Text highestRoundStat;
    public Text highestScoreStat;
    public Text fallenOffLevelStat;
    public Text samePlaceStat;
    public Text manualQuitStat;

    //Statistics Variables
    public int fallenOffLevelInt;
    public int samePlaceInt;
    public int manualQuitInt;

    //Enemy spawn & increment
    private float spawnRange = 9; //Axis range within playarea
    private int enemyCount;
    private int bossCount;

    //Wave numbers
    private int actualWaveNumber = 1;
    private int waveNumber = 1;
    private int lastWaveNumber;
    
    public int baseWaveOverBonus = 25;
    public int difficultyBonus;
    private int score;
    private float timeSurvived = 0;

    [SerializeField] int selectedDifficulty;
    [SerializeField] int bossSpawnDifficulty;
    public float nukeSpawnChance;
    public float bossSpawnChance;
    [SerializeField] bool waveOver;
    
    public bool isGamePaused = false;
    public bool isGameActive = false;
    public bool isNukeActive = false;

    void Awake() //Acquires script references and initialises UI text.
    {
        audioManager = FindObjectOfType<AudioManager>();

        manualQuitInt = PlayerPrefs.GetInt("ManualQuit");

        //End Game UI Statistics
        highScore.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
        highestRound.text = "Highest Round: " + PlayerPrefs.GetInt("HighestRound", 0).ToString();

        TimeSpan longestTime = TimeSpan.FromSeconds(PlayerPrefs.GetFloat("LongestTime", 0));

        //Statistics UI
        longestTimeStat.text = "Longest Time: " + longestTime.ToString("mm':'ss");
        highestRoundStat.text = "Highest Round: " + PlayerPrefs.GetInt("HighestRound", 0).ToString();
        highestScoreStat.text = "Highest Score: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
        fallenOffLevelStat.text = "Times Fallen off Level: " + PlayerPrefs.GetInt("FallenOffLevel", 0).ToString();
        samePlaceStat.text = "Times for Staying in the Same Place: " + PlayerPrefs.GetInt("SamePlace", 0).ToString();
        manualQuitStat.text = "Times Player Manually Quit the Game: " + PlayerPrefs.GetInt("ManualQuit", 0).ToString();
    }

    // Update is called once per frame
    void Update() //Checks the state of the game to decide when to spawn more enemies and initiate other functions and co-routines.
    {
        enemyCount = FindObjectsOfType<Enemy>().Length; //.Length returns number of enemies in scene. Need number because the variable is an "int".
        bossCount = FindObjectsOfType<EnemyBoss>().Length;

        if(Input.GetKeyDown(KeyCode.Escape) && isGameActive)
        {
            isGamePaused = !isGamePaused;
            PauseGame();
        }

        CoreGameplay();
        
        if(isGameActive)
        {
            TimeSurvived();
        }
    }

    public void CoreGameplay()
    {
        if(enemyCount == 0 && bossCount == 0 && isGameActive && !isNukeActive) //If no more enemies are in the scene, spawn enemies and powerups
        {
            waveOver = true;
            UpdateScore(difficultyBonus);

            lastWaveNumber = waveNumber;
            actualWaveNumber++;
            HighestRound();

            waveNumber++;
            waveNumber += selectedDifficulty;

            waveOverBonusText.text = "Wave Over Bonus: +" + difficultyBonus;
            waveOverBonusText.gameObject.SetActive(true);
            StartCoroutine(WaveOverBonus());

            scoreText.text = "Score: " + score;
            waveCounterText.text = "Round: " + actualWaveNumber;

            SpawnEnemyWave(waveNumber);
            Instantiate(powerupPrefab, GenerateSpawnPosition(), powerupPrefab.transform.rotation);
            
            if(UnityEngine.Random.value <= nukeSpawnChance)
            { 
                Instantiate(nukePrefab, GenerateSpawnPosition(), nukePrefab.transform.rotation); 
            }

            if(UnityEngine.Random.value <= bossSpawnChance)
            {
                IncreaseBossSpawn();
                Debug.Log("New boss spawn chance: " + bossSpawnChance);
                Instantiate(bossEnemyPrefab, GenerateSpawnPosition(), bossEnemyPrefab.transform.rotation);
            }
        }
    }

    public void StartGame(int difficulty) // Function to begin the game.
    {
        isGameActive = true;
        selectedDifficulty += difficulty; //Spawns more enemies depending on difficulty
        difficultyBonus = baseWaveOverBonus * difficulty;

        score = 0;
        scoreText.text = "Score: " + score;
        UpdateScore(0);

        //If the user doesn't choose a song, play the default song. Otherwise, play their choice.
        if(audioManager.ingameMusic == null)
        { 
            audioManager.ChangeBGM(audioManager.nierGameplay1); 
        }
        else
        {
            audioManager.ChangeBGM(audioManager.ingameMusic);
        }

        titleScreen.gameObject.SetActive(false);
        hud.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);

        Instantiate(powerupPrefab, GenerateSpawnPosition(), powerupPrefab.transform.rotation);
        SpawnEnemyWave(waveNumber + difficulty);                          
    }

    public void DifficultyModifier(int difficulty)
    {
        bossSpawnDifficulty = difficulty;

        switch(difficulty)
        {
            case 1: //Easy Difficulty
                nukeSpawnChance = 0.4f;
                bossSpawnChance = 0.1f;
                Debug.Log("Nuke spawn chance = " + nukeSpawnChance * 100 + "%");
                Debug.Log("Boss spawn chance = " + bossSpawnChance * 100 + "%");
                break;
            
            case 2: //Normal Difficulty
                nukeSpawnChance = 0.25f;
                bossSpawnChance = 0.2f;
                Debug.Log("Nuke spawn chance = " + nukeSpawnChance * 100 + "%");
                Debug.Log("Boss spawn chance = " + bossSpawnChance * 100 + "%");
                break;
            
            case 3: //Hard Difficulty
                nukeSpawnChance = 0.2f;
                bossSpawnChance = 0.3f;
                Debug.Log("Nuke spawn chance = " + nukeSpawnChance * 100 + "%");
                Debug.Log("Boss spawn chance = " + bossSpawnChance * 100 + "%");
                break;
            
            case 4: //Insane Difficulty
                nukeSpawnChance = 0.1f;
                bossSpawnChance = 0.4f;
                Debug.Log("Nuke spawn chance = " + nukeSpawnChance * 100 + "%");
                Debug.Log("Boss spawn chance = " + bossSpawnChance * 100 + "%");
                break;
            
            default:
                nukeSpawnChance = 0.25f;
                Debug.Log("A difficulty was not selected somehow. Nuke spawn chance set to " + nukeSpawnChance * 100 + "%");
                break;
        }
    }

    public void IncreaseBossSpawn() //Amount of bosses & frequency of boss spawns increases.
    {
        if(actualWaveNumber <= 9)
        {
            for(int i = 0; i < 1; i++)  // "i" begins at 0 | Loop will continue until it has completed 1 time | "i" will increase by 1 for each loop
            {
                Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation); 
            }
        }
        else if(actualWaveNumber >= 10)
        {
            for(int i = 0; i < 2; i++)
            {
                Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation); 
            }
        }

        switch(bossSpawnDifficulty)
        {
            case 1:
                bossSpawnChance += 0.005f;
                break;

            case 2:
                bossSpawnChance += 0.01f;
                break;

            case 3:
                bossSpawnChance += 0.015f;
                break;

            case 4:
                bossSpawnChance += 0.02f;
                break;

            default:
                bossSpawnChance += 0.01f;
                Debug.Log("The chosen difficulty could not be found. Defaulting to add +0.01 to boss spawn chance.");
                break;
        }
    }

    public void UpdateScore(int scoreToAdd) //Updates score as the player plays the game. Also stores a new high score if the previous high score has been beaten.
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;

        if(score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
            highScore.text = "High Score: " + score;
        }
    }

    public void HighestRound() //The highest round the player has achieved
    {
        if(actualWaveNumber > PlayerPrefs.GetInt("HighestRound", 0))
        {
            PlayerPrefs.SetInt("HighestRound", actualWaveNumber);
            highestRound.text = "Highest Round: " + actualWaveNumber;
        }
    }

    public void TimeSurvived() //How long the player has survived
    {
        timeSurvived += Time.deltaTime;

        float minutes = Mathf.FloorToInt(timeSurvived / 60);
        float seconds = Mathf.FloorToInt(timeSurvived % 60);

        string formattedTime = string.Format("Time Survived: {0:00}:{1:00}", minutes, seconds);

        finalTimeText.text = formattedTime;

        if(timeSurvived > PlayerPrefs.GetFloat("LongestTime", 0))
        {
            PlayerPrefs.SetFloat("LongestTime", timeSurvived);
        }
    }

    public void ResetScoreAndRound() //Function to reset the high score, highest round & longest time stored.
    {
        PlayerPrefs.DeleteKey("HighScore");
        PlayerPrefs.DeleteKey("HighestRound");
        PlayerPrefs.DeleteKey("LongestTime");

        highScoreResetMenu.gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(true);

        highScoreConfirmMessage.gameObject.SetActive(true);
        StartCoroutine(ResetScoreAndRoundConfirm());
    }

    public void ResetAllStats() //Resets all statistics saved in PlayerPrefs
    {
        PlayerPrefs.DeleteAll();

        statsResetMenu.gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(true);

        AllStatResetConfirmMessage.gameObject.SetActive(true);
        StartCoroutine(ResetAllStatsConfirm());

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    IEnumerator ResetScoreAndRoundConfirm() //Confirmation message of score reset.
    {
        yield return new WaitForSeconds(1.5f);
        highScoreConfirmMessage.gameObject.SetActive(false);
    }

    IEnumerator ResetAllStatsConfirm() //Confirmation message of all stat reset.
    {
        yield return new WaitForSeconds(1.5f);
        AllStatResetConfirmMessage.gameObject.SetActive(false);
    }

    public void ChangeBGMNotification()
    {
        changeBGMConfirmMessage.gameObject.SetActive(true);
        StartCoroutine(ChangeBGMConfirm());
    }

    IEnumerator ChangeBGMConfirm()
    {
        yield return new WaitForSeconds(3f);
        changeBGMConfirmMessage.gameObject.SetActive(false);
    }

    IEnumerator WaveOverBonus() //Visual indicator to notify player of bonus score
    {
        yield return new WaitForSeconds(4f);
        waveOverBonusText.gameObject.SetActive(false);
    }

    public void PauseGame() //Time is stopped in game, allowing the player to pause
    {
        isGamePaused = !isGamePaused;

        if(isGamePaused)
        {
            Time.timeScale = 0f;
            
            hud.gameObject.SetActive(false);
            pauseMenu.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;

            pauseMenu.gameObject.SetActive(false);
            hud.gameObject.SetActive(true);
        }
    }

    public void GameOver() //Triggers the game over sequence
    {
        isGameActive = false;
        Time.timeScale = 1;

        scoreText.gameObject.SetActive(false);
        hud.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);

        if(audioManager.psychoPassGO1 != null)
        { 
            audioManager.ChangeBGM(audioManager.psychoPassGO1); 
        }
        
        finalScoreText.text = "Your Score was: " + score;
        finalRoundText.text = "You reached Round: " + actualWaveNumber;
        endGameScreen.gameObject.SetActive(true);
    }

    public void GameOverByMenu() //User ends the game by clicking the end game button
    {
        playerEndedReason.gameObject.SetActive(true);

        manualQuitInt++;
        PlayerPrefs.SetInt("ManualQuit", manualQuitInt);
        Debug.Log(manualQuitInt);
        manualQuitStat.text = "Times Fallen off Level: " + manualQuitInt;
    }

    public void OptionsMenu() //Options menu appears
    {
        titleScreen.gameObject.SetActive(false);
        highScoreResetMenu.gameObject.SetActive(false);
        statsResetMenu.gameObject.SetActive(false);
        
        optionsMenu.gameObject.SetActive(true);
    }

    public void StatisticsMenu() //Statistics menu appears
    {
        titleScreen.gameObject.SetActive(false);
        highScoreResetMenu.gameObject.SetActive(false);
        
        statisticsMenu.gameObject.SetActive(true);
    }

    public void ResetScoreAndRoundMenu() //Score and round reset UI appears
    {
        optionsMenu.gameObject.SetActive(false);
        highScoreResetMenu.gameObject.SetActive(true);
    }

    public void ResetAllStatsMenu()
    {
        optionsMenu.gameObject.SetActive(false);
        statsResetMenu.gameObject.SetActive(true);
    }

    public void MainMenu() //Reloads the scene back to the main menu (used for when the game has been completed)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;

        scoreText.gameObject.SetActive(false);
        hud.gameObject.SetActive(false);
        endGameScreen.gameObject.SetActive(false);
        titleScreen.gameObject.SetActive(true);
    }

    public void MainMenuBeforeGame() //Goes to main menu without reloading the scene
    {
        optionsMenu.gameObject.SetActive(false);
        statisticsMenu.gameObject.SetActive(false);
        titleScreen.gameObject.SetActive(true);
    }

    public void ExitGame() 
    {
        Application.Quit();
        Debug.Log("Game is closing");
    }

    void SpawnEnemyWave(int enemiesToSpawn) //Method to spawn enemies
    {
        if(isGameActive)
        {
            for(int i = 0; i < enemiesToSpawn; i++)  // "i" begins at 0 | Loop will continue until it has completed 3 times | "i" will increase by 1 for each loop
            {
                Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation); 
            }
        }
    }

    //Method for generating random positions for spawning enemies and powerups.
    private Vector3 GenerateSpawnPosition() 
    {
        float spawnPosX = UnityEngine.Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = UnityEngine.Random.Range(-spawnRange, spawnRange);
        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);
        
        return randomPos;
    }
}