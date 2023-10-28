using HeroesFlight.System.Combat.Enum;
using HeroesFlightProject.System.Gameplay.Controllers;

namespace HeroesFlight.System.Combat.Model
{
    public class CombatEntityModel
    {
        public CombatEntityModel(IHealthController healthController, IAttackControllerInterface attackController,
            CombatEntityType entityType)
        {
            HealthController = healthController;
            AttackController = attackController;
            EntityType = entityType;
        }

        public CombatEntityModel(IHealthController healthController, CombatEntityType entityType)
        {
            HealthController = healthController;
            AttackController = null;
            EntityType = entityType;
        }
        public IHealthController HealthController { get; }
        public IAttackControllerInterface AttackController { get; }
        public CombatEntityType EntityType { get; }
    }
}