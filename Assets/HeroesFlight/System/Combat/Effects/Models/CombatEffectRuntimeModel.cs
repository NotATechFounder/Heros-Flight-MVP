using System;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    public class CombatEffectRuntimeModel 
    {
        public CombatEffectRuntimeModel(CombatEffect effect, GameObject visual)
        {
            Effect = effect;
            Visual = visual;
        }
        public CombatEffect Effect { get; }
        public GameObject Visual { get; }
       
    }
}