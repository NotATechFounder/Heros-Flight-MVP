using HeroesFlight.System.FileManager;
using System;
using UnityEngine;

[System.Serializable]
public class TimedRewardHandler
{
    public event Action<int> OnRewardChanged;

    [SerializeField] private string key;
    [SerializeField] private int rewardCount;
    [SerializeField] TimeType timeType;
    [SerializeField] float nextRewardTimeAdded = 20f;
    [SerializeField] float checkingInterval = 2f;
    [SerializeField] Data data;

    public string GetKey => key;
    public TimeType GetTimeType => timeType;
    public float GetNextRewardTimeAdded => nextRewardTimeAdded;
    public float GetCheckingInterval => checkingInterval;
    public string GetLastRewardTime => data.lastRewardTime;
    public int GetRewardCount => data.rewardCount;

    public void ReduceRewardCount()
    {
        data.rewardCount--;
        FileManager.Save(key, data);
        OnRewardChanged?.Invoke(data.rewardCount);
    }

    public void ResetRewardCount(string time)
    {
        data.rewardCount = rewardCount;
        data.lastRewardTime = time;
        FileManager.Save(key, data);
        OnRewardChanged?.Invoke(data.rewardCount);
    }

    public void LoadData()
    {
        Data savedData = FileManager.Load<Data>(key);
        if (savedData == null)
        {
            data = new Data();
            data.lastRewardTime = InternetManager.Instance.GetCurrentDateTime().ToString();
            data.rewardCount = rewardCount;
            FileManager.Save(key, data);
        }
        else
        {
            data = savedData;
        }
    }

    [System.Serializable]
    public class Data
    {
        public string lastRewardTime;
        public int rewardCount;
    }
}
