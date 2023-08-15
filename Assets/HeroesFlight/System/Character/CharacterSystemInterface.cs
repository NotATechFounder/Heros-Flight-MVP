using HeroesFlight.System.Character.Enum;

namespace HeroesFlight.System.Character
{
    public interface CharacterSystemInterface : ISystemInterface
    {
        CharacterControllerInterface CurrentCharacter { get; }
        CharacterControllerInterface CreateCharacter();

        void SetCurrentCharacterType(CharacterType currentType);
        void SetCharacterControllerState(bool isEnabled);
        
        void ResetCharacter();
    }
}