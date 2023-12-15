using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopDataHolder : MonoBehaviour
{
    [SerializeField] private Chest[] chests;
    [SerializeField] private GoldPackGroup goldPack;

    public Chest[] GetChests => chests;
    public GoldPackGroup GoldPackGroup => goldPack;

    public Chest GetChest(ChestType chestType)
    {
        return Array.Find(chests, chest => chest.GetChestType == chestType);
    }

    public GoldPackGroup GetGoldPack()
    {
        return goldPack;
    }
}
