using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BossAbilityBase : AbilityBaseNPC
    {
        protected float modifier = 0f;
        

        public virtual void SetModifierValue(float modification)
        {
            modifier = modification;
        }

     
    }
}