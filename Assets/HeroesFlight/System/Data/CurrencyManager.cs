using System.Collections.Generic;
using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    public event Action<CurrencySO, bool> OnCurrencyChanged;
    [SerializeField] private List<CurrencySO> _currencies;

    private void Awake()
    {
        InitCurrencies();
        AssignCurrrencyChangeCallback();
    }

    public void ReduceCurency(string key, float amount)
    {
        CurrencySO currency = GetCurrecy(key);
        currency.ReduceCurrency(amount);
    }

    public void AddCurency(string key, float amount)
    {
        CurrencySO currency = GetCurrecy(key);
        currency.IncreaseCurrency(amount);
    }

    public void SetCurencyAmount(string key, float amount)
    {
        CurrencySO currency = GetCurrecy(key);
        currency.SetCurrency(amount);
    }

    public void AddCurrrencyChangeCallback(string currencyKey, Action<CurrencySO, bool> action)
    {
        _currencies.Find ((currency) => currency.GetKey == currencyKey).AssignCurrencyChangeCallback(action);
    }

    public void AssignCurrrencyChangeCallback()
    {
        _currencies.ForEach((currency) => currency.AssignCurrencyChangeCallback((currencySO, isIncrease) =>
        {
           // Debug.Log($"{currencySO.GetKey} has changed by {currencySO.GetCurrencyAmount} and isIncrease is {isIncrease}");
            OnCurrencyChanged?.Invoke(currencySO, isIncrease);
        }));
    }

    public void TriggerAllCurrencyChange()
    {
        _currencies.ForEach((currency) => OnCurrencyChanged?.Invoke(currency, true));
    }

    public float GetCurrencyAmount(string currencyKey)
    {
        CurrencySO currencySO = _currencies.Find((currency) => currency.GetKey == currencyKey);
        return currencySO == null ? 0 : currencySO.GetCurrencyAmount;
    }

    public CurrencySO GetCurrecy(string currencyKey) => _currencies.Find((currency) => currency.GetKey == currencyKey);

    public void InitCurrencies() => _currencies.ForEach(currency => currency.Init());

    public void LoadCurrencies() => _currencies.ForEach(currency => currency.Load());

    public void SaveCurrencies() => _currencies.ForEach(currency => currency.Save());
}
