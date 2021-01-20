using UnityEngine;
using UnityEngine.UI;

public class ShowVolumePercentage : MonoBehaviour
{
    public Text percentageText;

    void Awake()
    {
        percentageText = GetComponent<Text>();
    }

    public void VolumePercentage(float sliderValue)
    {
        percentageText.text = Mathf.RoundToInt(sliderValue * 100) + "%";
    }
}
