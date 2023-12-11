using HeroesFlight.System.FileManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestRewardHandler : MonoBehaviour
{
    public const string Save_ID = "QuestData";
    [SerializeField] private List<QuestSO> quests = new List<QuestSO>();
    [Header("Debug")]
    [SerializeField] private QuestSO currentQuest;
    [SerializeField] private Data currentData = new Data();

    public QuestSO CurrentQuest => currentQuest;
    public Data CurrentData => currentData;

    public void RewardClaimed()
    {
        currentData.index = (currentData.index + 1) % quests.Count;
        currentData.qP = 0;
        currentQuest = quests[currentData.index];
        Save();
    }

    public void Save()
    {
        FileManager.Save(Save_ID, currentData);
    }

    public void Load()
    {
        Data savedData = FileManager.Load<Data>(Save_ID);
        currentData = savedData ?? new Data();
        currentQuest = quests[currentData.index >= quests.Count ? 0 : currentData.index];
    }

    [Serializable]
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
