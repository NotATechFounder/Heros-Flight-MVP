using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RewardPackSO : ScriptableObject
{
    public abstract List<Reward> GetReward();
}
