using Pelumi.ObjectPool;
using StansAssets.Foundation.Async;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoosterSpawner : MonoBehaviour
{
    [SerializeField] private BoosterDatabase boosterDatabase;
    [SerializeField] private BoosterItem boosterItemPrefab;
    [SerializeField] private float attackDropDelay = 30f;

    [SerializeField] private BoosterManager boosterManager;
    [SerializeField] private List<BoosterItem> spawnedBoosterItem;
    [SerializeField] private bool pauseAttackBooster;

    public void SpawnBoostLoot(BoosterDropSO boosterDropSO, Vector3 originPos)
    {
        List<BoosterSO> boosterSOList = boosterDropSO.GetDrops();
        Vector3 radomPos = originPos;
        foreach (var boosterSO in boosterSOList)
        {
            radomPos = new Vector3(originPos.x + UnityEngine.Random.Range(-1f, 1f), originPos.y + UnityEngine.Random.Range(-1f, 1f), originPos.z);
            SpawnBoostLoot(boosterSO.BoosterName, radomPos);
        }
    }

    public void SpawnBoostLoot(string name, Vector3 position)
    {
        BoosterSO boosterSO = boosterDatabase.GetItemSOByID(name);
        if (boosterSO != null)
        {
            if(boosterSO.BoosterEffectType == BoosterEffectType.Attack)
            {
                if(!pauseAttackBooster)
                {
                    pauseAttackBooster = true;
                    CoroutineUtility.WaitForSeconds(attackDropDelay, () => pauseAttackBooster = false);
                }
                else
                {
                    return;
                }
            }

            BoosterItem newBoosterItem = ObjectPoolManager.SpawnObject(boosterItemPrefab, position, Quaternion.identity);
            newBoosterItem.Initialize(boosterSO, OnBoosterItemInteracted);
            spawnedBoosterItem.Add(newBoosterItem);
        }
    }

    private bool OnBoosterItemInteracted(BoosterItem item)
    {
        if(boosterManager.ActivateBooster(item.BoosterSO))
        {
            if (item.BoosterSO.BoosterEffectType == BoosterEffectType.Attack)
            {
                pauseAttackBooster = false;
            }
            spawnedBoosterItem.Remove(item);
            return true;
        }
        return false;
    }

    public void ClearAllBoosters()
    {
        foreach (var boosterItem in spawnedBoosterItem)
        {
            ObjectPoolManager.ReleaseObject(boosterItem);
        }
        spawnedBoosterItem.Clear();
    }
}
