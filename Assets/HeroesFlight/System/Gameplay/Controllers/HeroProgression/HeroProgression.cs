using Codice.CM.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroProgression : MonoBehaviour
{
    public event Action<int> OnSpChanged;
    public event Action<float> OnXpAdded;
    public event Action<int> OnLevelUp;

    [SerializeField] private int spPerLevel;
    [SerializeField] private float expToNextLevelBase;
    [SerializeField] private float expToNextLevelMultiplier;
    [SerializeField] private HPAttributeSO[] HPAttributeSOs;
    [SerializeField] private HeroProgressionAttributeInfo[] heroProgressionAttributeInfos;

    private Dictionary<HeroProgressionAttribute, int> hPAttributeSpModifiedDic;
    [Header("Debug")]
    [SerializeField] private int avaliableSp;
    [SerializeField] private int totalUsedSp;
    [SerializeField] private int currentUsedSp;
    [SerializeField] private int currentLevel;
    [SerializeField] private float currentExp;
    [SerializeField] private float expToNextLevel;

    public HeroProgressionAttributeInfo[]  HeroProgressionAttributeInfos => heroProgressionAttributeInfos;

    private void Start()
    {
        hPAttributeSpModifiedDic = new Dictionary<HeroProgressionAttribute, int>();

        expToNextLevel = expToNextLevelBase * Mathf.Pow(expToNextLevelMultiplier, currentLevel);

        SetUpHeroProgressionAttributeInfo();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddExp(100);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Confirm();
        }
    }

    public void SetUpHeroProgressionAttributeInfo()
    {
        heroProgressionAttributeInfos = new HeroProgressionAttributeInfo[HPAttributeSOs.Length];
        for (int i = 0; i < HPAttributeSOs.Length; i++)
        {
            HeroProgressionAttributeInfo heroProgressionAttributeInfo =  new HeroProgressionAttributeInfo();
            heroProgressionAttributeInfos[i] = heroProgressionAttributeInfo;
            heroProgressionAttributeInfos[i].Initialize(HPAttributeSOs[i]);
        }
    }

    public void SubscribeAttributeCallBacks()
    {
        foreach (var attribute in heroProgressionAttributeInfos)
        {
            switch (attribute.AttributeSO.Attribute)
            {
                case HeroProgressionAttribute.Power:
                    attribute.SetEffect(OnPowerSPChanged);
                    break;
                case HeroProgressionAttribute.Vitality:
                    attribute.SetEffect(OnVitalitySPChanged);
                    break;
                case HeroProgressionAttribute.Agility:
                    attribute.SetEffect(OnAgilitySPChanged);
                    break;
                case HeroProgressionAttribute.Defense:
                    attribute.SetEffect(OnDefenseSPChanged);
                    break;
                case HeroProgressionAttribute.HealthBoost:
                    attribute.SetEffect(OnHealthBoostSPChanged);
                    break;
                case HeroProgressionAttribute.CriticalHit:
                    attribute.SetEffect(OnCriticalHitSPChanged);
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

    }

    private void OnAgilitySPChanged(HeroProgressionAttributeInfo info)
    {

    }

    private void OnDefenseSPChanged(HeroProgressionAttributeInfo info)
    {

    }

    private void OnHealthBoostSPChanged(HeroProgressionAttributeInfo info)
    {

    }

    private void OnCriticalHitSPChanged(HeroProgressionAttributeInfo info)
    {
 
    }

    public void IncrementAttributeSP(HeroProgressionAttributeInfo attributeInfo)
    {
        if (CanGiveSP())
        {
            if (hPAttributeSpModifiedDic.ContainsKey(attributeInfo.AttributeSO.Attribute))
            {
                hPAttributeSpModifiedDic[attributeInfo.AttributeSO.Attribute]++;
            }
            else
            {
                hPAttributeSpModifiedDic.Add(attributeInfo.AttributeSO.Attribute, 1);
            }

            attributeInfo.TriggerModified(true);
            attributeInfo.IncrementSP();
            avaliableSp--;
            OnSpChanged?.Invoke(avaliableSp);
        }
    }

    public void DecrementAttributeSP(HeroProgressionAttributeInfo attributeInfo)
    {
        if (CanReturnSP())
        {
            if (hPAttributeSpModifiedDic.ContainsKey(attributeInfo.AttributeSO.Attribute)
               && hPAttributeSpModifiedDic[attributeInfo.AttributeSO.Attribute] > 0)
            {
                hPAttributeSpModifiedDic[attributeInfo.AttributeSO.Attribute]--;

                if (hPAttributeSpModifiedDic[attributeInfo.AttributeSO.Attribute] == 0)
                {
                    attributeInfo.TriggerModified(false);
                }

                attributeInfo.DecrementSP();
                avaliableSp++;
                OnSpChanged?.Invoke(avaliableSp);
            }  
        }
    }

    public bool CanGiveSP()
    {
        return avaliableSp + totalUsedSp > totalUsedSp;
    }

    public bool CanReturnSP()
    {
        return avaliableSp >= 0;
    }

    public void Confirm()
    {
        hPAttributeSpModifiedDic.Clear();
        totalUsedSp += currentUsedSp;
    }

    public void ResetSP()
    {
        if (totalUsedSp == 0)
        {
            return;
        }

        foreach (var attribute in heroProgressionAttributeInfos)
        {
            attribute.ResetSP();
        }

        hPAttributeSpModifiedDic.Clear();
        avaliableSp = totalUsedSp + spPerLevel;
        currentUsedSp = avaliableSp;
        totalUsedSp = 0;
        OnSpChanged?.Invoke(avaliableSp);
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
        avaliableSp = spPerLevel;
        currentUsedSp = avaliableSp;
        currentLevel++;
        currentExp -= expToNextLevel;
        expToNextLevel = expToNextLevelBase * Mathf.Pow(expToNextLevelMultiplier, currentLevel);
        OnLevelUp?.Invoke(currentLevel);
        OnSpChanged?.Invoke(avaliableSp);
    }
}

[Serializable]
public class HeroProgressionAttributeInfo
{
    public Action<bool> OnModified;
    public Action<int> OnSPChanged;

    [SerializeField] HPAttributeSO attributeSO;
    [SerializeField] private int currentSP;
    private Action<HeroProgressionAttributeInfo> OnChange;

    public HPAttributeSO AttributeSO => attributeSO;

    public int CurrentSP => currentSP;

    public void Initialize(HPAttributeSO hPAttributeSO)
    {
        attributeSO = hPAttributeSO;
        currentSP = 0;
    }

    public void IncrementSP()
    {
        ++currentSP;
        OnSPChanged?.Invoke(currentSP);
        OnChange?.Invoke(this);
    }

    public void DecrementSP()
    {
        --currentSP;
        OnSPChanged?.Invoke(currentSP);
        OnChange?.Invoke(this);
    }

    public void SetEffect(Action<HeroProgressionAttributeInfo> action)
    {
        OnChange = action;
    }

    public void TriggerModified(bool modified)
    {
        OnModified?.Invoke(modified);
    }

    internal void ResetSP()
    {
        currentSP = 0;
        OnSPChanged?.Invoke(currentSP);
        OnChange?.Invoke(this);
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