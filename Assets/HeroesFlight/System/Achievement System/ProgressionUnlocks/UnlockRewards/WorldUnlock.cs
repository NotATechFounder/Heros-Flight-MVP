using UnityEngine;

namespace HeroesFlight.System.Achievement_System.ProgressionUnlocks.UnlockRewards
{
    [CreateAssetMenu(fileName = "World Unlock", menuName = "Unlocks/World Unlock")]
    public class WorldUnlock : UnlockReward
    {
        [SerializeField] private WorldType targetWorld;
        public WorldType TargetWorld => targetWorld;
    }
}