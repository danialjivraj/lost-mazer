using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderWithPercent : MonoBehaviour
{
    public Slider slider;
    public Text  percentText;

    void OnEnable()
    {
        slider.onValueChanged.AddListener(UpdateText);
        UpdateText(slider.value);
    }

    void OnDisable()
    {
        slider.onValueChanged.RemoveListener(UpdateText);
    }

    void UpdateText(float value)
    {
        int pct = Mathf.RoundToInt(value * 100f);
        percentText.text = pct.ToString() + "%";
    }
}
