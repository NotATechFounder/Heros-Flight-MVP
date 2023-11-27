using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rarity { Common, UnCommon, Rare, Epic, Legendary };
public enum EquipmentType { Weapon, Armour, Ring, Belt, Necklace };

public class EquipmentSO : ItemSO
{
    public EquipmentType equipmentType;
    public StatType statType;
    public BonusStat bonusStat;
    public ItemRarityStat[] itemRarityStats;

    private void Awake() => itemType = ItemType.Equipment;
}

[Serializable]
public class BonusStat
{
    public HeroType heroType;
    public int value;
}

[Serializable]
public class ItemRarityStat
{
    public Rarity rarity;
    public ItemEffectType effects;
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
    public int baseValue;
    public int incrementPerLevel;

    public int GetValue(int level)
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
