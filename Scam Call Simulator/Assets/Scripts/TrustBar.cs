using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrustBar : MonoBehaviour
{
    public Slider slider;       // drag the Slider UI object here
    public Text trustLabel;     // drag a Text object here (shows "50")

    private float trustValue = 50f;
    public float TrustValue { get { return trustValue; } }

    public void SetTrust(float value)
    {
        trustValue = Mathf.Clamp(value, 0f, 100f);
        slider.value = trustValue / 100f;
        trustLabel.text = Mathf.RoundToInt(trustValue).ToString();
    }

    public void ModifyTrust(float delta)
    {
        SetTrust(trustValue + delta);
    }
}