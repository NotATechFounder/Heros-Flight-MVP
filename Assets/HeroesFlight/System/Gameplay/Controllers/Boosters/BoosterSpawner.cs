using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterSpawner : MonoBehaviour
{
    [SerializeField] private List<BoosterSO> boosterSOList;

    public void SpawnBoostLoot(BoosterEffectType boosterEffectType)
    {
        foreach (var boosterSO in boosterSOList)
        {
            if (boosterSO.BoosterEffectType == boosterEffectType)
            {
                Instantiate(boosterSO, transform.position, Quaternion.identity);
            }
        }
    }
}
