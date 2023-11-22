using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rarities { Common, Great, Rare, Epic, Legendary };
public enum EquipmentType { Weapon, Armour, Accessory, Scroll, Pet };

public class EquipmentObject : ItemSO
{
    public Rarities rarity;
    public EquipmentType equipmentType;
    public BuffType[] buffs;

    private void Awake() => itemType = ItemType.Equipment;
}

[Serializable]
public class ItemBuff
{
    public BuffType buffType;
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
