using System;
using System.Collections.Generic;

namespace HeroesFlight.System.ShrineSystem
{
    [Serializable]
    public class ShrineSaveData
    {
        public ShrineSaveData()
        {
            UnlockData.Add(ShrineNPCType.Blacksmith,false);
            UnlockData.Add(ShrineNPCType.AngelsGambit, false);
            UnlockData.Add(ShrineNPCType.ActiveAbilityReRoller, false);
            UnlockData.Add(ShrineNPCType.PassiveAbilityReRoller, false);
            UnlockData.Add(ShrineNPCType.HealingMagicRune, true);
        }
        public Dictionary<ShrineNPCType, bool> UnlockData = new Dictionary<ShrineNPCType, bool>();
    }
}