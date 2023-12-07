using HeroesFlight.System.Inventory;
using HeroesFlight.System.UI;
using StansAssets.Foundation.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopSystem : IShopSystemInterface
{
    public ShopDataHolder ShopDataHolder { get; private set; }
    private IUISystem uISystem;
    private RewardSystemInterface rewardSystem;
    private InventorySystemInterface inventorySystem;
    private DataSystemInterface dataSystem; 

    public ShopSystem(IUISystem uISystemInterface, RewardSystemInterface rewardSystemInterface, InventorySystemInterface inventorySystemInterface, DataSystemInterface dataSystemInterface)
    {
        uISystem = uISystemInterface;
        rewardSystem = rewardSystemInterface;
        inventorySystem = inventorySystemInterface;
        dataSystem = dataSystemInterface;
    }

    public void Init(Scene scene = default, Action onComplete = null)
    {
        ShopDataHolder = scene.GetComponent<ShopDataHolder>();
    }


    public void InjectUiConnection()
    {
        // Suscribe to UI events
    }

    public void Reset()
    {
        // Reset UI events
    }

    public void ProccessRewards(List<Reward> rewards)
    {
        rewardSystem.ProcessRewards(rewards);
    }

    public void BuyChestWithGold(Chest.ChestType chestType)
    {
        Chest chest = ShopDataHolder.GetChest(chestType);
        if (chest == null) return;
        if (dataSystem.CurrencyManager.GetCurrencyAmount(CurrencyKeys.Gold) >= chest.GetGemChestPrice)
        {
            dataSystem.CurrencyManager.ReduceCurency(CurrencyKeys.Gold, chest.GetGemChestPrice);
            rewardSystem.ProcessRewards(chest.OpenChest());
        }
    }

    public void BuyChestWithGems(Chest.ChestType chestType)
    {
        Chest chest = ShopDataHolder.GetChest(chestType);
        if (chest == null) return;
        if (dataSystem.CurrencyManager.GetCurrencyAmount(CurrencyKeys.Gem) >= chest.GetGemChestPrice)
        {
            dataSystem.CurrencyManager.ReduceCurency(CurrencyKeys.Gem, chest.GetGemChestPrice);
            rewardSystem.ProcessRewards(chest.OpenChest());
        }
    }

    public void BuyGoldPack(GoldPack goldPack)
    {
        int index = (int)goldPack;
        CurrencyPack pack = ShopDataHolder.GetGoldPack();
        if (pack == null) return;
        if (dataSystem.CurrencyManager.GetCurrencyAmount(CurrencyKeys.Gem) >= pack.GetCost(index))
        {
            dataSystem.CurrencyManager.ReduceCurency(CurrencyKeys.Gem, pack.GetCost(index));
            dataSystem.CurrencyManager.AddCurrency(CurrencyKeys.Gold, pack.GetAmount(index));
        }
    }
}
