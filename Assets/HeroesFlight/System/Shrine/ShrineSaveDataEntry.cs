using System;

namespace HeroesFlight.System.ShrineSystem.Angel
{
    [Serializable]
    public class ShrineSaveDataEntry
    {
        public ShrineNPCType NpcType;
        public bool isUnlocked;

        public ShrineSaveDataEntry(ShrineNPCType npcType, bool isUnlocked)
        {
            NpcType = npcType;
            this.isUnlocked = isUnlocked;
          
        }
    }
}