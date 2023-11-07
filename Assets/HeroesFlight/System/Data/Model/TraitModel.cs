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
            bool isFeatUnlocked, bool isFeatUnlockable, Sprite visual, int baseValue,int currentValue)
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

        public string Id{ get; }
        public int BaseValue { get; }

        public int CurrentValue { get; }
        public int RequiredLvl{ get; }
        public string BlockingId{ get; }
        public int Tier{ get; }
        public int Slot{ get; }
        public int GoldCost{ get; }
        public bool IsFeatUnlocked{ get; }
        public bool IsFeatUnlockable{ get; }
        public Sprite Visual{ get; }
    }
}