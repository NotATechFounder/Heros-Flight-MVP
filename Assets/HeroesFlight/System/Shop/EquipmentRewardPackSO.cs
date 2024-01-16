using HeroesFlight.Common.Enum;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Equipment Item Reward Pack", menuName = "Rewards/ Equipment Item Reward Pack", order = 1)]
public class EquipmentRewardPackSO : RewardPackSO
{
    [System.Serializable]
    public class ItemQuery
    {
        public Rarity rarity;
        [Range(0, 100)]
        public float chance = 50;
    }

    [SerializeField] private int numberOfRewards = 1;
    [SerializeField] private ItemQuery[] itemQueries;
    [SerializeField] private ItemDatabaseSO itemDatabase;

    public override List<Reward> GetReward()
    {
        List < Reward > rewardToGive = new List<Reward>();

        float totalChance = 0;

        foreach (ItemQuery itemReward in itemQueries)
        {
            totalChance += itemReward.chance;
        }

        for (int i = 0; i < numberOfRewards; i++)
        {
            float randomRoll = Random.Range(0f, totalChance);

            foreach (ItemQuery itemReward in itemQueries)
            {
                if (randomRoll <= itemReward.chance)
                {
                    rewardToGive.Add(new Reward(itemDatabase.GetRandomItem(ItemType.Equipment), 1, itemReward.rarity));
                    break;
                }
                else
                {
                    randomRoll -= itemReward.chance;
                }
            }
        }

        return rewardToGive;
    }
}