using System;
using HeroesFlight.System.Character.Enum;
using UnityEngine;

namespace HeroesFlight.System.Character
{
    public interface CharacterControllerInterface
    {
        public event Action<bool> OnFaceDirectionChange;

        public Transform CharacterTransform { get; }
        public CharacterSO CharacterSO { get; }
        public CharacterStatController CharacterStatController { get; }
        event Action<CharacterState> OnCharacterMoveStateChanged;
        bool IsFacingLeft { get; }
        void Init();
        Vector3 GetVelocity();
        void SetActionState(bool isEnabled);
    }
}