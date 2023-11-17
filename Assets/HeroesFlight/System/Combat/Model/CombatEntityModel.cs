using HeroesFlight.System.Combat.Effects;
using HeroesFlight.System.Combat.Enum;
using HeroesFlightProject.System.Gameplay.Controllers;

namespace HeroesFlight.System.Combat.Model
{
    public class CombatEntityModel
    {
        public CombatEntityModel(IHealthController healthController, IAttackControllerInterface attackController,
            CombatEntityEffectsHandlerInterface effectHandler,
            CombatEntityType entityType)
        {
            HealthController = healthController;
            AttackController = attackController;
            EffectHandler = effectHandler;
            EntityType = entityType;
        }

        public CombatEntityModel(IHealthController healthController, CombatEntityEffectsHandlerInterface effectHandler,
            CombatEntityType entityType)
        {
            HealthController = healthController;
            AttackController = null;
            EffectHandler = effectHandler;
            EntityType = entityType;
        }

        public IHealthController HealthController { get; }
        public IAttackControllerInterface AttackController { get; }
        public CombatEntityEffectsHandlerInterface EffectHandler { get; }
        public CombatEntityType EntityType { get; }
    }
}