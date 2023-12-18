using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chest", menuName = "Shop/Chest", order = 1)]
public class ChestSO : ScriptableObject
{
    [SerializeField] private ChestType chestType;

    [TextArea(3, 10)]
    [SerializeField] private string chestInfo;
    [SerializeField] private RewardPackSO rewards;
    [SerializeField] int price;

    [Header("Timed Reward")]
    [SerializeField] TimeType timeType;
    [SerializeField] float nextRewardTimeAdded = 20f;
    [SerializeField] float checkingInterval = 2f;

    public ChestType GetChestType => chestType;
    public RewardPackSO GetRewardPack => rewards;
    public int GetPrice => price;
    public string GetChestInfo => chestInfo;

    public TimeType GetTimeType => timeType;
    public float GetNextRewardTimeAdded => nextRewardTimeAdded;
    public float GetCheckingInterval => checkingInterval;
}