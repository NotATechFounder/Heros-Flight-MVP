using HeroesFlight.System.FileManager;
using ScriptableObjectDatabase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Currency", menuName = "Shop/Currency")]
public class CurrencySO : ScriptableObject,IHasID
{
    [SerializeField] private string key;
    [SerializeField] private string currencyName;
    [SerializeField] private Sprite currencySprite;
    [SerializeField] private Gradient currencyGradient;
    [SerializeField] private Gradient currencySparkGradient;
    [SerializeField] private Color currencyColor;

    [SerializeField] private Data _currencyData;

    public string GetCurrencyName => currencyName;
    public Gradient GetGradient => currencyGradient;
    public Gradient GetSparkGradient => currencySparkGradient;
    public Color GetColor => currencyColor;
    public string GetKey => key;
    public Sprite GetSprite => currencySprite;
    public float GetCurrencyAmount => _currencyData.Amount;

    public void ReduceCurrency(float amount)
    {
        _currencyData.Amount -= amount;
        Save();
    }

    public void IncreaseCurrency(float amount)
    {
        _currencyData.Amount += amount;
        Save();
    }

    public void SetCurrency(float amount)
    {
        _currencyData.Amount = amount;
        Save();
    }

    public void Save() => FileManager.Save(key, _currencyData);

    public void Load()
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
