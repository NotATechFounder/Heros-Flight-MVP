using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AiHealthController : HealthController,AiSubControllerInterface
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

        public void SetHealthStats(float maxHealth,float defence)
        {
            this.maxHealth = maxHealth;
            this.defence = defence;
        }
    }
}