using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource BGM; //Reference to audio source on camera

    public AudioClip ingameMusic;

    //Main Menu Music
    public AudioClip nierMM1;
    public AudioClip nierMM2;
    
    //Music during gameplay
    public AudioClip nierGameplay1;
    public AudioClip nierGameplay2;
    public AudioClip nierGameplay3;
    public AudioClip nierGameplay4;

    //Music during game over screen
    public AudioClip psychoPassGO1;
    public AudioClip psychoPassGO2;
    public AudioClip nierGO1;
    public AudioClip nierGO2;
    public AudioClip fullmetalGO1;

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
}