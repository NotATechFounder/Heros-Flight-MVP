using System;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.Common.Progression;
using UnityEngine;

public class HeroProgression : MonoBehaviour
{
    public event Action<int> OnSpChanged;
    public event Action<int, int, float> OnEXPAdded;

    [SerializeField] private int spPerLevel;
    [SerializeField] private float expToNextLevelBase;
    [SerializeField] private float expToNextLevelMultiplier;
    [SerializeField] private HPAttributeSO[] HPAttributeSOs;
    [SerializeField] private HeroProgressionAttributeInfo[] heroProgressionAttributeInfos;

    private Dictionary<HeroProgressionAttribute, int> hPAttributeSpModifiedDic;

    [Header("Debug")]
    [SerializeField] private CharacterStatController characterStatController;
    [SerializeField] private int avaliableSp;
    [SerializeField] private int totalUsedSp;
    [SerializeField] private int currentUsedSp;
    [SerializeField] private int currentLevel;
    [SerializeField] private float currentExp;
    [SerializeField] private float expToNextLevel;

    public HeroProgressionAttributeInfo[]  HeroProgressionAttributeInfos => heroProgressionAttributeInfos;

    public void Initialise(CharacterStatController characterStatController)
    {
        this.characterStatController = characterStatController;
        hPAttributeSpModifiedDic = new Dictionary<HeroProgressionAttribute, int>();
        expToNextLevel = expToNextLevelBase * Mathf.Pow(expToNextLevelMultiplier, currentLevel);
        SetUpHeroProgressionAttributeInfo();
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

    public void ProccessAttributes()
    {
        foreach (KeyValuePair<HeroProgressionAttribute, int> attribute in hPAttributeSpModifiedDic)
        {
            switch (attribute.Key)
            {
                case HeroProgressionAttribute.Power:
                    ProccessPower(GetAttributeInfo(HeroProgressionAttribute.Power), attribute.Value);
                    break;
                    case HeroProgressionAttribute.Vitality:
                    ProccessVitality(GetAttributeInfo(HeroProgressionAttribute.Vitality));
                    break;
                    case HeroProgressionAttribute.Agility:
                    ProccessAgility(GetAttributeInfo(HeroProgressionAttribute.Agility));
                    break;
                    case HeroProgressionAttribute.Defense:
                    ProccessDefense(GetAttributeInfo(HeroProgressionAttribute.Defense));
                    break;
                    case HeroProgressionAttribute.CriticalHit:
                    ProccessCriticalHit(GetAttributeInfo(HeroProgressionAttribute.CriticalHit));
                    break;
            }
        }
    }

    public void ResetAttributes()
    {
        foreach (var attributeInfo in heroProgressionAttributeInfos)
        {
            if (hPAttributeSpModifiedDic.ContainsKey(attributeInfo.AttributeSO.Attribute))
            {
                attributeInfo.ReduceSP(hPAttributeSpModifiedDic[attributeInfo.AttributeSO.Attribute]);
            }

            switch (attributeInfo.AttributeSO.Attribute)
            {
                case HeroProgressionAttribute.Power:

                    characterStatController.ModifyPhysicalDamage(attributeInfo.GetTotalValue("PhysicalOutput"), false);
                    characterStatController.ModifyMagicDamage(attributeInfo.GetTotalValue("MagicalOutput"), false);

                    break;
                case HeroProgressionAttribute.Vitality:

                    characterStatController.ModifyMaxHealth(attributeInfo.GetTotalValue("VitalityOutput"), false);

                    break;
                case HeroProgressionAttribute.Agility:

                    characterStatController.ModifyMoveSpeed(attributeInfo.GetTotalValue("FlySpeedOutput"), false);
                    characterStatController.ModifyAttackSpeed(attributeInfo.GetTotalValue("AttackSpeedOutput"), false);
                    characterStatController.ModifyDodgeChance(attributeInfo.GetTotalValue("DodgeOutput"), false, false);

                    break;
                case HeroProgressionAttribute.Defense:
                    characterStatController.ModifyDefense(attributeInfo.GetTotalValue("DefenseOutput"), false, false);
                    break;
                case HeroProgressionAttribute.CriticalHit:

                    characterStatController.ModifyCriticalHitChance(attributeInfo.GetTotalValue("CriticalHitOutput"),false, false);

                    break;
            }
        }
    }

    public HeroProgressionAttributeInfo GetAttributeInfo(HeroProgressionAttribute attribute)
    {
        foreach (var attributeInfo in heroProgressionAttributeInfos)
        {
            if (attributeInfo.AttributeSO.Attribute == attribute)
            {
                return attributeInfo;
            }
        }
        return null;
    }

    private void ProccessPower(HeroProgressionAttributeInfo info, int newValue)
    {
        characterStatController.ModifyPhysicalDamage(info.GetKeyValue("PhysicalOutput", newValue), true);
        characterStatController.ModifyMagicDamage(info.GetKeyValue("MagicalOutput", newValue), true);
    }

    private void ProccessVitality(HeroProgressionAttributeInfo info)
    {
        characterStatController.ModifyMaxHealth(info.GetTotalValue("VitalityOutput"), true);
    }

    private void ProccessAgility(HeroProgressionAttributeInfo info)
    {
        characterStatController.ModifyMoveSpeed(info.GetTotalValue("FlySpeedOutput"), true);
        characterStatController.ModifyAttackSpeed(info.GetTotalValue("AttackSpeedOutput"), true);
        characterStatController.ModifyDodgeChance(info.GetTotalValue("DodgeOutput"), true,false);
    }

    private void ProccessDefense(HeroProgressionAttributeInfo info)
    {
        characterStatController.ModifyDefense(info.GetTotalValue("DefenseOutput"), true,false);
    }

    private void ProccessCriticalHit(HeroProgressionAttributeInfo info)
    {
        characterStatController.ModifyCriticalHitChance(info.GetTotalValue("CriticalHitOutput"), true, false);
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
            if (hPAttributeSpModifiedDic.ContainsKey(attributeInfo.AttributeSO.Attribute))
            {
                hPAttributeSpModifiedDic[attributeInfo.AttributeSO.Attribute]--;

                if (hPAttributeSpModifiedDic[attributeInfo.AttributeSO.Attribute] == 0)
                {
                    attributeInfo.TriggerModified(false);
                    hPAttributeSpModifiedDic.Remove(attributeInfo.AttributeSO.Attribute);
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
        ProccessAttributes();
        hPAttributeSpModifiedDic.Clear();
        totalUsedSp += currentUsedSp;
    }

    public void ResetSP()
    {
        if (totalUsedSp == 0)
        {
            return;
        }

        ResetAttributes();

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
            OnEXPAdded?.Invoke(0,0,currentExp / expToNextLevel);
        }
    }

    private void LevelUpOnce()
    {
        avaliableSp += spPerLevel;
        currentUsedSp = avaliableSp;
        currentLevel++;
        currentExp -= expToNextLevel;
        expToNextLevel = expToNextLevelBase * Mathf.Pow(expToNextLevelMultiplier, currentLevel);
        OnSpChanged?.Invoke(avaliableSp);
    }

    private void LevelUp()
    {
        int currentLvl = currentLevel;
        int numberOfLevelsGained = 0;
        do
        {
            LevelUpOnce();
            ++numberOfLevelsGained;
        } while (currentExp >= expToNextLevel);

        OnEXPAdded?.Invoke(currentLvl, numberOfLevelsGained, currentExp / expToNextLevel);
    }
}

[Serializable]
public class HeroProgressionAttributeInfo
{
    public Action<bool> OnModified;
    public Action<int> OnSPChanged;

    [SerializeField] HPAttributeSO attributeSO;
    [SerializeField] private int currentSP;

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
    }

    public void DecrementSP()
    {
        --currentSP;
        OnSPChanged?.Invoke(currentSP);
    }

    public void TriggerModified(bool modified)
    {
        OnModified?.Invoke(modified);
    }

    public void ResetSP()
    {
        currentSP = 0;
        OnSPChanged?.Invoke(currentSP);
        OnModified?.Invoke(false);
    }

    public void ReduceSP(int sp)
    {
        currentSP -= sp;
    }

    public float GetTotalValue(string key)
    {
        foreach (var keyValue in attributeSO.KeyValues)
        {
            if (keyValue.key == key)
            {
                return (currentSP / keyValue.pointThreshold) * keyValue.Value;
            }
        }
        return 0;
    }

    public float GetKeyValue(string key, int newValue)
    {
        int difference = 0;

        if (currentSP == newValue)
        {
            difference = newValue;
        }
        else
        {
            difference = Mathf.Abs(newValue - currentSP);
        }

        foreach (var keyValue in attributeSO.KeyValues)
        {
            if (keyValue.key == key)
            {
                return (difference / keyValue.pointThreshold) * keyValue.Value;
            }
        }
        return 0;
    }
}



[System.Serializable]
public class HeroProgressionAttributeKeyValue
{
    public string key;
    public int pointThreshold;
    public float Value;

    public float GetValue()
    {
        return Value;
    }
}