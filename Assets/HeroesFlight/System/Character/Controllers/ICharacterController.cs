using System;
using HeroesFlight.System.Character.Enum;
using UnityEngine;

namespace HeroesFlight.System.Character
{
    public interface ICharacterController
    {
        event Action<CharacterState> OnCharacterMoveStateChanged;
        bool IsFacingLeft { get; }
        Vector3 GetVelocity();
    }
}