using System.Collections.Generic;
using HeroesFlight.System.Character.Enum;
using UnityEngine;

namespace HeroesFlight.System.Character
{
    public interface CharacterSystemInterface : SystemInterface
    {
        CharacterControllerInterface CurrentCharacter { get; }
        CharacterControllerInterface CreateCharacter(Vector2 position);

        void SetCurrentCharacterType(CharacterType currentType);
        void SetCharacterControllerState(bool isEnabled);
        
        void ResetCharacter(Vector2 position);

        void UpdateUnlockedClasses(CharacterType typeToUnlock);

        List<CharacterType> GetUnlockedClasses();

    }
}