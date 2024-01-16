using System;
using System.Collections.Generic;
using HeroesFlight.System.ShrineSystem.Angel;

namespace HeroesFlight.System.ShrineSystem
{
    [Serializable]
    public class ShrineSaveData
    {
        public ShrineSaveData()
        {
            UnlockData.Add(new ShrineSaveDataEntry(ShrineNPCType.Blacksmith, false));
            UnlockData.Add(new ShrineSaveDataEntry(ShrineNPCType.AngelsGambit, false));
            UnlockData.Add(new ShrineSaveDataEntry(ShrineNPCType.ActiveAbilityReRoller, false));
            UnlockData.Add(new ShrineSaveDataEntry(ShrineNPCType.PassiveAbilityReRoller, false));
            UnlockData.Add(new ShrineSaveDataEntry(ShrineNPCType.HealingMagicRune, true));
        }

        public List<ShrineSaveDataEntry> UnlockData = new();

        public bool HasEntry(ShrineNPCType type)
        {
            foreach (var data in UnlockData)
            {
                if (data.NpcType == type)
                {
                    return true;
                }
            }

            return false;
        }

        public bool GetNpcState(ShrineNPCType type)
        {
            foreach (var data in UnlockData)
            {
                if (data.NpcType == type)
                {
                    return data.isUnlocked;
                }
            }
            return false;
        }

        public void UnlockNpc(ShrineNPCType type)
        {
            foreach (var data in UnlockData)
            {
                if (data.NpcType == type)
                {
                    data.isUnlocked=true;
                }
            }
        }
    }
}