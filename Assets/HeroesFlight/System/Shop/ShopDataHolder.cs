using System;
using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;

public class ShopDataHolder : MonoBehaviour
{
    [SerializeField] private Chest[] chests;
    [SerializeField] private GoldPackGroup goldPack;

    [SerializeField] private TimedRewardHandler regularChestRewardHandler;
    [SerializeField] private TimedRewardHandler goldChestRewardHandler;

    private TimedReward goldTimeReward;
    private TimedReward reqularChestTimeReward;

    public Chest[] GetChests => chests;
    public GoldPackGroup GoldPackGroup => goldPack;

    public TimedRewardHandler GetTimedRegularChestRewardHandler => regularChestRewardHandler;
    public TimedRewardHandler GetTimeGoldPackRewardHandlerGold => goldChestRewardHandler;

    public Chest GetChest(ChestType chestType)
    {
        return Array.Find(chests, chest => chest.GetChestType == chestType);
    }

    public GoldPackGroup GetGoldPack()
    {
        return goldPack;
    }

    public void Init()
    {
        SetRegularChestTimeReward();
        SetGoldChestTimeReward();
    }

    public void SetRegularChestTimeReward()
    {
        regularChestRewardHandler.LoadData();

        reqularChestTimeReward = new TimedReward();
        reqularChestTimeReward.OnInternetConnected = () =>
        {
  
        };

        reqularChestTimeReward.OnInternetDisconnected = () =>
        {
   
        };

        reqularChestTimeReward.OnRewardReadyToBeCollected = () =>
        {
            reqularChestTimeReward.ClaimTimedReward();
        };

        reqularChestTimeReward.RewardPlayer = (LastRewardClaimDate) =>
        {
            regularChestRewardHandler.ResetRewardCount(LastRewardClaimDate);
        };

        reqularChestTimeReward.OnTimerUpdated = (time) =>
        {
            // Debug.Log("Timer Updated" + time);
        };

        reqularChestTimeReward.Init(this, regularChestRewardHandler.GetLastRewardTime, regularChestRewardHandler.GetTimeType, 
            regularChestRewardHandler.GetNextRewardTimeAdded, regularChestRewardHandler.GetCheckingInterval);
    }

    public void SetGoldChestTimeReward()
    {
        goldChestRewardHandler.LoadData();

        goldTimeReward = new TimedReward();
        goldTimeReward.OnInternetConnected = () =>
        {

        };

        goldTimeReward.OnInternetDisconnected = () =>
        {

        };

        goldTimeReward.OnRewardReadyToBeCollected = () =>
        {
            goldTimeReward.ClaimTimedReward();
        };

        goldTimeReward.RewardPlayer = (LastRewardClaimDate) =>
        {
            goldChestRewardHandler.ResetRewardCount(LastRewardClaimDate);
        };

        goldTimeReward.OnTimerUpdated = (time) =>
        {
            // Debug.Log("Timer Updated" + time);
        };

        goldTimeReward.Init(this, goldChestRewardHandler.GetLastRewardTime, goldChestRewardHandler.GetTimeType,
            goldChestRewardHandler.GetNextRewardTimeAdded, goldChestRewardHandler.GetCheckingInterval);
    }
}
