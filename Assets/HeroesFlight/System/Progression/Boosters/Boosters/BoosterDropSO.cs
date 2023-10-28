using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoosterDropSO", menuName = "Boost/BoosterDropSO", order = 2)]
public class BoosterDropSO : ScriptableObject
{
    [System.Serializable]
    public class BoosterToDrop
    {
        public BoosterSO boosterSO;

        [Range(0, 100)]
        public float dropChance;
    }
    
    [SerializeField] private List<BoosterToDrop> boosterToDropList;
    public List<BoosterSO> GetDrops()
    {
        List<BoosterSO> boosterSOList = new List<BoosterSO>();

        foreach (var boosterToDrop in boosterToDropList)
        {
            if (Random.Range(0, 100) <= boosterToDrop.dropChance)
            {
                boosterSOList.Add(boosterToDrop.boosterSO);
            }
        }
        return boosterSOList;
    }
}
