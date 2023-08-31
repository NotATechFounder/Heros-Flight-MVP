
using System;
using HeroesFlight.Common;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AttackAbility : AbilityBaseNPC
    {
        [SerializeField] AiAgentCombatModel model;
        public override void UseAbility(float damage, IHealthController target = null, Action onComplete = null)
        {
            base.UseAbility(damage,target, onComplete);
            target?.DealDamage(new DamageModel(damage, DamageType.NoneCritical,AttackType.Regular));
        }
    }
}