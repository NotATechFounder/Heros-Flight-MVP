using HeroesFlight.System.Character;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class CharacterHealthController : HealthController
    {
        CharacterControllerInterface controller;
        public override void Init()
        {
            controller = GetComponent<CharacterControllerInterface>();
            maxHealth = controller.Data.CombatModel.Health;
            base.Init();
        }
    }
}