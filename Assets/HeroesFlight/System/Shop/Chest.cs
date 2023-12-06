using System.Collections.Generic;
using UnityEngine;
using System;

public class Chest : MonoBehaviour
{
    public enum ChestType
    {
        Normal,
        Rare,
        Epic
    }

    [SerializeField] private ChestType chestType;
    [SerializeField] private RewardPack rewards;
    [SerializeField] float gemchestPrice;
    [SerializeField] float goldchestPrice;

    [Header("Timed Reward")]
    [SerializeField] TimeType timeType;
    [SerializeField] float nextRewardTimeAdded = 20f;
    [SerializeField] float checkingInterval = 2f;

    private TimedReward timedReward;
    public ChestType GetChestType => chestType;
    public RewardPack GetRewards => rewards;

    public float GetGemChestPrice => gemchestPrice;
    public float GetGoldChestPrice => goldchestPrice;

    void Start()
    {
        SetUpNormalChest();
    }

    void SetUpNormalChest()
    {
        if (chestType == ChestType.Normal)
        {
            timedReward = new TimedReward();
            timedReward.OnInternetConnected = () =>
            {
                Debug.Log("Internet Connected");
            };

            timedReward.OnInternetDisconnected = () =>
            {
                Debug.Log("Internet Disconnected");
            };

            timedReward.OnRewardReadyToBeCollected = () =>
            {
                Debug.Log("Reward Ready To Be Collected");
            };

            timedReward.RewardPlayer = (LastRewardClaimDate) =>
            {
                // enable button
                Debug.Log("Reward Player");
            };

            timedReward.OnTimerUpdateed = (time) =>
            {
               // Debug.Log("Timer Updated" + time);
            };

            string todaysDate = DateTime.Now.ToString();

            timedReward.Init(this, todaysDate, TimeType.Seconds, nextRewardTimeAdded, checkingInterval);
        }
    }

    public void OpenChest()
    {
        rewards.GiveReward();
    }

    public void OpenNormalChestWithAds()
    {
        if (chestType == ChestType.Normal && timedReward.IsRewardReady())
        {
            timedReward.ClaimTimedReward();
        }
    }
}
