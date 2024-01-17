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
    public EquipmentStar equipmentStar;
    public EquipmentType equipmentType;
    public StatType statType;
    public HeroType heroType;
    public StatTypeWithValue specialHeroEffect;
    public UniqueStatModificationEffect[] uniqueStatModificationEffects;
    public UniqueCombatEffect[] uniqueCombatEffects;

    private void Awake() => itemType = ItemType.Equipment;

    private void OnValidate()
    {
        for (int i = 0; i < uniqueStatModificationEffects.Length; i++)
        {
            uniqueStatModificationEffects[i].curve.UpdateCurve();
        }

        for (int i = 0; i < uniqueCombatEffects.Length; i++)
        {
            uniqueCombatEffects[i].curve.UpdateCurve();
        }
    }
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
}

[Serializable]
public class StarIncrease
{
    public EquipmentStar equipmentStar;
    public int incrementPerLevel;
}

[Serializable]
public class EquipmentStatIncrease
{
    public EquipmentType equipmentType;
    public StarIncrease[] starIncreases;
}

