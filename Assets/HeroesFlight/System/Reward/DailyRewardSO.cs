using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DailyRewardSO", menuName = "Rewards/DailyRewardSO")]
public class DailyRewardSO : ScriptableObject
{
    [SerializeField] private GroupRewardSO[] dayRewards;

    public Reward[] GetRewards(int day)
    {
        return dayRewards[day].GetRewards;
    }
}
