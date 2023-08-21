
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
        public override void UseAbility(IHealthController target = null, Action onComplete = null)
        {
            base.UseAbility(target, onComplete);
            target?.DealDamage(new DamageModel(model.GetMonsterStatData.Damage, DamageType.NoneCritical,AttackType.Regular));
        }
    }
}