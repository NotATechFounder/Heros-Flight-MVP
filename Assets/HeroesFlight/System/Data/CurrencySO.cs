using HeroesFlight.System.FileManager;
using ScriptableObjectDatabase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Currency", menuName = "Shop/Currency")]
public class CurrencySO : RewardObject, IHasID
{
    public enum CurrencyType
    {
        Persistent,
        NonPersistent
    }

    [Header("Currency Settings")]
    [SerializeField] private CurrencyType currencyType;
    [SerializeField] private string key;
    [SerializeField] private string currencyName;

    [Header("Currency Visual")]
    [SerializeField] private Sprite currencySprite;
    [SerializeField] private Gradient currencyGradient;
    [SerializeField] private Gradient currencySparkGradient;
    [SerializeField] private Color currencyColor;

    [Header("Currency Data")]
    [SerializeField] private Data _currencyData;

    public string GetCurrencyName => currencyName;
    public Gradient GetGradient => currencyGradient;
    public Gradient GetSparkGradient => currencySparkGradient;
    public Color GetColor => currencyColor;
    public string GetKey => key;
    public Sprite GetSprite => currencySprite;
    public float GetCurrencyAmount => _currencyData.Amount;

    private event Action<CurrencySO, bool> OnCurrencyChange;

    public void Init()
    {
        OnCurrencyChange = null;
        Load();
    }

    public void AssignCurrencyChangeCallback(Action<CurrencySO, bool> _OnCurrencyChange)
    {
        OnCurrencyChange += _OnCurrencyChange;
    }

    public void ReduceCurrency(float amount, bool notify = true)
    {
        _currencyData.Amount -= amount;
        Save();
        if (notify) OnCurrencyChange?.Invoke(this, false);
    }

    public void IncreaseCurrency(float amount, bool notify = true)
    {
        _currencyData.Amount += amount;
        Save();
        if (notify) OnCurrencyChange?.Invoke(this, true);
    }

    public void SetCurrency(float amount, bool notify = true)
    {
        _currencyData.Amount = amount;
        Save();
        if (notify) OnCurrencyChange?.Invoke(this, true);
    }

    public void Save() => FileManager.Save(key, _currencyData);

    public void Load()
    {
        switch (currencyType)
        {
            case CurrencyType.Persistent:
                LoadPersistent();
                break;
            case CurrencyType.NonPersistent:
                LoadNonPersistent();
                break;
        }
    }

    private void LoadNonPersistent()
    {
        _currencyData = new Data(this);
    }

    private void LoadPersistent()
    {
        Data savedCurrencyData = FileManager.Load<Data>(key);
        _currencyData = savedCurrencyData != null ? savedCurrencyData : new Data(this);
    }

    public string GetID()
    {
        return key;
    }

    [System.Serializable]
    public class Data
    {
        public string key;
        public float Amount = 0;

        public Data(CurrencySO currency) { key = currency.key; }
    }
}
