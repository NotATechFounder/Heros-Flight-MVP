using System;
using System.Collections.Generic;
using HeroesFlight.System.Achievement_System.ProgressionUnlocks.UnlockRewards;
using UnityEngine;

namespace HeroesFlight.System.Achievement_System.ProgressionUnlocks
{
    public class ProgressionUnlocksHandler
    {
        public ProgressionUnlocksHandler(DataSystemInterface dataSystemInterface)
        {
            dataSystem = dataSystemInterface;
            LoadCache();
        }

        private DataSystemInterface dataSystem;

        public event Action<UnlockReward> OnRewardUnlocked;
        private const string LOAD_FOLDER = "Progression/";
        private const string SAVE_NAME = "Progression";
        private Dictionary<string, ProgressionUnlock> progressionMap = new();
        private ProgressionSaveData unlockData ;


        public void ProcessWorldProgression(WorldType world, int lvlFinished)
        {
            foreach (var unlock in progressionMap.Values)
            {
                if (unlock.ObjectiveType == QuestType.LevelCompletion && unlock.TargetWorld == world &&
                    unlock.ObjectiveValue == lvlFinished)
                {
                    Debug.Log($"Should unlock id {unlock.UnlockId}");
                    unlockData.UnlockedIds.Add(unlock.UnlockId);
                    Save();
                    OnRewardUnlocked?.Invoke(unlock.UnlockReward);
                }
            }
        }

        public void ProcessPlayerLvlReached(int newLVl)
        {
        }

        public void ProcessBossKilled(WorldType bossWorld)
        {
            switch (bossWorld)
            {
                case WorldType.World1:
                    // Code for processing boss killed in World1
                    break;
                case WorldType.World2:
                    // Code for processing boss killed in World2
                    break;
                case WorldType.World3:
                    // Code for processing boss killed in World3
                    break;
                default:
                    // Code for handling unknown bossWorld value
                    break;
            }
        }

        void LoadCache()
        {
            //Debug.Log("Loading cache");
            var availableCache = Resources.LoadAll<ProgressionUnlock>(LOAD_FOLDER);
            //Debug.Log(availableCache.Length);

           var savedData= FileManager.FileManager.Load<ProgressionSaveData>(SAVE_NAME);
           unlockData = savedData ?? new ProgressionSaveData();
           
            foreach (var progressionEntry in availableCache)
            {
                //Debug.Log($"adding feat with id {feat.Id}");
                if (!unlockData.UnlockedIds.Contains(progressionEntry.UnlockId))
                {
                    progressionMap.Add(progressionEntry.UnlockId, progressionEntry);
                }
              
            }

            Debug.Log(progressionMap.Count);
        }

        public void Save()
        {
            FileManager.FileManager.Save(SAVE_NAME,unlockData);
        }
    }
}