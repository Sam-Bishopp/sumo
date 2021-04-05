using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource BGM; //Reference to audio source on camera
    
    public AudioClip ingameMusic; //Player's music choice

    //Main Menu Music
    public AudioClip nierMM1;
    
    //Music during gameplay
    public AudioClip nierGameplay1;
    public AudioClip nierGameplay2;
    public AudioClip nierGameplay3;
    public AudioClip nierGameplay4;

    //Music during game over screen
    public AudioClip psychoPassGO1;

    public int soundtrackPreference;

    public void Start()
    {
        soundtrackPreference = PlayerPrefs.GetInt("SoundtrackPref", 0);
        Debug.Log("Soundtrack: " + soundtrackPreference);

        SaveBGM(soundtrackPreference);
    }

    //Prepares the player's music choice to be played once the game begins.
    public void PrepareBGM(AudioClip prepMusic)
    {
        ingameMusic = prepMusic;
    }

    //Method to change BGM when called. Parses a new audio clip to be played.
    public void ChangeBGM(AudioClip music)
    {
        if(BGM.clip.name == music.name) 
        { return; }

        BGM.Stop();
        BGM.clip = music;
        BGM.Play();
    }

    //Finds the player's soundtrack preference based on the value of the "SoundtrackPref" player preference (Their last choice of soundtrack). 
    public void SaveBGM(int soundtrack)
    {
        switch(soundtrack)
        {
            case 1: //Soundtrack 1
            PlayerPrefs.SetInt("SoundtrackPref", soundtrack);
            PrepareBGM(nierGameplay1);
            break;

            case 2: //Soundtrack 2
            PlayerPrefs.SetInt("SoundtrackPref", soundtrack);
            PrepareBGM(nierGameplay2);
            break;

            case 3: //Soundtrack 3
            PlayerPrefs.SetInt("SoundtrackPref", soundtrack);
            PrepareBGM(nierGameplay3);
            break;

            case 4: //Soundtrack 4
            PlayerPrefs.SetInt("SoundtrackPref", soundtrack);
            PrepareBGM(nierGameplay4);
            break;

            default: //Default Selection
            Debug.Log("Setting the soundtrack from SpawnManager...");
            break;
        }
    }
}