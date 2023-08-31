using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat", menuName = "Stat Management/ New Stat")]
public class StatSO : ScriptableObject
{
    [SerializeField] private CustomAnimationCurve statCurve;
    public CustomAnimationCurve StatCurve => statCurve;

    private void OnValidate()
    {
        if (statCurve.curveType != CurveType.Custom)
        {
            statCurve.UpdateCurve();
        }
    }
}

[Serializable]
public class CustomAnimationCurve
{
    public AnimationCurve animationCurve;
    public CurveType curveType;
    public int minLevel;
    public int minValue;
    public int maxLevel;
    public int maxValue;

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

    public int GetCurrentValue(int level) { return Mathf.RoundToInt(animationCurve.Evaluate(level)); }

    public int GetTotalValue(int level)
    {
        int totalAmount = 0;
        for (int i = 1; i < level; i++) totalAmount += Mathf.RoundToInt(animationCurve.Evaluate(i));
        return totalAmount;
    }
}

public enum CurveType
{
    Custom,
    Linear,
    EaseInOut,   
}