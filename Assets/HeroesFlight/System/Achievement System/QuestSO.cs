using ScriptableObjectDatabase;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest")]
public class QuestSO : ScriptableObject, IHasID
{
    [Header("Core Quest Info")]
    [SerializeField] protected string ID;
    [SerializeField] protected string questInfo;
    [SerializeField] protected QuestType questType;
    [SerializeField] protected int questGoal;
    [SerializeField] protected RewardPackSO rewardPack;

    public string GetQuestInfo() => questInfo;
    public void SetQuestInfo (string questInfo) => this.questInfo = questInfo;  
    public QuestType GetQuestType() => questType;
    public int GetQuestGoal() => questGoal;
    public RewardPackSO GetQuestRewardPack() => rewardPack;
    public bool IsQuestCompleted(int progress) => progress >= questGoal;

    public float GetNormalizedProgress(int progress)
    {
        return (float)progress / (float)questGoal;
    }

    public string GetID()
    {
        return ID;
    }
}
