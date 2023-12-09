using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class ChancedReward
{
    [Range(0, 100)] public float chance = 50;
    public Reward reward;
}

[CreateAssetMenu(fileName = "New RewardPack", menuName = "Rewards/RewardPack")]
public class RewardPackSO : ScriptableObject
{
    [SerializeField] private Reward[] fixedRewards;
    [SerializeField] int numberChancedRewards;
    [SerializeField] private ChancedReward[] chancedRewards;

    private List<Reward> rewardToGive = new List<Reward>();

    public Reward[] GetFixedRewards() => fixedRewards;
    public List<Reward> GetRewardsToGive() => rewardToGive;

    public List<Reward> GetReward()
    {
        rewardToGive.Clear();
        GiveAllFixedRewards();
        GenerateRewardByChance();
        return rewardToGive;
    }

    public Reward GiveSingleReward(int index)
    {
        return fixedRewards[index];
    }

    private void GiveAllFixedRewards()
    {
        foreach (Reward reward in fixedRewards)
        {
            rewardToGive.Add(reward);
        }
    }

    private void GenerateRewardByChance()
    {
        GenerateRandomRewardByChance(chancedRewards, numberChancedRewards);
    }

    public void GenerateRandomRewardByChance(ChancedReward[] rewardArray, int currentNumberOfRewards)
    {
        float totalChance = 0;
        foreach (ChancedReward chancedReward in rewardArray)
        {
            totalChance += chancedReward.chance;
        }

        for (int i = 0; i < currentNumberOfRewards; i++)
        {
            float randomRoll = Random.Range(0f, totalChance);

            float pos = 0;
            foreach (ChancedReward chancedReward in rewardArray)
            {
                if (randomRoll < chancedReward.chance + pos)
                {
                    rewardToGive.Add(chancedReward.reward);
                    break;
                }
                pos += chancedReward.chance;
            }
        }
    }
}
