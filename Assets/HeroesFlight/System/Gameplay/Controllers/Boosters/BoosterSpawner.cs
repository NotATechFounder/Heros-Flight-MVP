using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoosterSpawner : MonoBehaviour
{
    [SerializeField] private List<BoosterSO> boosterSOList;
    [SerializeField] private BoosterItem boosterItem;
    [SerializeField] private BoosterManager boosterManager;
    [SerializeField] private List<BoosterItem> spawnedBoosterItem;

    public void SpawnBoostLoot(BoosterEffectType boosterEffectType)
    {
        foreach (var boosterSO in boosterSOList)
        {
            if (boosterSO.BoosterEffectType == boosterEffectType)
            {
                BoosterItem newBoosterItem = Instantiate(boosterItem, transform.position, Quaternion.identity);
                newBoosterItem.Initialize(boosterSO, OnBoosterItemInteracted);
                spawnedBoosterItem.Add(newBoosterItem);
            }
        }
    }

    public void SpawnBoostLoot(string name, Vector3 position)
    {
        BoosterSO boosterSO = boosterSOList.Find(x => x.BoosterName == name);
        if (boosterSO != null)
        {
            BoosterItem newBoosterItem = Instantiate(boosterItem, position, Quaternion.identity);
            newBoosterItem.Initialize(boosterSO, OnBoosterItemInteracted);
            spawnedBoosterItem.Add(newBoosterItem);
        }
    }

    private bool OnBoosterItemInteracted(BoosterItem item)
    {
        if(boosterManager.ActivateBooster(item.BoosterSO))
        {
            spawnedBoosterItem.Remove(item);
            return true;
        }
        return false;
    }
}
