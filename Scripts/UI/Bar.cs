using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Bar : MonoBehaviour
{
    public Slider slider;
    public Image fill;
    public Gradient gradient;

    public void SetMaxValue(int value)
    {
        slider.maxValue = value;
        slider.value = value;  
    }

    public void SetValue(int value)
    {
        slider.value = value;
    }
}
