using UnityEngine;

[System.Serializable]
public class GroupRewardSO
{
    [SerializeField] private Reward[] rewards;
    public Reward[] GetRewards => rewards;  
}
