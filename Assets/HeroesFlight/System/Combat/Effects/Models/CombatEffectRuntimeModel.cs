using System;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    [Serializable]
    public class CombatEffectRuntimeModel 
    {
        public CombatEffectRuntimeModel(CombatEffect effect, GameObject visual,int lvl)
        {
            this.combatEffect = effect;
            this.visual = visual;
            Lvl = lvl;
        }
        [SerializeField] private CombatEffect  combatEffect;
        [SerializeField] private GameObject visual;

        public CombatEffect Effect => combatEffect;
        public GameObject Visual => visual;
        public int Lvl;

    }
}