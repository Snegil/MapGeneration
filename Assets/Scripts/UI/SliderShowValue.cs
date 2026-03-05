using System;
using TMPro;
using UnityEngine;

public class SliderShowValue : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    public void OnChange(float value)
    {
        text.text = Math.Round(value, 2).ToString();
    }
}
