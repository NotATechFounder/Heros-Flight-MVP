using HeroesFlight.System.FileManager;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public const string WORLD_SAVE_KEY =  "WorldSave";

    [SerializeField] private WorldVisualSO[] worlds;
    [SerializeField] private WorldType selectedWorld;
    [Header("Data")]
    [SerializeField] private Data data;

    public WorldType SelectedWorld => selectedWorld;

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
        int maxUnlockedWorldIndex = Array.IndexOf(Enum.GetValues(typeof(WorldType)), data.LastUnlockedWorld);
        int worldIndex = Array.IndexOf(Enum.GetValues(typeof(WorldType)), worldType);
        return worldIndex <= maxUnlockedWorldIndex;
    }

    public void UnlockNext()
    {
        int maxUnlockedWorldIndex = Array.IndexOf(Enum.GetValues(typeof(WorldType)), data.LastUnlockedWorld);
        int nextWorldIndex = maxUnlockedWorldIndex + 1;
        if (nextWorldIndex < worlds.Length)
        {
            data.LastUnlockedWorld = (WorldType)Enum.GetValues(typeof(WorldType)).GetValue(nextWorldIndex);
        }
        Save();
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
            data.LastUnlockedWorld = WorldType.World1;
            data.worldInfoData = new List<WorldInfoData>();
        }
    }

    [Serializable]
    public class Data
    {
        public WorldType LastUnlockedWorld;
        public List<WorldInfoData> worldInfoData;
    }

    [Serializable]
    public class WorldInfoData
    {
        public WorldType worldType;
        public int maxLevelReached;
    }
}
