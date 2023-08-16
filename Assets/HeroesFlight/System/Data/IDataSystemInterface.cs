using HeroesFlight.System;
using System;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.System.FileManager.Rewards;
using UnityEngine;

public interface IDataSystemInterface : ISystemInterface
{
    public event Action<CurrencySO, bool> OnCurrencyChange;
    RewardsHandlerInterface RewardHandler { get; }
    public void ReduceCurency(string key, float amount);

    public void AddCurency(string key, float amount);

    public void SetCurencyAmount(string key, float amount);

    public float GetCurrencyAmount(string currencyKey);

    public void LoadCurrencies();

    public void SaveCurrencies();

  


}
