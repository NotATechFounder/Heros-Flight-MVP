using System;
using StansAssets.Foundation.Extensions;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.ShrineSystem
{
    public class ShrineSystem : ShrineSystemInterface
    {
        public ShrineSystem()
        {
            Load();
        }

        private Shrine shrine;
        private ShrineSaveData saveData;
        private const string NPC = "NPC";
        public Shrine Shrine => shrine;


        public void Init(Scene scene = default, Action onComplete = null)
        {
            shrine = scene.GetComponentInChildren<Shrine>();
            shrine.InitNpcStates(saveData);
        }

        public void Reset()
        {
        }


        public void UnlockNpc(ShrineNPCType npcType)
        {
            saveData.UnlockNpc(npcType);
            shrine.UnlockNpc(npcType);
            Save();
        }


        private void Load()
        {
            var data = FileManager.FileManager.Load<ShrineSaveData>(NPC);
            saveData = data == null ? new ShrineSaveData() : data;
        }

        void Save()
        {
            FileManager.FileManager.Save(NPC, saveData);
        }
    }
}