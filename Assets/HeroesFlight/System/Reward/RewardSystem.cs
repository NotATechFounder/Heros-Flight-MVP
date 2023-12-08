using HeroesFlight.Common;
using HeroesFlight.System.Inventory;
using HeroesFlight.System.UI;
using HeroesFlight.System.UI.Reward;
using StansAssets.Foundation.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class RewardSystem : RewardSystemInterface
{
    private DataSystemInterface dataSystem;
    private InventorySystemInterface inventorySystem;
    private IUISystem uiSystem;
    private RewardDataHandler rewardDataHandler;

    public RewardSystem(DataSystemInterface dataSystemInterface, InventorySystemInterface inventorySystemInterface , IUISystem uiSystemInterface)
    {
        dataSystem = dataSystemInterface;
        inventorySystem = inventorySystemInterface;
        uiSystem = uiSystemInterface;
    }

    public void Init(Scene scene = default, Action onComplete = null)
    {
        rewardDataHandler = scene.GetComponent<RewardDataHandler>();
        rewardDataHandler.Initialise();
    }


    public void Reset()
    {

    }

    public void InjectUiConnection()
    {
        uiSystem.UiEventHandler.DailyRewardMenu.GetRewardVisuals += GetDailyRewardVisual;
        uiSystem.UiEventHandler.DailyRewardMenu.OnClaimRewardButtonClicked += ClaimReward;
        uiSystem.UiEventHandler.DailyRewardMenu.GetLastUnlockedIndex += rewardDataHandler.GetLastUnlockedIndex;
        uiSystem.UiEventHandler.DailyRewardMenu.IsRewardReady += rewardDataHandler.IsRewardReady;
        rewardDataHandler.OnRewardReadyToBeCollected += uiSystem.UiEventHandler.DailyRewardMenu.OnRewardReadyToBeCollected;
    }

    public void ClaimReward(int day)
    {
        Reward[] rewards = rewardDataHandler.GetDailyRewardSO.GetRewards(day);
        ProcessRewards(rewards.ToList());
        uiSystem.UiEventHandler.DailyRewardMenu.RefreshDailyRewards();
        rewardDataHandler.ClaimedReward();
    }

    private List<RewardVisual> GetDailyRewardVisual(int day)
    {
        List <RewardVisual> rewardVisuals = new List<RewardVisual>();
        Reward[] rewards = rewardDataHandler.GetDailyRewardSO.GetRewards(day);
        for (int i = 0; i < rewards.Length; i++)
        {
            rewardVisuals.Add(GetRewardVisual(rewards[i]));
        }
        return rewardVisuals;
    }

    public void ProcessReward(Reward reward)
    {
        switch (reward.GetRewardType())
        {
            case RewardType.Currency:
                dataSystem.CurrencyManager.AddCurrency(reward.GetRewardObject<CurrencySO>(), reward.GetAmount());
                break;
            case RewardType.Item:
                inventorySystem.InventoryHandler.AddToInventory(reward.GetRewardObject<ItemSO>(), reward.GetAmount(), reward.GetRarity());
                break;
            default: break;
        }
    }

    public void ProcessRewards(List<Reward> rewards)
    {
        List<InventoryAddModificator> inventoryAddModificators = new List<InventoryAddModificator>();

        foreach (Reward reward in rewards)
        {
            switch (reward.GetRewardType())
            {
                case RewardType.Currency:
                    dataSystem.CurrencyManager.AddCurrency(reward.GetRewardObject<CurrencySO>(), reward.GetAmount());
                    break;
                case RewardType.Item:
                    inventoryAddModificators.Add(new InventoryAddModificator(reward.GetRewardObject<ItemSO>(), reward.GetAmount(), reward.GetRarity()));
                    break;
                default: break;
            }
        }

        inventorySystem.InventoryHandler.AddMultipleToInventory(inventoryAddModificators.ToArray());
    }

    public RewardVisual GetRewardVisual(Reward reward)
    {
        RewardVisual rewardVisual = new RewardVisual();
        switch (reward.GetRewardType())
        {
            case RewardType.Currency:
                CurrencySO currencySO = reward.GetRewardObject<CurrencySO>();
                rewardVisual.icon = currencySO.GetSprite;
                rewardVisual.color = currencySO.GetColor;
                rewardVisual.name = currencySO.GetCurrencyName;

                break;
            case RewardType.Item:
                ItemSO itemSO = reward.GetRewardObject<ItemSO>();
                rewardVisual.icon = itemSO.icon;
                rewardVisual.name = itemSO.Name;
                RarityPalette rarityPalette = inventorySystem.InventoryHandler.GetPalette(reward.GetRarity());
                rewardVisual.color = rarityPalette.backgroundColour;
                break;
            default: break;
        }

        rewardVisual.amount = reward.GetAmount();
        return rewardVisual;
    }

    public void OtherOption(Reward reward)
    {
        //switch (reward.GetRewardObject<ScriptableObject>())
        //{
        //    case CurrencySO:
        //        currencyManager.AddCurrency(reward.GetRewardObject<CurrencySO>(), reward.GetAmount());
        //        break;
        //    case ItemSO:
        //        InventoryHandler.AddToInventory(reward.GetRewardObject<ItemSO>(), reward.GetAmount(), reward.GetRarity());
        //        break;
        //    default: break;
        //}
    }}
