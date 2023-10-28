using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;

namespace HeroesFlight.System.Combat.Model
{
    public class EntityDamageModel
    {
        public EntityDamageModel(IHealthController controller,HealthModificationIntentModel healthModificationIntentModel)
        {
            TargetController = controller;
            HealthModificationIntentDealt = healthModificationIntentModel;
        }
        public IHealthController TargetController { get; }
        public HealthModificationIntentModel HealthModificationIntentDealt { get; }
    }
}