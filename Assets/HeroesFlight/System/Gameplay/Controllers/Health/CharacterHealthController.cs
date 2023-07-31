using HeroesFlight.System.Character;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class CharacterHealthController : HealthController
    {
        CharacterControllerInterface controller;
        CharacterAttackController attackController;

        public override void Init()
        {
            controller = GetComponent<CharacterControllerInterface>();
            attackController = GetComponent<CharacterAttackController>();
            maxHealth = controller.Data.CombatModel.Health;
            base.Init();
        }

        protected override void ProcessDeath()
        {
            base.ProcessDeath();
            controller.SetActionState(true);
            attackController.DisableActions();
        }
    }
}