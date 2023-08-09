using HeroesFlight.System.Gameplay;
using Pelumi.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CurrencySpawner : MonoBehaviour
{
    [SerializeField] private CurrencyDatabase currencyDatabase;
    [SerializeField] private CurrencyItem currencyPrefab;
    [SerializeField] private List<CurrencyItem> spawnedCurrencyItems = new List<CurrencyItem>();
    [SerializeField] private GamePlaySystemInterface gamePlaySystem;
    [SerializeField] private Transform playerTransfrom;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 10; i++)
          //  SpawnGoldAtPosition(100, transform.position);
            SpawnAtPosition(CurrencyKeys.Experience, 100, transform.position);
        }
    }

    public void Initialize(GamePlaySystemInterface gamePlaySystemInterface)
    {
        gamePlaySystem = gamePlaySystemInterface;
    }

    public void SetPlayer(Transform playerTrans)
    {
        playerTransfrom = playerTrans;
    }

    public void SpawnGoldAtPosition(float amount, Vector3 position)
    {
        SpawnAtPosition(CurrencyKeys.Gold, amount, position);
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
        switch (currencyItem.CurrencySO.GetKey)
        {
            case CurrencyKeys.Gold:
                gamePlaySystem.AddGold((int)amount);
                break;
            case CurrencyKeys.Experience:
                gamePlaySystem.AddExperience((int)amount);
                break;
        }

        spawnedCurrencyItems.Remove(currencyItem);
    }

    public void ActiveAllLoot()
    {
        spawnedCurrencyItems.ForEach(currency => currency.MoveToPlayer());
    }
}
