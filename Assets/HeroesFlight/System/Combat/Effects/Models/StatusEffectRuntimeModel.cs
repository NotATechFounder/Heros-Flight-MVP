using System;
using HeroesFlight.System.Combat.Effects.Enum;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    public class StatusEffectRuntimeModel : EffectModelInterface
    {
       
        public event Action<StatusEffectRuntimeModel> OnEnd;
        public event Action<StatusEffectRuntimeModel> OnTick; 

        public StatusEffectRuntimeModel(StatusEffect effect, GameObject visual)
        {
            Effect = effect;
            Visual = visual;
            currentDuration = effect.Duration;
            CurrentStacks = 1;
        }

        public StatusEffect Effect { get; }
        public int CurrentStacks { get; private set; }
        public GameObject Visual { get; }
        private float currentDuration;

        public void ExecuteTick()
        {
            if (Effect.DurationType == EffectDurationType.Fixed)
            {
                if (currentDuration <= 0)
                {
                    OnEnd?.Invoke(this);
                    return;
                }
                
                currentDuration--;
                OnTick?.Invoke(this);
                
            }
        }

        public void AddStack()
        {
            CurrentStacks++;
            RefreshDuration();
        }

        public void RefreshDuration()
        {
            currentDuration = Effect.Duration;
        }
    }
}