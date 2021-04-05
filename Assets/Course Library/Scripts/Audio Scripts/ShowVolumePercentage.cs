using UnityEngine;
using UnityEngine.UI;

public class ShowVolumePercentage : MonoBehaviour
{
    public Text percentageText;

    void Awake()
    {
        percentageText = GetComponent<Text>(); //Reference to volume percentage text
    }

    public void VolumePercentage(float sliderValue) //Raise and lower the percentage value according to the slider's position
    {
        percentageText.text = Mathf.RoundToInt(sliderValue * 100) + "%";
    }
}
