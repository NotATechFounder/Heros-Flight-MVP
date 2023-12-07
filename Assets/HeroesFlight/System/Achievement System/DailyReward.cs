using HeroesFlight.System.FileManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyReward : MonoBehaviour
{
    public event Action<int> OnRewardReadyToBeCollected;

    public const string DailyReward_Save = "DailyRewardData";

    [SerializeField] private RewardPack rewardPacks;
    [SerializeField] private Reward reward;

    [Header("Timed Reward")]
    [SerializeField] TimeType timeType;
    [SerializeField] float nextRewardTimeAdded = 20f;
    [SerializeField] float checkingInterval = 2f;

    [Header("Data")]
    [SerializeField] Data data;
    private TimedReward timedReward;

    [System.Serializable]
    public class Data
    {
        public int lastRewardIndex;
        public string lastClaimedTime;
    }

    private void Start()
    {
        Load();
        SetUp();

        timedReward.OnTimerUpdateed += (time) =>
        {
            Debug.Log("Timer Updated" + time);
        };
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
            OnRewardReadyToBeCollected?.Invoke(data.lastRewardIndex);
            Debug.Log("Reward Ready To Be Collected");
        };

        timedReward.RewardPlayer = (LastRewardClaimDate) =>
        {
            //rewardPacks.GiveSingleReward(data.lastRewardIndex);
            data.lastRewardIndex = data.lastRewardIndex >= 7 ? 0 : data.lastRewardIndex;
            data.lastRewardIndex++;
            data.lastClaimedTime = LastRewardClaimDate;
            Save();
            Debug.Log("Rewarded Player");
        };

        timedReward.OnTimerUpdateed = (time) =>
        {
            // Debug.Log("Timer Updated" + time);
        };

        timedReward.Init(this, data.lastClaimedTime, timeType, nextRewardTimeAdded, checkingInterval);
    }

    public void ClaimReward()
    {
        timedReward.ClaimTimedReward();
    }

    public void Load()
    {
        Data loadedData = FileManager.Load<Data>(DailyReward_Save);
        data = loadedData ?? new Data();
        if (loadedData == null)
        {
            data.lastClaimedTime = InternetManager.Instance.GetCurrentDateTime().ToString();
        }
    }

    public void Save()
    {
        FileManager.Save(DailyReward_Save, data);
    }

    public int GetLastUnlockedIndex()
    {
        return data.lastRewardIndex;
    }

    public bool IsRewardReady()
    {
        return timedReward.IsRewardReady();
    }
}
