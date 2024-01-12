using System.Collections.Generic;
using UnityEngine;
using System;

public class Chest : MonoBehaviour
{
    [SerializeField] private ChestSO chestSO;
    public ChestType GetChestType => chestSO.GetChestType;
    public RewardPackSO GetRewards => chestSO.GetRewardPack;
    public int GetGemChestPrice => chestSO.GetPrice;
    public string GetChestInfo => chestSO.GetChestInfo;

    public List<Reward> OpenChest()
    {
       return chestSO.GetRewardPack.GetReward();
    }
}
