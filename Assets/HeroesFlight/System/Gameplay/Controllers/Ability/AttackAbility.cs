
using System;
using HeroesFlight.Common;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AttackAbility : AbilityBase
    {
        [SerializeField] AiAgentCombatModel model;
        public override void UseAbility(IHealthController target = null, Action onComplete = null)
        {
            base.UseAbility(target, onComplete);
            target?.DealDamage(model.Damage);
        }
    }
}