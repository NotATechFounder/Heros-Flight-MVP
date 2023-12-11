using HeroesFlight.System.FileManager;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestRewardHandler : MonoBehaviour
{
    public const string Save_ID = "QuestData";
    [SerializeField] private QuestDataBase questDataBase;

    [Header("Debug")]
    [SerializeField] private QuestSO currentQuest;
    [SerializeField] private Data currentData = new Data();

    public QuestSO CurrentQuest => currentQuest;
    public Data CurrentData => currentData;

    public void RewardClaimed()
    {
        currentData.index = (currentData.index + 1) % questDataBase.Items.Length;
        currentData.qP = 0;
        currentQuest = questDataBase.GetItemSOByID(currentData.index.ToString());
        Save();
    }

    public void Save()
    {
        if (currentData.index >= questDataBase.Items.Length)
        {
            currentData.index = 1;
        }
        FileManager.Save(Save_ID, currentData);
    }

    public void Load()
    {
        Data savedData = FileManager.Load<Data>(Save_ID);
        currentData = savedData ?? new Data();
        currentQuest = questDataBase.GetItemSOByID((currentData.index >= questDataBase.Items.Length ? 1 : currentData.index).ToString());
    }

    [Serializable]
    public class Data
    {
        public int index = 1; // quest index
        public QuestType qT; // quest type
        public int qP; // quest progress
    }
}
