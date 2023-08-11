using Codice.CM.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroProgression : MonoBehaviour
{
    public event Action<float> OnXpAdded;

    public event Action<int> OnLevelUp;

    [SerializeField] private float expToNextLevelBase;
    [SerializeField] private float expToNextLevelMultiplier;
    [SerializeField] private HeroProgressionAttributeInfo[] HeroProgressionAttributeInfo;

    private int currentLevel;
    private float currentExp;
    private float expToNextLevel;

    private void Start()
    {
        expToNextLevel = expToNextLevelBase * Mathf.Pow(expToNextLevelMultiplier, currentLevel);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddExp(100);
        }
    }

    public void SubscribeAttributeCallBacks()
    {
        foreach (var attribute in HeroProgressionAttributeInfo)
        {
            switch (attribute.AttributeSO.Attribute)
            {
                case HeroProgressionAttribute.Power:
                    attribute.SubscribeToSPChanged(OnPowerSPChanged);
                    break;
                case HeroProgressionAttribute.Vitality:
                    attribute.SubscribeToSPChanged(OnVitalitySPChanged);
                    break;
                case HeroProgressionAttribute.Agility:
                    attribute.SubscribeToSPChanged(OnAgilitySPChanged);
                    break;
                case HeroProgressionAttribute.Defense:
                    attribute.SubscribeToSPChanged(OnDefenseSPChanged);
                    break;
                case HeroProgressionAttribute.HealthBoost:
                    attribute.SubscribeToSPChanged(OnHealthBoostSPChanged);
                    break;
                case HeroProgressionAttribute.CriticalHit:
                    attribute.SubscribeToSPChanged(OnCriticalHitSPChanged);
                    break;
            }   
        }
    }

    private void OnPowerSPChanged(HeroProgressionAttributeInfo info)
    {
        float physicalDamageIncrease = info.AttributeSO.GetKeyValue("PhysicalOutput");
    }

    private void OnVitalitySPChanged(HeroProgressionAttributeInfo info)
    {
        throw new NotImplementedException();
    }

    private void OnAgilitySPChanged(HeroProgressionAttributeInfo info)
    {
        throw new NotImplementedException();
    }

    private void OnDefenseSPChanged(HeroProgressionAttributeInfo info)
    {
        throw new NotImplementedException();
    }

    private void OnHealthBoostSPChanged(HeroProgressionAttributeInfo info)
    {
        throw new NotImplementedException();
    }

    private void OnCriticalHitSPChanged(HeroProgressionAttributeInfo info)
    {
        throw new NotImplementedException();
    }

    public void AddExp(float exp)
    {
        currentExp += exp;
        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
        else
        {
            OnXpAdded?.Invoke(currentExp/ expToNextLevel);
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExp -= expToNextLevel;
        expToNextLevel = expToNextLevelBase * Mathf.Pow(expToNextLevelMultiplier, currentLevel);
        OnLevelUp?.Invoke(currentLevel);
    }

    public void UnsubscribeAttributeCallBacks()
    {
        foreach (var attribute in HeroProgressionAttributeInfo)
        {
            switch (attribute.AttributeSO.Attribute)
            {
                case HeroProgressionAttribute.Power:
                    attribute.UnsubscribeToSPChanged(OnPowerSPChanged);
                    break;
                case HeroProgressionAttribute.Vitality:
                    attribute.UnsubscribeToSPChanged(OnVitalitySPChanged);
                    break;
                case HeroProgressionAttribute.Agility:
                    attribute.UnsubscribeToSPChanged(OnAgilitySPChanged);
                    break;
                case HeroProgressionAttribute.Defense:
                    attribute.UnsubscribeToSPChanged(OnDefenseSPChanged);
                    break;
                case HeroProgressionAttribute.HealthBoost:
                    attribute.UnsubscribeToSPChanged(OnHealthBoostSPChanged);
                    break;
                case HeroProgressionAttribute.CriticalHit:
                    attribute.UnsubscribeToSPChanged(OnCriticalHitSPChanged);
                    break;
            }
        }
    }
}

[Serializable]
public class HeroProgressionAttributeInfo
{
    [SerializeField] HPAttributeSO attributeSO;
    [SerializeField] private int currentSP;
    private Action<HeroProgressionAttributeInfo> OnSPChanged;

    public HPAttributeSO AttributeSO => attributeSO;

    public float CurrentSP => currentSP;

    public void IncrementdSP()
    {
        ++currentSP;
        OnSPChanged?.Invoke(this);
    }

    public void DecrementSP()
    {
        --currentSP;
        OnSPChanged?.Invoke(this);
    }

    public void SubscribeToSPChanged(Action<HeroProgressionAttributeInfo> action)
    {
        OnSPChanged = action;
    }

    public void UnsubscribeToSPChanged(Action<HeroProgressionAttributeInfo> action)
    {
        OnSPChanged = action;
    }
}

public enum HeroProgressionAttribute
{
    Power,
    Vitality,
    Agility,
    Defense,
    HealthBoost,
    CriticalHit,
}

[System.Serializable]
public class AttributeKeyValue
{
    public string key;
    public int pointThreshold;
    public float Value;

    public float GetValue()
    {
        return Value;
    }
}