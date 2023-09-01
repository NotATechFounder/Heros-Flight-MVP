using HeroesFlightProject.System.Gameplay.Controllers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    [SerializeField] BoosterDropSO boosterDropSO;

    [Header("Gold")]
    [SerializeField] RangeValue goldRange;
    [SerializeField] int goldInBatch = 10;

    int goldAmount;
    BoosterSpawner boosterSpawner;
    CurrencySpawner currencySpawner;

    public void Initialize(BoosterSpawner boosterSpawner, CurrencySpawner currencySpawner)
    {
        this.boosterSpawner = boosterSpawner;
        this.currencySpawner = currencySpawner;
    }

    public void DropReward()
    {
        boosterSpawner.SpawnBoostLoot(boosterDropSO, transform.position);

        goldAmount = Mathf.RoundToInt(goldRange.GetRandomValue()) / goldInBatch;
        for (int i = 0; i < goldInBatch; i++)
        {
            currencySpawner.SpawnAtPosition(CurrencyKeys.Gold, goldAmount, transform.position);
        }
    }
}
