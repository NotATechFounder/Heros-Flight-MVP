using HeroesFlight.System.Achievement_System.ProgressionUnlocks.UnlockRewards;
using UnityEngine;

namespace HeroesFlight.System.Achievement_System.ProgressionUnlocks
{
    [CreateAssetMenu(fileName = "Progression Unlock", menuName = "Unlocks/Progression Unlock")]
    public class ProgressionUnlock : ScriptableObject
    {
        [SerializeField] private string unlockId;
        [SerializeField] private QuestType objectiveType;
        [SerializeField] private int objectiveValue;
        [SerializeField] private WorldType targetWorld;
        [SerializeField] private UnlockReward unlockReward;

        public UnlockReward UnlockReward => unlockReward;

        public string UnlockId => unlockId;

        public QuestType ObjectiveType => objectiveType;

        public int ObjectiveValue => objectiveValue;

        public WorldType TargetWorld => targetWorld;
    }
}