using System;
using UnityEngine;

namespace HeroesFlight.System.FileManager.Model
{
    [Serializable]
    public class TraitModel
    {
        public TraitModel()
        {
            Id = string.Empty;
        }
        public TraitModel(string id, int tier, int slot, int requiredLvl, string blockingId, int goldCost, 
            bool isFeatUnlocked,bool isFeatUnlockable,Sprite visual)
        {
            Id = id;
            Tier = tier;
            Slot = slot;
            RequiredLvl = requiredLvl;
            BlockingId = blockingId;
            GoldCost = goldCost;
            IsFeatUnlocked = isFeatUnlocked;
            IsFeatUnlockable = isFeatUnlockable;
            Visual = visual;
        }

        public string Id;
        public int RequiredLvl;
        public string BlockingId;
        public int Tier;
        public int Slot;
        public int GoldCost;
        public bool IsFeatUnlocked;
        public bool IsFeatUnlockable;
        public Sprite Visual;
    }
}