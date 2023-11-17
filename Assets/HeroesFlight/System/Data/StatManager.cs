using System;
using System.Collections.Generic;
using HeroesFlight.Common.Progression;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    [SerializeField] private StatPointSO[] statPointSO;
    private Dictionary<StatAttributeType, int> trailAttributeModifiedDic = new Dictionary<StatAttributeType, int>();
    private Dictionary<StatAttributeType, int> statPointsDic = new Dictionary<StatAttributeType, int>();
    private StatModel statModel;
    private Dictionary<StatType, StatPointInfo> statTypePerSp = new Dictionary<StatType, StatPointInfo>();

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        foreach (StatPointSO statPointSo in statPointSO)
        {
            foreach (StatPointInfo statPointInfo in statPointSo.KeyValues)
            {
                statTypePerSp.Add(statPointInfo.statType, statPointInfo);
            }

            statPointsDic.Add(statPointSo.StatAttributeType, 0);
        }
    }

    public void AddStats(Dictionary<StatAttributeType, int> attributes)
    {
        foreach (KeyValuePair<StatAttributeType, int> attribute in attributes)
        {
            switch (attribute.Key)
            {
                case StatAttributeType.Power:
                    statModel.ModifyAttribute(StatType.PhysicalDamage, attribute.Value * statTypePerSp[StatType.PhysicalDamage].valuePerSp, StatModel.StatModificationType.Addition, statTypePerSp[StatType.PhysicalDamage].statCalculationType);
                    statModel.ModifyAttribute(StatType.MagicDamage, attribute.Value * statTypePerSp[StatType.MagicDamage].valuePerSp, StatModel.StatModificationType.Addition, statTypePerSp[StatType.MagicDamage].statCalculationType);
                    break;
                case StatAttributeType.Vitality:
                    statModel.ModifyAttribute(StatType.MaxHealth, attribute.Value * statTypePerSp[StatType.MaxHealth].valuePerSp, StatModel.StatModificationType.Addition, statTypePerSp[StatType.MaxHealth].statCalculationType);
                    break;
                case StatAttributeType.Agility:
                    statModel.ModifyAttribute(StatType.MoveSpeed, attribute.Value * statTypePerSp[StatType.MoveSpeed].valuePerSp, StatModel.StatModificationType.Addition, statTypePerSp[StatType.MoveSpeed].statCalculationType);
                    statModel.ModifyAttribute(StatType.AttackSpeed, attribute.Value * statTypePerSp[StatType.AttackSpeed].valuePerSp, StatModel.StatModificationType.Addition, statTypePerSp[StatType.AttackSpeed].statCalculationType);
                    statModel.ModifyAttribute(StatType.DodgeChance, attribute.Value * statTypePerSp[StatType.DodgeChance].valuePerSp, StatModel.StatModificationType.Addition, statTypePerSp[StatType.DodgeChance].statCalculationType);
                    break;
                case StatAttributeType.Defense:
                    statModel.ModifyAttribute(StatType.Defense, attribute.Value * statTypePerSp[StatType.Defense].valuePerSp, StatModel.StatModificationType.Addition, statTypePerSp[StatType.Defense].statCalculationType);
                    break;
                case StatAttributeType.CriticalHit:
                    statModel.ModifyAttribute(StatType.CriticalHitChance, attribute.Value * statTypePerSp[StatType.CriticalHitChance].valuePerSp, StatModel.StatModificationType.Addition, statTypePerSp[StatType.CriticalHitChance].statCalculationType);
                    break;
            }
        }
    }

    public void RemoveStats(Dictionary<StatAttributeType, int> attributes)
    {
        foreach (KeyValuePair<StatAttributeType, int> attribute in attributes)
        {
            switch (attribute.Key)
            {
                case StatAttributeType.Power:
                    statModel.ModifyAttribute(StatType.PhysicalDamage, attribute.Value * statTypePerSp[StatType.PhysicalDamage].valuePerSp, StatModel.StatModificationType.Subtraction, statTypePerSp[StatType.PhysicalDamage].statCalculationType);
                    statModel.ModifyAttribute(StatType.MagicDamage, attribute.Value * statTypePerSp[StatType.MagicDamage].valuePerSp, StatModel.StatModificationType.Subtraction, statTypePerSp[StatType.MagicDamage].statCalculationType);
                    break;
                case StatAttributeType.Vitality:
                    statModel.ModifyAttribute(StatType.MaxHealth, attribute.Value * statTypePerSp[StatType.MaxHealth].valuePerSp, StatModel.StatModificationType.Subtraction, statTypePerSp[StatType.MaxHealth].statCalculationType);
                    break;
                case StatAttributeType.Agility:
                    statModel.ModifyAttribute(StatType.MoveSpeed, attribute.Value * statTypePerSp[StatType.MoveSpeed].valuePerSp, StatModel.StatModificationType.Subtraction, statTypePerSp[StatType.MoveSpeed].statCalculationType);
                    statModel.ModifyAttribute(StatType.AttackSpeed, attribute.Value * statTypePerSp[StatType.AttackSpeed].valuePerSp, StatModel.StatModificationType.Subtraction, statTypePerSp[StatType.AttackSpeed].statCalculationType);
                    statModel.ModifyAttribute(StatType.DodgeChance, attribute.Value * statTypePerSp[StatType.DodgeChance].valuePerSp, StatModel.StatModificationType.Subtraction, statTypePerSp[StatType.DodgeChance].statCalculationType);
                    break;
                case StatAttributeType.Defense:
                    statModel.ModifyAttribute(StatType.Defense, attribute.Value * statTypePerSp[StatType.Defense].valuePerSp, StatModel.StatModificationType.Subtraction, statTypePerSp[StatType.Defense].statCalculationType);
                    break;
                case StatAttributeType.CriticalHit:
                    statModel.ModifyAttribute(StatType.CriticalHitChance, attribute.Value * statTypePerSp[StatType.CriticalHitChance].valuePerSp, StatModel.StatModificationType.Subtraction, statTypePerSp[StatType.CriticalHitChance].statCalculationType);
                    break;
            }
        }
    }

    public void ProcessTraitsStatsModifiers(Dictionary<StatAttributeType, int> modifiedStatsMap)
    {
        RemoveStats(trailAttributeModifiedDic);
        trailAttributeModifiedDic = new Dictionary<StatAttributeType, int>(modifiedStatsMap);
        AddStats(trailAttributeModifiedDic);
    }

    public void ProcessHPAttributeSpModifiers(Dictionary<StatAttributeType, int> modifiedStatsMap)
    {
        RemoveStats(statPointsDic);
        statPointsDic = new Dictionary<StatAttributeType, int>(modifiedStatsMap);
        AddStats(statPointsDic);
    }
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

[Serializable]
public class StatPointInfo
{
    public StatType statType;
    public StatModel.StatCalculationType statCalculationType;
    public float valuePerSp;
}