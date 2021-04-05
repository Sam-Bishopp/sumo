using UnityEngine;
using UnityEngine.UI;

public class DifficultySelector : MonoBehaviour
{
    public int difficulty;
    public float nukeSpawnChance = 0.9f;
    private SpawnManager spawnManager;
    [SerializeField] Button button;

    // Start is called before the first frame update
    void Start()
    {
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        
        button = GetComponent<Button>();
        button.onClick.AddListener(SetDifficulty);
    }

    void SetDifficulty() //Parse the difficulty value to modify the game, based on the selected difficulty.
    {
        spawnManager.StartGame(difficulty);
        spawnManager.DifficultyModifier(difficulty);
        
        Debug.Log(button.gameObject.name + " was clicked");
    }
}