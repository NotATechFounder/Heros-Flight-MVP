using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupImageFill : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    [SerializeField] private float _value = 0f;
    [SerializeField] private Image[] fills;

    private void OnValidate()
    {
        UpdateValue();
    }

    private void UpdateValue()
    {
        int numFills = fills.Length;
        if (numFills <= 0)
        {
            Debug.LogError("No Image fills assigned to the GroupSlider!");
            return;
        }

        float fillWidth = 1f / numFills;
        for (int i = 0; i < numFills; i++)
        {
            fills[i].fillAmount = Mathf.Clamp01(_value - i * fillWidth) / fillWidth;
            fills[i].enabled = _value >= i * fillWidth;
        }
    }

    public void SetValue(float value)
    {
        _value = value;
        UpdateValue();
    }

    public void SetValue(float value, float maxValue)
    {
        _value = value / maxValue;
        UpdateValue();
    }

    public void ToggleVisbility(bool visible)
    {
        foreach (Image fill in fills)
        {
            fill.enabled = visible;
        }
    }
}
