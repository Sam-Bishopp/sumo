using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour
{
    public AudioMixer bgmMixer;
    public Slider bgmSlider;

    void Start()
    {
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVol", 1f);
        bgmMixer.SetFloat("BGMVol", Mathf.Log10 (bgmSlider.value) * 20);
    }

    public void SetLevel(float sliderValue) //Set volume of BGM
    {
        PlayerPrefs.SetFloat("BGMVol", sliderValue);
        bgmMixer.SetFloat("BGMVol", Mathf.Log10 (sliderValue) * 20);
        PlayerPrefs.Save();
    }
}