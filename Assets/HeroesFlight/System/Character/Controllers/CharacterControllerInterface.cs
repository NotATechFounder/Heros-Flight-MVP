using System;
using HeroesFlight.System.Character.Enum;
using UnityEngine;

namespace HeroesFlight.System.Character
{
    public interface CharacterControllerInterface
    {
        public CharacterSO CharacterSO { get; }
        public CharacterStatController CharacterStatController { get; }
        event Action<CharacterState> OnCharacterMoveStateChanged;
        bool IsFacingLeft { get; }
        Vector3 GetVelocity();
        void SetActionState(bool isDisabled);
    }
}