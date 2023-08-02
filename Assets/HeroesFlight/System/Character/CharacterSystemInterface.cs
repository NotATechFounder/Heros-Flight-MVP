namespace HeroesFlight.System.Character
{
    public interface CharacterSystemInterface : ISystemInterface
    {
        CharacterControllerInterface CurrentCharacter { get; }
        CharacterControllerInterface CreateCharacter();
        void SetCharacterControllerState(bool isEnabled);
        
        void ResetCharacter();
    }
}