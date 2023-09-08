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
    [SerializeField] private List<CurrencyItem> spawnedExpItems = new List<CurrencyItem>();
    [SerializeField] private Transform playerTransfrom;

    WaitForSeconds delay = new WaitForSeconds(.05f);

    public void SetPlayer(Transform playerTrans)
    {
        playerTransfrom = playerTrans;
    }

    public void SpawnAtPosition(string key, float amount, Vector3 position)
    {
        CurrencySO currency = currencyDatabase.GetItemSOByID(key);

        bool isExp = currency.GetKey == CurrencyKeys.Experience;

        CurrencyItem currencyObj = ObjectPoolManager.SpawnObject(currencyPrefab, position, Quaternion.identity);
        currencyObj.Initialize(currency, ProcessCurrency, amount, playerTransfrom, !isExp);

        if (isExp) spawnedExpItems.Add(currencyObj);
    }

    public void ProcessCurrency(CurrencyItem currencyItem, float amount)
    {
        OnCollected?.Invoke(currencyItem.CurrencySO.GetKey, (int)amount);
    }

    public bool AllExpCollected() => spawnedExpItems.TrueForAll(item => item.IsCollected);

    public void ActivateExpItems(Action OnAllExpCollected)
    {
        StartCoroutine(ActivateExpItemsWithDelay(OnAllExpCollected));
    }

    private IEnumerator ActivateExpItemsWithDelay(Action OnAllExpCollected)
    {
        foreach (CurrencyItem item in spawnedExpItems)
        {
            item.MoveToPlayer();
            yield return delay;
        }

        yield return new WaitUntil(() => AllExpCollected());
        spawnedExpItems.Clear();
        yield return delay;
        OnAllExpCollected?.Invoke();
    }
}
