namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface IAttackControllerInterface
    {
        float Damage { get; }
        float TimeSinceLastAttack { get; }
        void AttackTargets();
        void Init();
        void DisableActions();
    }
}