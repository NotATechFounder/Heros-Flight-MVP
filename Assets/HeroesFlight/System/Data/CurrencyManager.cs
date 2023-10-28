using System.Collections.Generic;
using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private List<CurrencySO> _currencies;

    private void Awake()
    {
        InitCurrencies();
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
