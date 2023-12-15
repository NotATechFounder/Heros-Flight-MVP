using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GoldPackGroup
{
    [System.Serializable]
    public class GoldPack
    {
        public GoldPackType goldPackType;
        public int cost;
        public Reward reward;
    }

    [SerializeField] private GoldPack[] goldPacks;

    public GoldPack[] GetGoldPacks => goldPacks;

    public int GetCost(int index)
    {
        return goldPacks[index].cost;
    }

    public GoldPack GetContent(int index)
    {
        return goldPacks[index];
    }
}
