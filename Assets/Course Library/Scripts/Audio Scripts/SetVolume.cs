using UnityEngine;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour
{
    public AudioMixer bgmMixer;

    public void SetLevel(float sliderValue)
    {
        bgmMixer.SetFloat("BGMVol", Mathf.Log10 (sliderValue) * 20);
    }
}
