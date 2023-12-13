using HeroesFlight.System.FileManager;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public const string WORLD_SAVE_KEY = "WorldSave";

    [SerializeField] private WorldVisualSO[] worlds;
    [SerializeField] private WorldType selectedWorld;

   [SerializeField] private Data data;
    public WorldVisualSO[] Worlds => worlds;
    public WorldType SelectedWorld => selectedWorld;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        Load();
    }

    public void SetSelectedWorld(WorldType worldType)
    {
        selectedWorld = worldType;
    }

    public bool IsWorldUnlocked(WorldType worldType)
    {
        WorldInfoData targetEntry = null;
        foreach (var infoEntry in data.worldInfoData)
        {
            if (infoEntry.worldType == worldType)
            {
                targetEntry = infoEntry;
                break;
            }
        }

        if (targetEntry != null)
        {
            return targetEntry.isUnlocked;
        }

        return false;
        // int maxUnlockedWorldIndex = Array.IndexOf(Enum.GetValues(typeof(WorldType)), data.LastUnlockedWorld);
        // int worldIndex = Array.IndexOf(Enum.GetValues(typeof(WorldType)), worldType);
        // return worldIndex <= maxUnlockedWorldIndex;
    }

    public void UnlockWorld(WorldType typeToUnlock)
    {
        data.worldInfoData.Find(x => x.worldType == typeToUnlock).isUnlocked = true;
        Save();
    }
    // public void UnlockNext()
    // {
    //     int maxUnlockedWorldIndex = Array.IndexOf(Enum.GetValues(typeof(WorldType)), data.LastUnlockedWorld);
    //     int nextWorldIndex = maxUnlockedWorldIndex + 1;
    //     if (nextWorldIndex < worlds.Length)
    //     {
    //         data.LastUnlockedWorld = (WorldType)Enum.GetValues(typeof(WorldType)).GetValue(nextWorldIndex);
    //     }
    //
    //     Save();
    // }

    public int GetMaxLevelReached(WorldType worldType)
    {
        WorldInfoData worldInfoData = data.worldInfoData.Find(x => x.worldType == worldType);
        if (worldInfoData != null)
        {
            return worldInfoData.maxLevelReached;
        }
        else
        {
            return 0;
        }
    }

    public void SetMaxLevelReached(WorldType worldType, int levelReached)
    {
        WorldInfoData worldInfoData = data.worldInfoData.Find(x => x.worldType == worldType);

        if (worldInfoData != null)
        {
            if (levelReached < worldInfoData.maxLevelReached)
                return;
            worldInfoData.maxLevelReached = levelReached;
        }
        else
        {
            worldInfoData = new WorldInfoData();
            worldInfoData.worldType = worldType;
            worldInfoData.maxLevelReached = levelReached;
            data.worldInfoData.Add(worldInfoData);
        }

        Save();
    }


    public void Save()
    {
        FileManager.Save(WORLD_SAVE_KEY, data);
    }

    public void Load()
    {
        Data savedData = FileManager.Load<Data>(WORLD_SAVE_KEY);
        if (savedData != null)
        {
            data = savedData;
        }
        else
        {
            data = new Data();
           
        }
    }

    [Serializable]
    public class Data
    {
        public Data()
        {
            worldInfoData = new List<WorldInfoData>();
            foreach (WorldType world in Enum.GetValues(typeof(WorldType)))
            {
                var infoEntry = new WorldInfoData 
                {
                    worldType = world,
                    maxLevelReached = 0,
                    isUnlocked = world != WorldType.World3
                };

                worldInfoData.Add(infoEntry);
            }
        }

        public List<WorldInfoData> worldInfoData;
    }

    [Serializable]
    public class WorldInfoData
    {
        public WorldType worldType;
        public int maxLevelReached;
        public bool isUnlocked;
    }
}