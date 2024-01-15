using UnityEngine;

namespace HeroesFlight.System.Achievement_System.ProgressionUnlocks.UnlockRewards
{
    [CreateAssetMenu(fileName = "NPC Unlock", menuName = "Unlocks/NPC Unlock")]
    public class NPCUnlock : UnlockReward
    {
        [SerializeField] private ShrineNPCType npcType;
        public ShrineNPCType NPCType => npcType;
    }
}