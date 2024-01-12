using HeroesFlight.System.FileManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardDataHandler : MonoBehaviour
{
    public class DailyRewardData
    {
        public int lastRewardIndex;
        public string lastClaimedTime;
    }
    public event Action<int> OnRewardReadyToBeCollected;
    public const string DailyReward_Save = "DailyRewardData";

    [Header("Daily Reward")]

    [SerializeField] TimeType timeType;
    [SerializeField] float nextRewardTimeAdded = 20f;
    [SerializeField] float checkingInterval = 2f;
    [SerializeField] private DailyRewardSO dailyRewardSO;
    private TimedReward timedReward;
    private DailyRewardData dailyRewardData;
    public DailyRewardSO GetDailyRewardSO => dailyRewardSO;

    public void Initialise()
    {
        Load();
        SetUp();
    }

    void SetUp()
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
            // The reward is ready to be collected
            OnRewardReadyToBeCollected?.Invoke(dailyRewardData.lastRewardIndex);
        };

        timedReward.RewardPlayer = (LastRewardClaimDate) =>
        {
            dailyRewardData.lastRewardIndex = (dailyRewardData.lastRewardIndex + 1) % 7;
            dailyRewardData.lastClaimedTime = LastRewardClaimDate;
            Save();
        };

        timedReward.OnTimerUpdated = (time) =>
        {
            // Debug.Log("Timer Updated" + time);
        };

        timedReward.Init(this, dailyRewardData.lastClaimedTime, timeType, nextRewardTimeAdded, checkingInterval);
    }

    public void ClaimedReward()
    {
        timedReward.ClaimTimedReward();
    }

    public void Load()
    {
        DailyRewardData loadedData = FileManager.Load<DailyRewardData>(DailyReward_Save);
        dailyRewardData = loadedData ?? new DailyRewardData();
        if (loadedData == null)
        {
            dailyRewardData.lastClaimedTime = InternetManager.Instance.GetCurrentDateTime().ToString();
        }
    }

    public void Save()
    {
        FileManager.Save(DailyReward_Save, dailyRewardData);
    }

    public int GetLastUnlockedIndex()
    {
        return dailyRewardData.lastRewardIndex;
    }

    public bool IsRewardReady()
    {
        return timedReward.IsRewardReady();
    }
}
