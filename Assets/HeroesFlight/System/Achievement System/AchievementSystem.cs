using StansAssets.Foundation.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AchievementSystem : IAchievementSystemInterface
{
    public QuestRewardHandler questRewardHandler { get; private set; }

    private RewardSystemInterface rewardSystemInterface;

    public AchievementSystem(RewardSystemInterface rewardSystemInterface)
    {
        this.rewardSystemInterface = rewardSystemInterface;
    }

    public void Init(Scene scene = default, Action onComplete = null)
    {
        questRewardHandler = scene.GetComponent<QuestRewardHandler>();

        questRewardHandler.Load();
    }

    public void Reset()
    {

    }

    public void AddQuestProgress(QuestType questType, int amount)
    {
        if (questRewardHandler.CurrentQuest == null || questRewardHandler.CurrentQuest.GetQuestType() != questType) return;
        questRewardHandler.CurrentData.qP += amount;
        CheckIfRewardIsReady();
    }

    public void CheckIfRewardIsReady()
    {
        if (questRewardHandler.CurrentQuest.IsQuestCompleted(questRewardHandler.CurrentData.qP))
        {
            Debug.Log("Quest Completed");
        }
    }

    public void ClaimQuestReward()
    {
        if (!questRewardHandler.CurrentQuest.IsQuestCompleted(questRewardHandler.CurrentData.qP))
        {
            Debug.Log("Quest not completed");
            return;
        }
        questRewardHandler.RewardClaimed();

        rewardSystemInterface.ProcessRewards(questRewardHandler.CurrentQuest.GetQuestRewardPack().GetReward());
        // visualise reward
    }
}
