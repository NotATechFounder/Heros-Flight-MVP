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

    public Chest GetChest(ChestType chestType)
    {
        return Array.Find(chests, chest => chest.GetChestType == chestType);
    }

    public GoldPackGroup GetGoldPack()
    {
        return goldPack;
    }

    public void SetRegularChestTimeReward()
    {
        reqularChestTimeReward = new TimedReward();
        reqularChestTimeReward.OnInternetConnected = () =>
        {
            Debug.Log("Internet Connected");
        };

        reqularChestTimeReward.OnInternetDisconnected = () =>
        {
            Debug.Log("Internet Disconnected");
        };

        reqularChestTimeReward.OnRewardReadyToBeCollected = () =>
        {
            Debug.Log("Reward Ready To Be Collected");
        };

        reqularChestTimeReward.RewardPlayer = (LastRewardClaimDate) =>
        {
            // enable button
            Debug.Log("Reward Player");
        };

        reqularChestTimeReward.OnTimerUpdateed = (time) =>
        {
            // Debug.Log("Timer Updated" + time);
        };

        reqularChestTimeReward.Init(this, regularChestRewardHandler.GetLastRewardTime, regularChestRewardHandler.GetTimeType, 
            regularChestRewardHandler.GetNextRewardTimeAdded, regularChestRewardHandler.GetCheckingInterval);
    }

    public void SetGoldChestTimeReward()
    {
        goldTimeReward = new TimedReward();
        goldTimeReward.OnInternetConnected = () =>
        {
            Debug.Log("Internet Connected");
        };

        goldTimeReward.OnInternetDisconnected = () =>
        {
            Debug.Log("Internet Disconnected");
        };

        goldTimeReward.OnRewardReadyToBeCollected = () =>
        {
            Debug.Log("Reward Ready To Be Collected");
        };

        goldTimeReward.RewardPlayer = (LastRewardClaimDate) =>
        {
            // enable button
            Debug.Log("Reward Player");
        };

        goldTimeReward.OnTimerUpdateed = (time) =>
        {
            // Debug.Log("Timer Updated" + time);
        };

        goldTimeReward.Init(this, goldChestRewardHandler.GetLastRewardTime, goldChestRewardHandler.GetTimeType,
                       goldChestRewardHandler.GetNextRewardTimeAdded, goldChestRewardHandler.GetCheckingInterval);
    }
}
