namespace HeroesFlight.System.Character
{
    public interface CharacterSystemInterface : ISystemInterface
    {
        CharacterControllerInterface CreateCharacter();
        void SetCharacterControllerState(bool isEnabled);
      
    }
}