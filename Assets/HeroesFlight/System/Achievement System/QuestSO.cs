using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest")]
public class QuestSO : ScriptableObject
{
    [SerializeField] private string questName;
    [SerializeField] private QuestType questType;
    [SerializeField] private int questGoal;
    [SerializeField] private RewardPackSO questRewardPack;
    [SerializeField] private int rewardAmount; 
    
    public string GetQuestName() => questName;
    public QuestType GetQuestType() => questType;
    public int GetQuestGoal() => questGoal;
    public RewardPackSO GetQuestRewardPack() => questRewardPack;
    public int GetRewardAmount() => rewardAmount;

    public bool IsQuestCompleted(int progress) => progress >= questGoal;
}
