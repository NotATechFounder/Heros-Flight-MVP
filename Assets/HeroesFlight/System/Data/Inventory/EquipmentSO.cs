using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rarity { Common, UnCommon, Rare, Epic};
public enum EquipmentType { Weapon, Armour, Ring, Belt, Necklace };

public class EquipmentSO : ItemSO
{
    public EquipmentType equipmentType;
    public StatType statType;
    public HeroType heroType;
    public ItemStatByRarity[] itemBaseStats;
    public ItemRarityStat[] itemRarityStats;

    private void Awake() => itemType = ItemType.Equipment;

    public int GetBaseStatValue(Rarity rarity)
    {
        foreach (ItemStatByRarity itemBaseStat in itemBaseStats)
        {
            if (itemBaseStat.rarity == rarity) return (int)itemBaseStat.value;
        }
        return 0;
    }
}


[Serializable]
public class ItemStatByRarity
{
    public Rarity rarity;
    public float value;
}

[Serializable]
public class StatTypeWithValue
{
    public StatType statType;
    public int value;
    public StatModel.StatCalculationType statCalculationType;

    public StatTypeWithValue(StatType statType, int value, StatModel.StatCalculationType statCalculationType)
    {
        this.statType = statType;
        this.value = value;
        this.statCalculationType = statCalculationType;
    }
}

[Serializable]
public class UniqueEffect
{
    public EquipmentType equipmentType;
    public ItemRarityStat[] itemRarityStats;
}

[Serializable]
public class ItemRarityStat
{
    public Rarity rarity;
    public StatTypeWithValue statTypeWithValue;
}

[Serializable]
public class ItemEffect
{
    public ItemEffectType itemEffectType;
    public int value;
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


[Serializable]
public class RarityPalette
{
    public Color frameColour;
    public Color backgroundColour;
}
