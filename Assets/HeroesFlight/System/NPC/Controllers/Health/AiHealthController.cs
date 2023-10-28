using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AiHealthController : HealthController
    {
        AiControllerInterface aiController;
        
        public override void Init()
        {
            aiController = GetComponent<AiControllerInterface>();
            maxHealth = aiController.GetHealth;
            heathBarUI?.ChangeType(HeathBarUI.HealthBarType.ToggleVisibilityOnHit);
            defence = aiController.GetMonsterStatModifier().CalculateDefence(aiController.AgentModel.AiData.Defense);
            base.Init();
        }


        protected override void ProcessDeath()
        {        
            base.ProcessDeath();
            aiController.Disable();
        }

        public override void TryDealDamage(HealthModificationIntentModel healthModificationIntent)
        {
            base.TryDealDamage(healthModificationIntent);
            aiController.Aggravate();
            aiController.ProcessKnockBack();
            
        }
    }
}