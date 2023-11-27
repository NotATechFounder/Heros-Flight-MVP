using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rarity { Common, UnCommon, Rare, Epic, Legendary };
public enum EquipmentType { Weapon, Armour, Ring, Belt, Necklace };

public class EquipmentSO : ItemSO
{
    public Rarity rarity;
    public EquipmentType equipmentType;
    public ItemEffectType[] effects;

    private void Awake() => itemType = ItemType.Equipment;
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
    public int defaultDisamatlePrice;
    public int defaultMaterial;
}


[Serializable]
public class RarityPalette
{
    public Color frameColour;
    public Color backgroundColour;
}
