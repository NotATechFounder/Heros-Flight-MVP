using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BossAbilityBase : AbilityBaseNPC
    {
        [Range(0,100)]
        [SerializeField] float useChance;
        protected float modifier = 1f;
        protected CameraShakerInterface cameraShaker;
        public float UseChance => useChance;

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