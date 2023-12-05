using HeroesFlight.System.FileManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestRewardSystem : MonoBehaviour
{
    public const string Save_ID = "QuestData";
    [SerializeField] private List<QuestSO> quests = new List<QuestSO>();
    [SerializeField] private QuestSO currentQuest;
    [SerializeField] private Data currentData = new Data();

    public void Initialize()
    {
        Load();
        if (currentData.index >= quests.Count) return;  
        currentQuest = quests[currentData.index];
    }


    public void AddQuestProgress(QuestType questType, int amount)
    {
        if (currentQuest == null || currentQuest.GetQuestType() != questType) return;
        currentData.qP += amount;
    }

    public void ClaimQuestReward()
    {
        if (currentQuest == null || !currentQuest.IsQuestCompleted(currentData.qP)) return;
        currentData.index++;
        currentData.qP = 0;
        currentQuest = quests[currentData.index];

        // add reward
    }

    public void Save()
    {
        FileManager.Save(Save_ID, currentData);
    }

    public void Load()
    {
        Data savedData = FileManager.Load<Data>(Save_ID);
        currentData = savedData ?? new Data();

    }

    [System.Serializable]
    public class Data
    {
        public int index; // quest index
        public QuestType qT; // quest type
        public int qP; // quest progress
    }
}

public enum QuestType
{
    KillMobs,
    ReachLevel
}
