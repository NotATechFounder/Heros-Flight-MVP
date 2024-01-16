using UnityEngine;

namespace HeroesFlight.System.Achievement_System.ProgressionUnlocks.UnlockRewards
{
    public abstract class UnlockReward : ScriptableObject
    {
        [SerializeField] private ProgressionUnlockType unlockType;
        [SerializeField] private Sprite rewardImage;
        public ProgressionUnlockType UnlockType => unlockType;
        public Sprite RewardImage => rewardImage;
    }
}