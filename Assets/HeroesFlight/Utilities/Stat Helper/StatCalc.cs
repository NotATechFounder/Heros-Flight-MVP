using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatCalc 
{
    public static float ModifyValueByPercentage(float baseValue, float currentValue, float percentageAmount, bool increase)
    {
        float percentageValue = (percentageAmount / 100) * baseValue;
        return increase ? currentValue + percentageValue : currentValue - percentageValue;
    }

    public static float ModifyValue(float currentValue, float amount, bool increase)
    {
        return increase ? currentValue + amount : currentValue - amount;
    }
    public static float GetPercentage(float baseValue, float percentageAmount)
    {
        return (percentageAmount / 100) * baseValue;
    }
    
    public static float GetValueOfPercentage(float baseValue, float percent)
    {
        return (baseValue / 100) * percent;
    }
}


[System.Serializable]
public class KeyValue
{
    public string key;
    public float Value;

    public float GetValue()
    {
        return Value;
    }
}