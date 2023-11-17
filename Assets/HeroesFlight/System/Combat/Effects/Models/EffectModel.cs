using System;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    public interface EffectModelInterface
    {
         event Action<StatusEffectRuntimeModel> OnEnd;
         void ExecuteTick();

    }
}