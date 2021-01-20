using UnityEngine;
using UnityEngine.UI;

public class DifficultySelector : MonoBehaviour
{

    public int difficulty;
    public float nukeSpawnChance = 0.9f;
    private SpawnManager spawnManager;
    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        
        button = GetComponent<Button>();
        button.onClick.AddListener(SetDifficulty);
    }

    void SetDifficulty()
    {
        spawnManager.StartGame(difficulty);
        spawnManager.NukeSpawn(difficulty);
        spawnManager.BossSpawn(difficulty);
        
        Debug.Log(button.gameObject.name + " was clicked");
    }
}