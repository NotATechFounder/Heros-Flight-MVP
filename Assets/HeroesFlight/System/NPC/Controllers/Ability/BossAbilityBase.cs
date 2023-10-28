using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BossAbilityBase : AbilityBaseNPC
    {
        [Range(0, 100)][SerializeField] float useChance;
        [SerializeField] protected float modifier = 0f;
        [SerializeField] protected ParticleSystem abilityParticle;
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
            animator.StopDynamicAnimation();
        }

        public virtual void ResetAbility()
        {
            animator.StopDynamicAnimation();
        }

        public void SetCoolDown(float cd)
        {
            currentCooldown = cd;
        }
    }
}