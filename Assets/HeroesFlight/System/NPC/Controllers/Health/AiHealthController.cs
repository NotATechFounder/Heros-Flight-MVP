using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Controllers;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AiHealthController : HealthController, AiSubControllerInterface
    {
        AiControllerInterface aiController;

        public override void Init()
        {
            aiController = GetComponent<AiControllerInterface>();
            heathBarUI?.ChangeType(WorldBarUI.BarType.ToggleVisibilityOnHit);
            base.Init();
        }


        public override void ModifyHealth(HealthModificationIntentModel modificationIntentModel)
        {
            aiController.Aggravate();
            if (modificationIntentModel.AttackType != AttackType.DoT)
            {
                aiController.ProcessKnockBack();
            }

            base.ModifyHealth(modificationIntentModel);
        }

        public void SetHealthStats(float maxHealth, float defence)
        {
            this.maxHealth = maxHealth;
            this.defence = defence;
        }
    }
}