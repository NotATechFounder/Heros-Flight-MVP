using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class CurrencyManager : MonoBehaviour
{
    public event Action<CurrencySO, bool> OnCurrencyChange;
    [SerializeField] private List<CurrencySO> _currencies;


    private void Awake()
    {
        LoadCurrencies();
    }

    public void ReduceCurency(string key, float amount)
    {
        CurrencySO currency = GetCurrecy(key);
        currency.ReduceCurrency(amount);
        OnCurrencyChange?.Invoke(currency, false);
    }

    public void AddCurency(string key, float amount)
    {
        CurrencySO currency = GetCurrecy(key);
        currency.IncreaseCurrency(amount);
        OnCurrencyChange?.Invoke(currency, false);
    }

    public void SetCurencyAmount(string key, float amount)
    {
        CurrencySO currency = GetCurrecy(key);
        currency.SetCurrency(amount);
        OnCurrencyChange?.Invoke(currency, false);
    }

    public float GetCurrencyAmount(string currencyKey)
    {
        CurrencySO currencySO = _currencies.Find((currency) => currency.GetKey == currencyKey);
        return currencySO == null ? 0 : currencySO.GetCurrencyAmount;
    }

    public CurrencySO GetCurrecy(string currencyKey) => _currencies.Find((currency) => currency.GetKey == currencyKey);
   
    public void LoadCurrencies() => _currencies.ForEach(currency => currency.Load());

    public void SaveCurrencies() => _currencies.ForEach(currency => currency.Save());
}
