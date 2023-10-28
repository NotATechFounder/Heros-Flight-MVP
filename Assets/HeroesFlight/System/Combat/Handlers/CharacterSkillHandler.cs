using HeroesFlightProject.System.Combat.Controllers;

namespace HeroesFlight.System.Combat.Handlers
{
    public class CharacterSkillHandler
    {
        public CharacterSkillHandler(CharacterAbilityInterface mainAbility, ISkillControllerInterface offAbility)
        {
            CharacterUltimate = mainAbility;
            CharacterAbility = offAbility;
        }
        public CharacterAbilityInterface CharacterUltimate { get; }
        public ISkillControllerInterface CharacterAbility { get; }
    }
}