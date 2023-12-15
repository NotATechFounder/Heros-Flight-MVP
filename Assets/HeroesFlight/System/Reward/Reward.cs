using UnityEngine;
using System;
using HeroesFlight.Common.Enum;

[Serializable]
public class Reward
{
    [SerializeField] RewardBaseSO rewardObject;
    [SerializeField] private int amount = 1;

    [Header("Has Rarity?")]
    [SerializeField] public Rarity rarity;

    public int GetAmount()=> amount;
    public Rarity GetRarity() => rarity;

    public T GetRewardObject<T>() where T : RewardBaseSO
    {
        if (rewardObject is T)
        {
            return (T)rewardObject;
        }
        else
        {
            Debug.LogError($"RewardObject is not of type {typeof(T)}");
            return default;
        }
    }
}