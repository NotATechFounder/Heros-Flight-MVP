using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.NPC.Controllers;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AiHealthController : HealthController
    {
        AiControllerInterface aiController;
        
        public override void Init()
        {
            aiController = GetComponent<AiControllerInterface>();
            maxHealth = aiController.AgentModel.CombatModel.GetMonsterStatData.Health;
            heathBarUI?.ChangeType(HeathBarUI.HealthBarType.ToggleVisibilityOnHit);
            defence=aiController.GetMonsterStatModifier().CalculateDefence(aiController.AgentModel.CombatModel.GetMonsterStatData.Defense);
            base.Init();
        }

        protected override void ProcessDeath()
        {
            base.ProcessDeath();
            aiController.Disable();
        }

        public override void DealDamage(DamageModel damage)
        {
            aiController.Aggravate();
            aiController.ProcessKnockBack();
            base.DealDamage(damage);
        }
    }
}