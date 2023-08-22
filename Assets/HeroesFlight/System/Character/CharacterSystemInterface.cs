using System.Collections.Generic;
using HeroesFlight.System.Character.Enum;

namespace HeroesFlight.System.Character
{
    public interface CharacterSystemInterface : SystemInterface
    {
        CharacterControllerInterface CurrentCharacter { get; }
        CharacterControllerInterface CreateCharacter();

        void SetCurrentCharacterType(CharacterType currentType);
        void SetCharacterControllerState(bool isEnabled);
        
        void ResetCharacter();

        void UpdateUnlockedClasses(CharacterType typeToUnlock);

        List<CharacterType> GetUnlockedClasses();

    }
}