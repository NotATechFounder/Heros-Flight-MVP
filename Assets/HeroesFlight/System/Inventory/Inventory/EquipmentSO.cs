using System;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.Common;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Effects.Effects.Data;
using HeroesFlight.System.Combat.Effects.Effects;
using HeroesFlight.System.FileManager.Stats;
using UnityEngine;



public class EquipmentSO : ItemSO
{
    public EquipmentType equipmentType;
    public StatType statType;
    public HeroType heroType;
    public ItemStatByRarity[] itemBaseStats;
    public StatTypeWithValue specialHeroEffect;
    public UniqueStatModificationEffect[] uniqueStatModificationEffects;
    public UniqueCombatEffect[] uniqueCombatEffects;

    private void Awake() => itemType = ItemType.Equipment;

    private void OnValidate()
    {
        for (int i = 0; i < uniqueStatModificationEffects.Length; i++)
        {
            uniqueStatModificationEffects[i].curve.curveType = CurveType.Linear;
            uniqueStatModificationEffects[i].curve.UpdateCurve();
        }

        for (int i = 0; i < uniqueCombatEffects.Length; i++)
        {
            uniqueStatModificationEffects[i].curve.curveType = CurveType.Linear;
            uniqueCombatEffects[i].curve.UpdateCurve();
        }
    }

    public int GetBaseStatValue(Rarity rarity)
    {
        foreach (ItemStatByRarity itemBaseStat in itemBaseStats)
        {
            if (itemBaseStat.rarity == rarity) return itemBaseStat.value;
        }
        return 0;
    }
}


[Serializable]
public class ItemStatByRarity
{
    public Rarity rarity;
    public int value;
}

[Serializable]
public class UniqueStatModificationEffect
{
    [SerializeField] public Rarity rarity;
    [SerializeField] public StatType statType;
    [SerializeField] public CustomAnimationCurve curve;

    public Rarity GetRarity() => rarity;
    public StatTypeWithValue GetStatTypeWithValue(int currentLevel)
    {
        StatTypeWithValue statTypeWithValue = new StatTypeWithValue();
        statTypeWithValue.statType = statType;
        statTypeWithValue.value = curve.GetCurrentValueInt(currentLevel);
        return statTypeWithValue;
    }
}

[Serializable]
public class UniqueCombatEffect
{
    public Rarity rarity;
    public CombatEffect combatEffect;
    public CustomAnimationCurve curve;
}

[Serializable]
public class RarityInfo
{
    public Rarity rarity;
    public RarityPalette palette;
    public int maxLevel;
    public int nextRarityFuseRequirement;

    [Header("Default Prices")]
    public int defaultDisamatlePrice;
    public int defaultMaterial;

    [Header("Stat")]
    public int incrementPerLevel;

    public int GetValue(int baseValue, int level)
    {
        return baseValue + incrementPerLevel * (level - 1);
    }
}



