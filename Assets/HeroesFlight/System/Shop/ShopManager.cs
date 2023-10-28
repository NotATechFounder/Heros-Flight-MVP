using System;
using System.Collections;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public CurrencyManager currencyManager;

    [SerializeField] private Chest[] chests;

    public void SetCurrencyManager(CurrencyManager currencyManager)
    {
        this.currencyManager = currencyManager;
    }

    public void BuyChestWithGold(Chest.ChestType chestType)
    {
        Chest chest = GetChest(chestType);
        if (chest == null) return;
        if (currencyManager.GetCurrencyAmount(CurrencyKeys.Gold) >= chest.GetGemChestPrice)
        {
            currencyManager.ReduceCurency(CurrencyKeys.Gold, chest.GetGemChestPrice);
            chest.OpenChest();
        }
    }

    public void BuyChestWithGems(Chest.ChestType chestType)
    {
        Chest chest = GetChest(chestType);
        if (chest == null) return;
        if (currencyManager.GetCurrencyAmount(CurrencyKeys.Gem) >= chest.GetGemChestPrice)
        {
            currencyManager.ReduceCurency(CurrencyKeys.Gem, chest.GetGemChestPrice);
            chest.OpenChest();
        }
    }

    private Chest GetChest(Chest.ChestType chestType)
    {
        return Array.Find(chests, chest => chest.GetChestType == chestType);
    }
}
