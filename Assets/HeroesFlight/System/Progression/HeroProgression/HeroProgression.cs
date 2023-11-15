using System;
using System.Collections;
using System.Collections.Generic;
using Codice.CM.Common;
using HeroesFlight.Common.Progression;
using HeroesFlight.System.FileManager;
using HeroesFlight.System.Stats.Stats.Enum;
using UnityEngine;

public class HeroProgression : MonoBehaviour
{
    public event Action<int> OnSpChanged;

    [SerializeField] private int spPerLevel;
    [SerializeField] private float expToNextLevelBase;
    [SerializeField] private float expToNextLevelMultiplier;
    [SerializeField] private HPAttributeSO[] HPAttributeSOs;
    [SerializeField] private HeroProgressionAttributeInfo[] heroProgressionAttributeInfos;

    private Dictionary<HeroProgressionAttribute, int> trailAttributeModifiedDic;
    private Dictionary<HeroProgressionAttribute, int> hPAttributeSpModifiedDic;
    private StatModel statModel;

    [Header("Debug")]
    [SerializeField] private int avaliableSp;
    [SerializeField] private int totalUsedSp;
    [SerializeField] private int currentUsedSp;
    [SerializeField] private SkillPointData skillPointData;

    public HeroProgressionAttributeInfo[]  HeroProgressionAttributeInfos => heroProgressionAttributeInfos;

    public void Initialise()
    {
        hPAttributeSpModifiedDic = new Dictionary<HeroProgressionAttribute, int>();
        trailAttributeModifiedDic = new Dictionary<HeroProgressionAttribute, int>();
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

    public void ProccessStats(Dictionary<HeroProgressionAttribute, int> attributes)
    {
        foreach (KeyValuePair<HeroProgressionAttribute, int> attribute in attributes)
        {
            switch (attribute.Key)
            {
                case HeroProgressionAttribute.Power:
                    ProccessPower(GetAttributeInfo(HeroProgressionAttribute.Power), attribute.Value);
                    break;
                case HeroProgressionAttribute.Vitality:
                    ProccessVitality(GetAttributeInfo(HeroProgressionAttribute.Vitality), attribute.Value);
                    break;
                case HeroProgressionAttribute.Agility:
                    ProccessAgility(GetAttributeInfo(HeroProgressionAttribute.Agility), attribute.Value);
                    break;
                case HeroProgressionAttribute.Defense:
                    ProccessDefense(GetAttributeInfo(HeroProgressionAttribute.Defense), attribute.Value); ;
                    break;
                case HeroProgressionAttribute.CriticalHit:
                    ProccessCriticalHit(GetAttributeInfo(HeroProgressionAttribute.CriticalHit), attribute.Value);
                    break;
            }
        }
    }

    public void CombineStats()
    {
        Dictionary<HeroProgressionAttribute, int> combinedStat = new Dictionary<HeroProgressionAttribute, int>(trailAttributeModifiedDic);

        foreach (var item in trailAttributeModifiedDic)
        {

        }
    }

    private void ProccessPower(HeroProgressionAttributeInfo info, int newValue)
    {
        statModel.ModifyAttribute(StatType.PhysicalDamage, info.GetCurrentValue(StatType.PhysicalDamage, newValue), StatModel.StatModificationType.Addition, StatModel.StatCalculationType.Percentage);
        statModel.ModifyAttribute(StatType.MagicDamage, info.GetCurrentValue(StatType.MagicDamage, newValue), StatModel.StatModificationType.Addition, StatModel.StatCalculationType.Percentage);
    }

    private void ProccessVitality(HeroProgressionAttributeInfo info, int newValue)
    {
        statModel.ModifyAttribute(StatType.MaxHealth, info.GetCurrentValue(StatType.MaxHealth, newValue), StatModel.StatModificationType.Addition, StatModel.StatCalculationType.Percentage);
    }

    private void ProccessAgility(HeroProgressionAttributeInfo info, int newValue)
    {
        statModel.ModifyAttribute(StatType.MoveSpeed, info.GetCurrentValue(StatType.MoveSpeed, newValue), StatModel.StatModificationType.Addition, StatModel.StatCalculationType.Percentage);
        statModel.ModifyAttribute(StatType.AttackSpeed, info.GetCurrentValue(StatType.AttackSpeed, newValue), StatModel.StatModificationType.Addition, StatModel.StatCalculationType.Percentage);
        statModel.ModifyAttribute(StatType.DodgeChance, info.GetCurrentValue(StatType.DodgeChance, newValue), StatModel.StatModificationType.Addition, StatModel.StatCalculationType.Percentage);
    }

    private void ProccessDefense(HeroProgressionAttributeInfo info, int newValue)
    {
        statModel.ModifyAttribute(StatType.Defense, info.GetCurrentValue(StatType.Defense, newValue), StatModel.StatModificationType.Addition, StatModel.StatCalculationType.Percentage);
    }

    private void ProccessCriticalHit(HeroProgressionAttributeInfo info, int newValue)
    {
        statModel.ModifyAttribute(StatType.CriticalHitChance, info.GetCurrentValue(StatType.CriticalHitChance, newValue), StatModel.StatModificationType.Addition, StatModel.StatCalculationType.Percentage);
    }

    public void ProccessAttributes()
    {
        ProccessStats (trailAttributeModifiedDic);
        ProccessStats (hPAttributeSpModifiedDic);
    }

    public void ResetAttributes(Dictionary<HeroProgressionAttribute, int> attributes)
    {

        foreach (var item in attributes)
        {
            //switch (item.Key)
            //{
            //    case HeroProgressionAttribute.Power:

            //        statModel.ModifyAttribute(StatType.PhysicalDamage, attributeInfo.GetTotalValue(StatType.PhysicalDamage), StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);
            //        statModel.ModifyAttribute(StatType.MagicDamage, attributeInfo.GetTotalValue(StatType.MagicDamage), StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);

            //        break;
            //    case HeroProgressionAttribute.Vitality:

            //        statModel.ModifyAttribute(StatType.MaxHealth, attributeInfo.GetTotalValue(StatType.MaxHealth), StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);
            //        break;
            //    case HeroProgressionAttribute.Agility:

            //        statModel.ModifyAttribute(StatType.MoveSpeed, attributeInfo.GetTotalValue(StatType.MoveSpeed), StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);
            //        statModel.ModifyAttribute(StatType.AttackSpeed, attributeInfo.GetTotalValue(StatType.AttackSpeed), StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);
            //        statModel.ModifyAttribute(StatType.DodgeChance, attributeInfo.GetTotalValue(StatType.DodgeChance), StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);

            //        break;
            //    case HeroProgressionAttribute.Defense:
            //        statModel.ModifyAttribute(StatType.Defense, attributeInfo.GetTotalValue(StatType.Defense), StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);
            //        break;
            //    case HeroProgressionAttribute.CriticalHit:

            //        statModel.ModifyAttribute(StatType.CriticalHitChance, attributeInfo.GetTotalValue(StatType.CriticalHitChance), StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);

            //        break;
            //}
        }


        foreach (var attributeInfo in heroProgressionAttributeInfos)
        {
            switch (attributeInfo.AttributeSO.Attribute)
            {
                case HeroProgressionAttribute.Power:

                    statModel.ModifyAttribute(StatType.PhysicalDamage, attributeInfo.GetTotalValue(StatType.PhysicalDamage), StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);
                    statModel.ModifyAttribute(StatType.MagicDamage, attributeInfo.GetTotalValue(StatType.MagicDamage), StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);

                    break;
                case HeroProgressionAttribute.Vitality:

                    statModel.ModifyAttribute(StatType.MaxHealth, attributeInfo.GetTotalValue(StatType.MaxHealth), StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);
                    break;
                case HeroProgressionAttribute.Agility:

                    statModel.ModifyAttribute(StatType.MoveSpeed, attributeInfo.GetTotalValue(StatType.MoveSpeed), StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);
                    statModel.ModifyAttribute(StatType.AttackSpeed, attributeInfo.GetTotalValue(StatType.AttackSpeed), StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);
                    statModel.ModifyAttribute(StatType.DodgeChance, attributeInfo.GetTotalValue(StatType.DodgeChance), StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);

                    break;
                case HeroProgressionAttribute.Defense:
                    statModel.ModifyAttribute(StatType.Defense, attributeInfo.GetTotalValue(StatType.Defense), StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);
                    break;
                case HeroProgressionAttribute.CriticalHit:

                    statModel.ModifyAttribute(StatType.CriticalHitChance, attributeInfo.GetTotalValue(StatType.CriticalHitChance), StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);

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

       // ResetAttributes();

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

    public void AddTraitsStatsModifiers(Dictionary<HeroProgressionAttribute, int> modifiedStatsMap)
    {
        // remove the previous stats

        trailAttributeModifiedDic = new Dictionary<HeroProgressionAttribute, int>(modifiedStatsMap);

        ProccessAttributes();
    }

    public void Load()
    {
        SkillPointData savedSkillPointData = FileManager.Load<SkillPointData>("SkillPoint");
        skillPointData = savedSkillPointData != null ? savedSkillPointData : new SkillPointData();
    }

    public void Save()
    {
        FileManager.Save("SkillPoint", skillPointData);
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

    public void IncreaseSP(int sp)
    {
        currentSP += sp;
        OnSPChanged?.Invoke(currentSP);
    }

    public float GetTotalValue(StatType statType)
    {
        return GetCurrentValue (statType, currentSP);
    }

    public float GetCurrentValue(StatType statType,int sp)
    {
        foreach (var keyValue in attributeSO.KeyValues)
        {
            if (keyValue.statType == statType)
            {
                return keyValue.valuePerSp * sp;
            }
        }
        return 0;
    }
}

[Serializable]
public class HeroProgressionAttributeKeyValue
{
    public StatType statType;
    public float valuePerSp;

    public float GetValue()
    {
        return valuePerSp;
    }
}

[Serializable]
public class SkillPointData
{
    public int avaliableSp;
    public List<HeroProgressionSingleData> heroProgressionSingleDatas;

    public void SetXp(HeroProgressionAttribute heroProgressionAttribute, int sp)
    {
        foreach (var heroProgressionSingleData in heroProgressionSingleDatas)
        {
            if (heroProgressionSingleData.heroProgressionAttribute == heroProgressionAttribute)
            {
                heroProgressionSingleData.sp = sp;
                return;
            }
        }

        HeroProgressionSingleData heroProgressionSingleData1 = new HeroProgressionSingleData();
        heroProgressionSingleData1.heroProgressionAttribute = heroProgressionAttribute;
        heroProgressionSingleData1.sp = sp;
        heroProgressionSingleDatas.Add(heroProgressionSingleData1);
    }

    public int GetSp(HeroProgressionAttribute heroProgressionAttribute)
    {
        foreach (var heroProgressionSingleData in heroProgressionSingleDatas)
        {
            if (heroProgressionSingleData.heroProgressionAttribute == heroProgressionAttribute)
            {
                return heroProgressionSingleData.sp;
            }
        }

        return 0;
    }

    public void IncrementSp(HeroProgressionAttribute heroProgressionAttribute)
    {
        foreach (var heroProgressionSingleData in heroProgressionSingleDatas)
        {
            if (heroProgressionSingleData.heroProgressionAttribute == heroProgressionAttribute)
            {
                heroProgressionSingleData.sp++;
                return;
            }
        }

        HeroProgressionSingleData heroProgressionSingleData1 = new HeroProgressionSingleData();
        heroProgressionSingleData1.heroProgressionAttribute = heroProgressionAttribute;
        heroProgressionSingleData1.sp = 1;
        heroProgressionSingleDatas.Add(heroProgressionSingleData1);
    }

    public void DecrementSp(HeroProgressionAttribute heroProgressionAttribute)
    {
        foreach (var heroProgressionSingleData in heroProgressionSingleDatas)
        {
            if (heroProgressionSingleData.heroProgressionAttribute == heroProgressionAttribute)
            {
                heroProgressionSingleData.sp--;
                return;
            }
        }

        HeroProgressionSingleData heroProgressionSingleData1 = new HeroProgressionSingleData();
        heroProgressionSingleData1.heroProgressionAttribute = heroProgressionAttribute;
        heroProgressionSingleData1.sp = 0;
        heroProgressionSingleDatas.Add(heroProgressionSingleData1);
    }
}

[Serializable]
public class HeroProgressionSingleData
{
    public HeroProgressionAttribute heroProgressionAttribute;
    public int sp;
}

public enum StatType
{
    PhysicalDamage,
    MagicDamage,
    MaxHealth,
    MoveSpeed,
    AttackSpeed,
    DodgeChance,
    Defense,
    CriticalHitChance
}

public class StatModel
{
    public enum StatCalculationType
    {
        Flat,
        Percentage
    }

    public enum StatModificationType
    {
        Addition,
        Subtraction
    }

    Dictionary<StatType, float> baseStatDic;
    Dictionary<StatType, float> currentStatDic;

    public Dictionary<StatType, float> CurrentStatDic => currentStatDic;

    public StatModel(PlayerStatData playerStatData)
    {
        baseStatDic = new Dictionary<StatType, float>();
        baseStatDic.Add(StatType.PhysicalDamage, playerStatData.PhysicalDamage.max);
        baseStatDic.Add(StatType.MagicDamage, playerStatData.MagicDamage.max);
        baseStatDic.Add(StatType.MaxHealth, playerStatData.Health);
        baseStatDic.Add(StatType.MoveSpeed, playerStatData.MoveSpeed);
        baseStatDic.Add(StatType.AttackSpeed, playerStatData.AttackSpeed);
        baseStatDic.Add(StatType.DodgeChance, playerStatData.DodgeChance);
        baseStatDic.Add(StatType.Defense, playerStatData.Defense);
        baseStatDic.Add(StatType.CriticalHitChance, playerStatData.CriticalHitChance);
        currentStatDic  = new Dictionary<StatType, float>(baseStatDic);
    }

    public float GetStatValue(StatType statType)
    {
        return currentStatDic[statType];
    }

    public void ModifyAttribute(StatType StatType, float amount, StatModificationType statModificationType, StatCalculationType statCalculationType)
    {
        currentStatDic[StatType] = ModifyStat(baseStatDic[StatType], currentStatDic[StatType], amount, statModificationType, statCalculationType);
    }

    private float ModifyStat(float baseValue, float currentValue, float amount, StatModificationType statModificationType , StatCalculationType statCalculationType)
    {
        float value = currentValue;

        switch (statCalculationType)
        {
            case StatCalculationType.Flat:
                value += statModificationType == StatModificationType.Addition ? amount : -amount;
                break;
            case StatCalculationType.Percentage:
                value = StatCalc.ModifyValueByPercentage(baseValue, currentValue, amount, statModificationType == StatModificationType.Addition);
                break;
        }
        return value;
    }
}