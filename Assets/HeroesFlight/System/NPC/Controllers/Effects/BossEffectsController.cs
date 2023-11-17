using HeroesFlight.System.Combat.Effects.Effects;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers.Effects
{
    public class BossEffectsController : CombatEffectsController
    {
        [SerializeField] private AiEffectsController[] effectsControllers;
        public override void ExecuteTick()
        {
            foreach (var effectsController in effectsControllers)
            {
                effectsController.ExecuteTick();
            }
        }
    }
}