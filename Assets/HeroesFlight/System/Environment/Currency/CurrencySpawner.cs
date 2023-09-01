using Pelumi.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CurrencySpawner : MonoBehaviour
{
    public Action<string, int> OnCollected;
    [SerializeField] private CurrencyDatabase currencyDatabase;
    [SerializeField] private CurrencyItem currencyPrefab;
    [SerializeField] private List<CurrencyItem> spawnedCurrencyItems = new List<CurrencyItem>();
    [SerializeField] private Transform playerTransfrom;

    public void SetPlayer(Transform playerTrans)
    {
        playerTransfrom = playerTrans;
    }

    public void SpawnAtPosition(string key, float amount, Vector3 position)
    {
        CurrencySO currency = currencyDatabase.GetItemSOByID(key);
        CurrencyItem currencyObj = ObjectPoolManager.SpawnObject(currencyPrefab, position, Quaternion.identity);
        currencyObj.Initialize(currency, ProcessCurrency, amount, playerTransfrom);
        spawnedCurrencyItems.Add(currencyObj);
    }

    public void ProcessCurrency(CurrencyItem currencyItem, float amount)
    {
        OnCollected?.Invoke(currencyItem.CurrencySO.GetKey, (int)amount);
        spawnedCurrencyItems.Remove(currencyItem);
    }
}
