using System;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class Drop 
{
    [Range(0, 100)]
    public float dropRate;
    public int minAmount;
    public int maxAmount;

    public bool DropSuccessFul(Action<int> OnDropSuccessFul)
    {
        bool shouldDrop = Random.Range(0f, 100f) <= dropRate;
        if (shouldDrop)
        {
            OnDropSuccessFul?.Invoke(Random.Range(minAmount, maxAmount));
            return shouldDrop;
        }
        return false;
    }
}