using System;
using HeroesFlight.System.Stats.Traits.Enum;
using UnityEngine;

namespace HeroesFlight.System.Stats.Traits.Effects
{
    [CreateAssetMenu(fileName = "HealthRestoreEffect", menuName = "Traits/TraitEffects/HealthRestoreEffect", order = 100)]
    public class HealthRestoreEffect : TraitEffect
    {
        private void Awake()
        {
            traitType = TraitType.HealthRestore;
        }
    }
}