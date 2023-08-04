using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatCalc 
{
    public static float ModifyValueByPercentage(float baseValue, float currentValue, float percentageAmount, bool increase)
    {
        float percentageValue = ((float)percentageAmount / 100) * baseValue;
        return increase ? currentValue + percentageValue : currentValue - percentageValue;
    }

    public static float ModifyValue(float currentValue, float amount, bool increase)
    {
        return increase ? currentValue + amount : currentValue - amount;
    }
}
