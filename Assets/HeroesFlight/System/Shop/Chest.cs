using System.Collections.Generic;
using UnityEngine;
using System;

public class Chest : MonoBehaviour
{
    [SerializeField] private ChestSO chestSO;

    private TimedReward timedReward;
    public ChestType GetChestType => chestSO.GetChestType;
    public RewardPackSO GetRewards => chestSO.GetRewardPack;
    public float GetGemChestPrice => chestSO.GetPrice;
    public float GetGoldChestPrice => chestSO.GetPrice;

    public string GetChestInfo => chestSO.GetChestInfo;

    void Start()
    {
        SetUpNormalChest();
    }

    void SetUpNormalChest()
    {
        if (chestSO.GetChestType == ChestType.Regular)
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

            timedReward.Init(this, todaysDate, TimeType.Seconds, chestSO.GetNextRewardTimeAdded, chestSO.GetCheckingInterval);
        }
    }

    public List<Reward> OpenChest()
    {
       return chestSO.GetRewardPack.GetReward();
    }

    public void OpenNormalChestWithAds()
    {
        if (chestSO.GetChestType == ChestType.Regular && timedReward.IsRewardReady())
        {
            timedReward.ClaimTimedReward();
        }
    }
}
