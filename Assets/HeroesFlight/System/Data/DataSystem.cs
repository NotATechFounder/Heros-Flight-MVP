using StansAssets.Foundation.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataSystem : IDataSystemInterface
{
    CurrencyManager currencyManager;

    public event Action<CurrencySO, bool> OnCurrencyChange;

    public void Init(Scene scene = default, Action onComplete = null)
    {
        currencyManager = scene.GetComponent<CurrencyManager>();
        currencyManager.OnCurrencyChange += OnCurrencyChange;
        LoadCurrencies();
        onComplete?.Invoke();
    }

    public void Reset()
    {
        currencyManager.OnCurrencyChange -= OnCurrencyChange;
    }

    public void AddCurency(string key, float amount)
    {
        currencyManager.AddCurency(key, amount);
    }

    public float GetCurrencyAmount(string currencyKey)
    {
        return currencyManager.GetCurrencyAmount(currencyKey);
    }

    public void ReduceCurency(string key, float amount)
    {
        currencyManager.ReduceCurency(key, amount);
    }

    public void SetCurencyAmount(string key, float amount)
    {
        currencyManager.SetCurencyAmount(key, amount);
    }

    public void SaveCurrencies()
    {
        currencyManager.SaveCurrencies();
    }

    public void LoadCurrencies()
    {
        currencyManager.LoadCurrencies();
    }
}
