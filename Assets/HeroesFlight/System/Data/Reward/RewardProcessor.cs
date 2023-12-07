using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardProcessor 
{
    //private CurrencyManager currencyManager;
    //private InventoryHandler InventoryHandler;

    //public RewardProcessor(CurrencyManager currencyManager, InventoryHandler inventoryHandler)
    //{
    //    this.currencyManager = currencyManager;
    //    this.InventoryHandler = inventoryHandler;
    //}

    //public void ProcessReward(Reward reward)
    //{
    //    //switch (reward.GetRewardObject<ScriptableObject>())
    //    //{
    //    //    case CurrencySO:
    //    //        currencyManager.AddCurrency(reward.GetRewardObject<CurrencySO>(), reward.GetAmount());
    //    //        break;
    //    //    case ItemSO:
    //    //        InventoryHandler.AddToInventory(reward.GetRewardObject<ItemSO>(), reward.GetAmount(), reward.GetRarity());
    //    //        break;
    //    //    default: break;
    //    //}

    //    switch (reward.GetRewardType())
    //    {
    //        case RewardType.Currency:
    //            currencyManager.AddCurrency(reward.GetRewardObject<CurrencySO>(), reward.GetAmount());
    //            break;
    //        case RewardType.Item:
    //            InventoryHandler.AddToInventory(reward.GetRewardObject<ItemSO>(), reward.GetAmount(), reward.GetRarity());
    //            break;
    //        default: break;
    //    }
    //}

    //public void ProcessRewards(List<Reward> rewards)
    //{
    //    List<InventoryAddModificator> inventoryAddModificators = new List<InventoryAddModificator>();

    //    foreach (Reward reward in rewards)
    //    {
    //        switch (reward.GetRewardType())
    //        {
    //            case RewardType.Currency:
    //                currencyManager.AddCurrency(reward.GetRewardObject<CurrencySO>(), reward.GetAmount());
    //                break;
    //            case RewardType.Item:
    //                inventoryAddModificators.Add(new InventoryAddModificator(reward.GetRewardObject<ItemSO>(), reward.GetAmount(), reward.GetRarity()));
    //                break;
    //            default: break;
    //        }
    //    }

    //    InventoryHandler.AddMultipleToInventory(inventoryAddModificators.ToArray());
    //}
}
