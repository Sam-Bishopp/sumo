using UnityEngine;
using UnityEngine.UI;

public class SoundtrackSelector : MonoBehaviour
{
    private AudioManager audioManager;

    public int soundtrackSelection;
    [SerializeField] Button button;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();

        button = GetComponent<Button>();
        button.onClick.AddListener(SetSoundtrack);
    }

    private void SetSoundtrack() //Sets the soundtrack to be played based on the int assigned to the soundtrack buttons
    {
        audioManager.SaveBGM(soundtrackSelection);
        Debug.Log(button.gameObject.name + " was clicked");
    }
}