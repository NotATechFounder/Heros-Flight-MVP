using System;
using HeroesFlight.System.Character.Enum;
using HeroesFlight.System.Character.Model;
using UnityEngine;

namespace HeroesFlight.System.Character
{
    public interface CharacterControllerInterface
    {
        public CharacterData Data { get; }
        public Transform CharacterTransform { get; }
        event Action<CharacterState> OnCharacterMoveStateChanged;
        bool IsFacingLeft { get; }
        void Init();
        Vector3 GetVelocity();
        void SetActionState(bool isEnabled);
    }
}