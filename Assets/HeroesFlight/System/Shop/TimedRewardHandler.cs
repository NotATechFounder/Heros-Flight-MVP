using UnityEngine;

[System.Serializable]
public class TimedRewardHandler
{
    [SerializeField] private string key;
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

    public void SaveData()
    {
 
    }

    public void LoadData()
    {
 
    }

    [System.Serializable]
    public class Data
    {
        public string lastRewardTime;
        public int rewardCount;
    }
}
