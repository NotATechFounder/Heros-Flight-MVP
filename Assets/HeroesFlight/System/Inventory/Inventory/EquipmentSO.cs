using System;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.Common;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.FileManager.Stats;
using UnityEngine;



public class EquipmentSO : ItemSO
{
    public EquipmentType equipmentType;
    public StatType statType;
    public HeroType heroType;
    public ItemStatByRarity[] itemBaseStats;
    public ItemEffect specialHeroEffect;
    public ItemRarityStat[] itemRarityStats;

    private void Awake() => itemType = ItemType.Equipment;

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



