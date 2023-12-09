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
        uISystem.UiEventHandler.ShopMenu.TryPurchaseChest += BuyChestWithGem;
        uISystem.UiEventHandler.ShopMenu.TryPurchaseGoldPack += BuyGoldPack;
        Debug.Log("ShopSystem Injected");
    }

    public void Reset()
    {
        // Reset UI events
    }

    public void ProccessRewards(List<Reward> rewards)
    {
        rewardSystem.ProcessRewards(rewards);
    }

    public void BuyChestWithGold(ChestType chestType)
    {
        Chest chest = ShopDataHolder.GetChest(chestType);
        if (chest == null)
        {
            Debug.Log("Chest not found");
            return;
        }

        if (dataSystem.CurrencyManager.GetCurrencyAmount(CurrencyKeys.Gold) >= chest.GetGemChestPrice)
        {
            Debug.Log("Chest bought");
            dataSystem.CurrencyManager.ReduceCurency(CurrencyKeys.Gold, chest.GetGemChestPrice);
            rewardSystem.ProcessRewards(chest.OpenChest());
        }
        else
        {
            Debug.Log("Not enough gold");
        }
    }

    public void BuyChestWithGem(ChestType chestType)
    {
        Chest chest = ShopDataHolder.GetChest(chestType);
        if (chest == null)
        {
            Debug.Log("Chest not found");
            return;
        }

        if (dataSystem.CurrencyManager.GetCurrencyAmount(CurrencyKeys.Gem) >= chest.GetGemChestPrice)
        {
            dataSystem.CurrencyManager.ReduceCurency(CurrencyKeys.Gem, chest.GetGemChestPrice);
            List<Reward> rewards = chest.OpenChest();
            rewardSystem.ProcessRewards(rewards);
        }
        else
        {
            Debug.Log("Not enough gems");
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
        else
        {
            Debug.Log("Not enough gems");
        }
    }
}
