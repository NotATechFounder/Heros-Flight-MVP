using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private List<CurrencySO> _currencies;
    [SerializeField] private event Action<CurrencySO, bool> _onCurrencyChange;

    private void Awake()
    {
        LoadCurrencies();
    }

    public void ReduceCurency(string key, int amount)
    {
        CurrencySO currency = GetCurrecy(key);
        currency.ReduceCurrency(amount);
        _onCurrencyChange?.Invoke(currency, false);
    }

    public void AddCurency(string key, int amount)
    {
        CurrencySO currency = GetCurrecy(key);
        currency.IncreaseCurrency(amount);
        _onCurrencyChange?.Invoke(currency, false);
    }

    public void SetCurencyAmount(string key, int amount)
    {
        CurrencySO currency = GetCurrecy(key);
        currency.SetCurrency(amount);
        _onCurrencyChange?.Invoke(currency, false);
    }

    public int GetCurrencyAmount(string currencyKey) => _currencies.Find((currency) => currency.GetKey == currencyKey).GetCurrencyAmount;

    public CurrencySO GetCurrecy(string currencyKey) => _currencies.Find((currency) => currency.GetKey == currencyKey);
   
    public void LoadCurrencies() => _currencies.ForEach(currency => currency.Load());

    public void SaveCurrencies() => _currencies.ForEach(currency => currency.Save());
}
