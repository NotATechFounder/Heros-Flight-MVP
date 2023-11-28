using System;
using System.Collections.Generic;
using HeroesFlight.Common.Progression;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public event Action<StatModel> OnValueChanged;

    [SerializeField] private StatPointSO[] statPointSO;
    private Dictionary<StatType, StatPointInfo> statTypePerSp = new Dictionary<StatType, StatPointInfo>();
    private StatModel statModel = new StatModel(new PlayerStatData());

    private Dictionary<StatAttributeType, int> traitAttributeModifiedDic = new Dictionary<StatAttributeType, int>();
    private Dictionary<StatAttributeType, int> statPointsDic = new Dictionary<StatAttributeType, int>();
    private new List<StatTypeWithValue> equippedItemsStatDic  = new List<StatTypeWithValue>();


    public StatModel GetStatModel()
    {
        return statModel;
    }

    public void Init()
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

    public void ModifyStatAttribute(Dictionary<StatAttributeType, int> attributes, StatModel.StatModificationType statModificationType)
    {
        foreach (KeyValuePair<StatAttributeType, int> attribute in attributes)
        {
            switch (attribute.Key)
            {
                case StatAttributeType.Power:
                    statModel.ModifyAttribute(StatType.PhysicalDamage, attribute.Value * statTypePerSp[StatType.PhysicalDamage].valuePerSp, statModificationType, statTypePerSp[StatType.PhysicalDamage].statCalculationType);
                    statModel.ModifyAttribute(StatType.MagicDamage, attribute.Value * statTypePerSp[StatType.MagicDamage].valuePerSp, statModificationType, statTypePerSp[StatType.MagicDamage].statCalculationType);
                    break;
                case StatAttributeType.Vitality:
                    statModel.ModifyAttribute(StatType.MaxHealth, attribute.Value * statTypePerSp[StatType.MaxHealth].valuePerSp, statModificationType, statTypePerSp[StatType.MaxHealth].statCalculationType);
                    break;
                case StatAttributeType.Agility:
                    statModel.ModifyAttribute(StatType.MoveSpeed, attribute.Value * statTypePerSp[StatType.MoveSpeed].valuePerSp, statModificationType, statTypePerSp[StatType.MoveSpeed].statCalculationType);
                    statModel.ModifyAttribute(StatType.AttackSpeed, attribute.Value * statTypePerSp[StatType.AttackSpeed].valuePerSp, statModificationType, statTypePerSp[StatType.AttackSpeed].statCalculationType);
                    statModel.ModifyAttribute(StatType.DodgeChance, attribute.Value * statTypePerSp[StatType.DodgeChance].valuePerSp, statModificationType, statTypePerSp[StatType.DodgeChance].statCalculationType);
                    break;
                case StatAttributeType.Defense:
                    statModel.ModifyAttribute(StatType.Defense, attribute.Value * statTypePerSp[StatType.Defense].valuePerSp, statModificationType, statTypePerSp[StatType.Defense].statCalculationType);
                    break;
                case StatAttributeType.CriticalHit:
                    statModel.ModifyAttribute(StatType.CriticalHitChance, attribute.Value * statTypePerSp[StatType.CriticalHitChance].valuePerSp, statModificationType, statTypePerSp[StatType.CriticalHitChance].statCalculationType);
                    break;
            }
        }
    }

    public void ModifyStatType(List<StatTypeWithValue> stats, StatModel.StatModificationType statModificationType)
    {
        foreach (StatTypeWithValue attribute in stats)
        {
            Debug.Log(attribute.statType + " " + attribute.value);

            switch (attribute.statType)
            {
                case StatType.PhysicalDamage:
                    statModel.ModifyAttribute(StatType.PhysicalDamage, attribute.value, statModificationType, attribute.statCalculationType);
                    break;
                case StatType.MagicDamage:
                    statModel.ModifyAttribute(StatType.MagicDamage, attribute.value, statModificationType, attribute.statCalculationType);
                    break;
                case StatType.MaxHealth:
                    statModel.ModifyAttribute(StatType.MaxHealth, attribute.value, statModificationType, attribute.statCalculationType);
                    break;
                case StatType.MoveSpeed:
                    statModel.ModifyAttribute(StatType.MoveSpeed, attribute.value, statModificationType, attribute.statCalculationType);
                    break;
                case StatType.AttackSpeed:
                    statModel.ModifyAttribute(StatType.AttackSpeed, attribute.value, statModificationType, attribute.statCalculationType);
                    break;
                case StatType.DodgeChance:
                    statModel.ModifyAttribute(StatType.DodgeChance, attribute.value, statModificationType, attribute.statCalculationType);
                    break;
                case StatType.Defense:
                    statModel.ModifyAttribute(StatType.Defense, attribute.value, statModificationType, attribute.statCalculationType);
                    break;
                case StatType.CriticalHitChance:
                    statModel.ModifyAttribute(StatType.CriticalHitChance, attribute.value, statModificationType, attribute.statCalculationType);
                    break;
                default:   break;
            }
        }
    }

    public void ProcessTraitsStatsModifiers(Dictionary<StatAttributeType, int> modifiedStatsMap)
    {
        ModifyStatAttribute(traitAttributeModifiedDic, StatModel.StatModificationType.Subtraction);
        traitAttributeModifiedDic = new Dictionary<StatAttributeType, int>(modifiedStatsMap);
        ModifyStatAttribute(traitAttributeModifiedDic , StatModel.StatModificationType.Addition);

        OnValueChanged?.Invoke(statModel);
    }

    public void ProcessStatPointsModifiers(Dictionary<StatAttributeType, int> modifiedStatsMap)
    {
        ModifyStatAttribute(statPointsDic , StatModel.StatModificationType.Subtraction);
        statPointsDic = new Dictionary<StatAttributeType, int>(modifiedStatsMap);
        ModifyStatAttribute(statPointsDic, StatModel.StatModificationType.Addition);

        OnValueChanged?.Invoke(statModel);
    }

    public void ProcessEquippedItemsModifiers(List<StatTypeWithValue> modifiedStatsMap)
    {
        ModifyStatType(equippedItemsStatDic, StatModel.StatModificationType.Subtraction);
        equippedItemsStatDic = new List<StatTypeWithValue>(modifiedStatsMap);
        ModifyStatType(equippedItemsStatDic, StatModel.StatModificationType.Addition);
        OnValueChanged?.Invoke(statModel);
    }

    public void ProcessAllModifiers(PlayerStatData playerStatData)
    {
        statModel = new StatModel(playerStatData);
        ModifyStatAttribute(statPointsDic, StatModel.StatModificationType.Addition);
        ModifyStatAttribute(traitAttributeModifiedDic, StatModel.StatModificationType .Addition);
        ModifyStatType(equippedItemsStatDic, StatModel.StatModificationType.Addition);
        OnValueChanged?.Invoke(statModel);
    }

    private void OnGUI()
    {
        //if (statModel != null)
        //{
        //    float space = 30;
        //    foreach (KeyValuePair<StatType, float> stat in statModel.CurrentStatDic)
        //    {
        //        // draw in a big size
        //        GUI.Label(new Rect(500, space, 500, 500), stat.Key.ToString() + ": " + stat.Value.ToString());
        //        space += 30;
        //    }
        //}
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
    CriticalHitChance,
    AllStats,
    HealingBooster,
    AbilityDamage,

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

    private Dictionary<StatType, float> baseStatDic;
    private Dictionary<StatType, float> currentStatDic;
    private PlayerStatData playerStatData;

    public Dictionary<StatType, float> CurrentStatDic => currentStatDic;
    public PlayerStatData GetPlayerStatData => playerStatData;

    public StatModel(PlayerStatData playerStatData)
    {
        this.playerStatData = playerStatData;
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