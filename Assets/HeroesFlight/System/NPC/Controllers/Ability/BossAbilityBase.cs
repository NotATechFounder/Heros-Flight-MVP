namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BossAbilityBase : AbilityBaseNPC
    {
        protected float modifier = 1f;
        protected CameraShakerInterface cameraShaker;

        public void InjectShaker(CameraShakerInterface shaker)
        {
            cameraShaker = shaker;
        }
        public virtual void SetModifierValue(float modification)
        {
            modifier = modification;
        }

        public virtual void StopAbility()
        {
            
        }
    }
}