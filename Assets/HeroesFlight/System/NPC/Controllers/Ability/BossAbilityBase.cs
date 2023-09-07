namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BossAbilityBase : AbilityBaseNPC
    {
        protected float modifier = 1f;

        public virtual void SetModifierValue(float modification)
        {
            modifier = modification;
        }
    }
}