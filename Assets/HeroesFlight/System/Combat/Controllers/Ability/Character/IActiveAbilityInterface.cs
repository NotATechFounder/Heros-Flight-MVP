namespace HeroesFlightProject.System.Combat.Controllers
{
    public interface IActiveAbilityInterface
    {
        public TimedAbilityController PassiveAbilityOneController { get; }
        public TimedAbilityController PassiveAbilityTwoController { get; }
        public TimedAbilityController PassiveAbilityThreeController { get; }
    }
}