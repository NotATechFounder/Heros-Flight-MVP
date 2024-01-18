using System;
using UnityEngine;

public enum CurveType
{
    Custom,
    Linear,
    EaseInOut,
    CustomKeys
}

[Serializable]
public class CustomCurveKeys
{
    public CustomCurveKey[] keys;
}

[Serializable]
public class CustomCurveKey
{
    public float time;
    public float value;
}

[Serializable]
public class CustomAnimationCurve
{
    public AnimationCurve animationCurve;

    [Header("Properties")]
    public CurveType curveType;
    public float minLevel;
    public float minValue;
    public float maxLevel;
    public float maxValue;

    [Header("Debug")]
    public CustomCurveKeys customCurveKeys;

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
            case CurveType.CustomKeys:
            UpdateCurveFromCustomKeys();
            break;
        }
    }

    public void UpdateCustomKeys()
    {
        customCurveKeys = new CustomCurveKeys();
        customCurveKeys.keys = new CustomCurveKey[animationCurve.keys.Length];
        for (int i = 0; i < animationCurve.keys.Length; i++)
        {
            customCurveKeys.keys[i] = new CustomCurveKey();
            customCurveKeys.keys[i].time = animationCurve.keys[i].time;
            customCurveKeys.keys[i].value = animationCurve.keys[i].value;
        }      
    }

    public void UpdateCurveFromCustomKeys()
    {
        animationCurve.ClearKeys();
        for (int i = 0; i < customCurveKeys.keys.Length; i++)
        {
            animationCurve.AddKey(customCurveKeys.keys[i].time, customCurveKeys.keys[i].value);
        }

        for (int i = 0; i < animationCurve.keys.Length; i++)
        {
            if (i == 0 || i == animationCurve.keys.Length - 1) continue;
            animationCurve.SmoothTangents(i, 0);
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