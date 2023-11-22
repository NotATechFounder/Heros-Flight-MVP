using System;
using UnityEngine;

public enum CurveType
{
    Custom,
    Linear,
    EaseInOut,
}

[Serializable]
public class CustomAnimationCurve
{
    public AnimationCurve animationCurve;
    public CurveType curveType;
    public float minLevel;
    public float minValue;
    public float maxLevel;
    public float maxValue;

    public void UpdateCurveValues()
    {
        if (curveType == CurveType.Custom) return;

        if (animationCurve.keys.Length > 0)
        {
            minLevel = Mathf.RoundToInt(animationCurve.keys[0].time);
            minValue = Mathf.RoundToInt(animationCurve.keys[0].value);
            maxLevel = Mathf.RoundToInt(animationCurve.keys[animationCurve.keys.Length - 1].time);
            maxValue = Mathf.RoundToInt(animationCurve.keys[animationCurve.keys.Length - 1].value);
        }
    }

    public void UpdateCurve()
    {
        switch(curveType)
        {
            case CurveType.Custom:
                break;
            case CurveType.Linear:
                animationCurve = AnimationCurve.Linear(minLevel, minValue, maxLevel, maxValue);
                break;
            case CurveType.EaseInOut:
                animationCurve = AnimationCurve.EaseInOut(minLevel, minValue, maxLevel, maxValue);
                break;
        }
    }

    public int GetCurrentValueInt(int level) { return Mathf.RoundToInt(animationCurve.Evaluate(level)); }
    public float GetCurrentValueFloat(int level) { return animationCurve.Evaluate(level); }

    public int GetTotalValue(int level)
    {
        int totalAmount = 0;
        for (int i = 1; i < level; i++) totalAmount += Mathf.RoundToInt(animationCurve.Evaluate(i));
        return totalAmount;
    }
}
