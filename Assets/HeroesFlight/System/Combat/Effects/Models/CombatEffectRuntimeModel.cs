using System;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    public class CombatEffectRuntimeModel 
    {
        public CombatEffectRuntimeModel(CombatEffect effect, GameObject visual,int lvl)
        {
            Effect = effect;
            Visual = visual;
            Lvl = lvl;
        }
        public CombatEffect Effect { get; }
        public GameObject Visual { get; }
        public int Lvl;

    }
}