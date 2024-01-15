using UnityEngine;

namespace HeroesFlight.System.Achievement_System.ProgressionUnlocks.UnlockRewards
{
    public abstract class UnlockReward : ScriptableObject
    {
        [SerializeField] private ProgressionUnlockType unlockType;
        public ProgressionUnlockType UnlockType => unlockType;
    }
}