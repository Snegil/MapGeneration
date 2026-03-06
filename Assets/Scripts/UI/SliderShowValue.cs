using System;
using TMPro;
using UnityEngine;

public class SliderShowValue : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    bool showAsPercentage = false;

    public void OnChange(float value)
    {
        if (!showAsPercentage)
        {
            text.text = Math.Round(value, 2).ToString();
            return;
        }
        text.text = Math.Round(value, 2) * 100 + "%";
    }
}
