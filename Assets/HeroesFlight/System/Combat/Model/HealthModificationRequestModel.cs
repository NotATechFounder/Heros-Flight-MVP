using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;

namespace HeroesFlight.System.Combat.Model
{
    public class HealthModificationRequestModel
    {
        public HealthModificationRequestModel(HealthModificationIntentModel intentModel,IHealthController requestOwner)
        {
            IntentModel = intentModel;
            RequestOwner = requestOwner;
        }
        public HealthModificationIntentModel IntentModel { get; }
        public IHealthController RequestOwner { get; }
    }
}