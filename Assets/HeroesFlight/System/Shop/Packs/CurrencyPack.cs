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
        public Reward reward;
    }

    [SerializeField] private Content[] contents;

    public int GetCost(int index)
    {
        return contents[index].cost;
    }

    public Content GetContent(int index)
    {
        return contents[index];
    }
}
