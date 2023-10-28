using UnityEngine;
using System;
using Object = UnityEngine.Object;

[Serializable]
public class Reward
{
    public enum RewardType { Currency, Item}

    [SerializeField] private RewardType rewardType;
    [SerializeField] [Range(0, 100)]  private float chance;
    [SerializeField] RewardObject rewardObject;
    [SerializeField] private int amount;

    public int GetAmount()=> amount;
    public RewardType GetRewardType() => rewardType;
    public float GetChance() => chance;

    public void GiveReward()
    {
        switch (rewardType)
        {
            case RewardType.Currency:

                if (rewardObject is CurrencySO)
                {
                    CurrencySO currency = (CurrencySO)rewardObject;
                    currency.IncreaseCurrency(amount);
                }

                break;
            case RewardType.Item:

                break;
            default: break;
        }
    }
}
