using HeroesFlight.Common.Enum;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeroesFlight.System.Achievement_System.ProgressionUnlocks.UnlockRewards
{
    [CreateAssetMenu(fileName = "Character Unlock", menuName = "Unlocks/Character Unlock")]
    public class CharacterUnlock : UnlockReward
    {
        [SerializeField] private CharacterType characterType;
        public CharacterType CharacterCharacterType => characterType;
    }
}