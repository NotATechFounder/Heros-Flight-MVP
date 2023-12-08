using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CurrencyPack
{
    [System.Serializable]
    public class Content
    {
        public string name;
        public int cost;
        public int amount;
    }

    [SerializeField] private Content[] contents;

    public int GetCost(int index)
    {
        return contents[index].cost;
    }

    public int GetAmount(int index)
    {
        return contents[index].amount;
    }
}
