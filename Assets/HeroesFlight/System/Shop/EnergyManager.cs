using HeroesFlight.System.FileManager;
using System;
using System.Collections;
using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    public enum TimeType { Hours, Minutes, Seconds }

    [SerializeField] string saveName;

    [Header("Properties")]
    [SerializeField] TimeType timeType;
    [SerializeField] int currentEnergy;
    [SerializeField] int maxEnergy = 20;
    [SerializeField] int restoreDuration;
    [SerializeField] EnergyData energyData;

    private DateTime nextEnergyTime;
    bool isRestoring = false;

    public int MaxEnergy() => maxEnergy;

    private void Start()
    {
        LoadDate();
        LoadAccumulatedEnergyOffline();
        StartCoroutine(RestoreEnergy());
    }

    private void LoadDate()
    {
        EnergyData newEnergyData = FileManager.Load<EnergyData>(saveName);
        energyData = newEnergyData ?? new EnergyData();
        nextEnergyTime = StringToDate(energyData.nextEnergyTime);
    }

    public void SaveData()
    {
        energyData.maxEnergy = maxEnergy;
        energyData.nextEnergyTime = nextEnergyTime.ToString();
       // FileManager.Save(saveName, energyData);
    }

    private void LoadAccumulatedEnergyOffline()
    {
        if (currentEnergy < maxEnergy)
        {
            DateTime lastTime = nextEnergyTime;
            TimeSpan time = InternetManager.Instance.GetCurrentDateTime() - lastTime;

            Debug.Log( "Hour : " + time.Hours + " : " + "Minute : " + time.Minutes + " : " + "Second : " + time.Seconds);

            int ammountToAdd = timeType switch
            {
                TimeType.Hours => (int)time.TotalHours,
                TimeType.Minutes => (int)time.TotalMinutes,
                TimeType.Seconds => (int)time.TotalSeconds,
                _ => 0,
            };

            if (ammountToAdd > 0 && currentEnergy < maxEnergy)
            {
                currentEnergy += ammountToAdd;
                if (currentEnergy > maxEnergy) currentEnergy = maxEnergy;
                UpdateNextEnergyTime();
            }
        }
    }

    private IEnumerator RestoreEnergy()
    {
        isRestoring = true;

        while (currentEnergy < maxEnergy)
        {
            TimeSpan time = nextEnergyTime - InternetManager.Instance.GetCurrentDateTime();

            UpdateTimer(time);

            UpdateEnergyTimerText(time);

            yield return new WaitForSeconds(1f);
        }

        isRestoring = false;
    }

    private void UpdateTimer(TimeSpan time)
    {
        bool isEnergyRestored = timeType switch
        {
            TimeType.Hours => time.TotalHours <= 0,
            TimeType.Minutes => time.TotalMinutes <= 0,
            TimeType.Seconds => time.TotalSeconds <= 0,
            _ => false,
        };

        if (isEnergyRestored)
        {
            currentEnergy++;
            UpdateNextEnergyTime();
        }
    }

    public bool UseEnergy(int amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;

            if (!isRestoring)
            {
                UpdateNextEnergyTime();
                StartCoroutine(RestoreEnergy());
            }
            return true;
        }
        else return false;
    }

    void UpdateNextEnergyTime()
    {
        nextEnergyTime = AddDuration(InternetManager.Instance.GetCurrentDateTime(), restoreDuration);
        energyData.nextEnergyTime = nextEnergyTime.ToString();
    }

    private void UpdateEnergyTimerText(TimeSpan time)
    {
        string timeValue = String.Format("{0:D2} : {1:D1}", time.Minutes, time.Seconds);
        Debug.Log(timeValue);
    }

    private DateTime AddDuration(DateTime dateTime, int duration)
    {
        DateTime Duration = timeType switch
        {
            TimeType.Hours => dateTime.AddHours(duration),
            TimeType.Minutes => dateTime.AddMinutes(duration),
            TimeType.Seconds => dateTime.AddSeconds(duration),
            _ => dateTime,
        };
        return Duration;
    }

    private DateTime StringToDate(string dateTime)
    {
        if (string.IsNullOrEmpty(dateTime)) return InternetManager.Instance.GetCurrentDateTime();
        else return DateTime.Parse(dateTime);
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    [Serializable]
    public class EnergyData
    {
        public int maxEnergy;
        public string nextEnergyTime;
    }
}
