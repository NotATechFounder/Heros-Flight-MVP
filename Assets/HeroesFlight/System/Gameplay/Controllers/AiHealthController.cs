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
            maxHealth = aiController.AgentModel.CombatModel.Health;
            base.Init();
        }

        protected override void ProcessDeath()
        {
            base.ProcessDeath();
            aiController.Disable();
        }

        public override void DealDamage(int damage)
        {
            if(IsDead())
                return;

            TriggerDamageMessage(damage);
            currentHealh -= damage;
            aiController.ProcessKnockBack();
            if (IsDead())
                ProcessDeath();
          
        }
    }
}