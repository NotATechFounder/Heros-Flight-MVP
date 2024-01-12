using System;
using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;

public class DiceHandler : MonoBehaviour
{
    [SerializeField] private int gemCost;
    [SerializeField] private TimedRewardHandler rewardHandler;

    private TimedReward timeReward;

    public int GetGemCost => gemCost;
    public TimedRewardHandler GetTimedRewardHandler => rewardHandler;


    public void Init()
    {
        SetRegularChestTimeReward();
    }

    public void SetRegularChestTimeReward()
    {
        rewardHandler.LoadData();

        timeReward = new TimedReward();
        timeReward.OnInternetConnected = () =>
        {

        };

        timeReward.OnInternetDisconnected = () =>
        {

        };

        timeReward.OnRewardReadyToBeCollected = () =>
        {
            timeReward.ClaimTimedReward();
        };

        timeReward.RewardPlayer = (LastRewardClaimDate) =>
        {
            rewardHandler.ResetRewardCount(LastRewardClaimDate);
        };

        timeReward.OnTimerUpdated = (time) =>
        {
            // Debug.Log("Timer Updated" + time);
        };

        timeReward.Init(this, rewardHandler.GetLastRewardTime, rewardHandler.GetTimeType,
            rewardHandler.GetNextRewardTimeAdded, rewardHandler.GetCheckingInterval);
    }
}
