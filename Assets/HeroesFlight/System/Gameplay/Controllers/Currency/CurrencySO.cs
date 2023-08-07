using HeroesFlight.System.FileManager;
using ScriptableObjectDatabase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Currency", menuName = "Shop/Currency")]
public class CurrencySO : ScriptableObject,IHasID
{
    [SerializeField] private string key;
    [SerializeField] private Sprite Icon;
    [SerializeField] private Data _currencyData;

    public string GetKey => key;
    public Sprite GetIcon => Icon;
    public int GetCurrencyAmount => _currencyData.Amount;

    public void ReduceCurrency(int amount)
    {
        _currencyData.Amount -= amount;
        Save();
    }

    public void IncreaseCurrency(int amount)
    {
        _currencyData.Amount += amount;
        Save();
    }

    public void SetCurrency(int amount)
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
        public int Amount = 0;

        public Data(CurrencySO currency) { key = currency.key; }
    }
}
