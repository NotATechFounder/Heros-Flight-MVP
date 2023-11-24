using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rarities { Common, UnCommon, Rare, Epic, Legendary };
public enum EquipmentType { Weapon, Armour, Ring, Belt, Necklace };

public class EquipmentSO : ItemSO
{
    public Rarities rarity;
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
    public Rarities rarity;
    public int maxLevel;
    public int nextRarityFuseRequirement;
    public int defaultSellPrice;
    public int defaultScroll;
}
