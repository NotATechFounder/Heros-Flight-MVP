namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface IAttackControllerInterface
    {
        int Damage { get; }
        float TimeSinceLastAttack { get; }
        void AttackTargets();
        void Init();
        void ToggleControllerState(bool isEnabled);
    }
}