using HeroesFlight.System.FileManager;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class EnergyManager : MonoBehaviour
{
    public enum TimeType { Hours, Minutes, Seconds }

    public const string SAVE_ID = "EnergyData";

    public event Action<string> OnEnergyTimerUpdated;

    [Header("Properties")]
    [SerializeField] TimeType timeType;
    [SerializeField] int maxEnergy = 20;
    [SerializeField] int restoreDuration;
    [SerializeField] EnergyData energyData;

    private DateTime nextEnergyTime;
    private bool isRestoring = false;
    private CurrencyManager currencyManager;

    public int MaxEnergy() => maxEnergy;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UseEnergy(5);
        }
    }

    public void Initialize(CurrencyManager currencyManager)
    {
        this.currencyManager = currencyManager;
        LoadDate();
        LoadAccumulatedEnergyOffline();
        StartCoroutine(RestoreEnergy());
    }

    private void LoadDate()
    {
        EnergyData newEnergyData = FileManager.Load<EnergyData>(SAVE_ID);
        if (newEnergyData == null)
        {
            energyData = new EnergyData();
            currencyManager.SetCurencyAmount(CurrencyKeys.Energy,maxEnergy);
        }
        else
        {
            energyData = newEnergyData ;
        }
        nextEnergyTime = StringToDate(energyData.nextEnergyTime);
    }

    public void SaveData()
    {
        energyData.maxEnergy = maxEnergy;
        energyData.nextEnergyTime = nextEnergyTime.ToString();
        FileManager.Save(SAVE_ID, energyData);
    }

    private void LoadAccumulatedEnergyOffline()
    {
        if (currencyManager.GetCurrencyAmount(CurrencyKeys.Energy) < maxEnergy)
        {
            DateTime lastTime = nextEnergyTime;
            TimeSpan time = InternetManager.Instance.GetCurrentDateTime() - lastTime;

            //Debug.Log( "Hour : " + time.Hours + " : " + "Minute : " + time.Minutes + " : " + "Second : " + time.Seconds);

            int ammountToAdd = timeType switch
            {
                TimeType.Hours => (int)time.TotalHours,
                TimeType.Minutes => (int)time.TotalMinutes,
                TimeType.Seconds => (int)time.TotalSeconds,
                _ => 0,
            };

            if (ammountToAdd > 0 && currencyManager.GetCurrencyAmount(CurrencyKeys.Energy) < maxEnergy)
            {
                currencyManager.AddCurrency(CurrencyKeys.Energy, ammountToAdd);
                if (currencyManager.GetCurrencyAmount(CurrencyKeys.Energy) > maxEnergy)
                {
                    currencyManager.SetCurencyAmount(CurrencyKeys.Energy, maxEnergy);
                    OnEnergyTimerUpdated?.Invoke("Full");
                } 
                UpdateNextEnergyTime();
            }
        }
    }

    private IEnumerator RestoreEnergy()
    {
        isRestoring = true;

        while (currencyManager.GetCurrencyAmount(CurrencyKeys.Energy) < maxEnergy)
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
            currencyManager.AddCurrency(CurrencyKeys.Energy, 1);
            UpdateNextEnergyTime();
        }
    }

    public bool UseEnergy(int amount)
    {
        if (currencyManager.GetCurrencyAmount(CurrencyKeys.Energy) >= amount)
        {
            currencyManager.ReduceCurency(CurrencyKeys.Energy, amount);

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
        OnEnergyTimerUpdated?.Invoke(timeValue);
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
