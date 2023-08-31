using System.Collections.Generic;
using System.Linq;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class StormAttackController : BaseCharacterAttackController
    {
        CharacterAttackAbilityInterface chainLightningAbility;

        public override void Init()
        {
            base.Init();
            chainLightningAbility = GetComponent<ChainLightningAbility>();
            chainLightningAbility.OnDealingDamage += HandleDamageFromAbility;
        }

        protected override void DealNormalDamage(int hits, Collider2D[] colliders)
        {
            var baseDamage = Damage;
            
            float criticalChance = characterController.CharacterStatController.CurrentCriticalHitChance;
            bool isCritical = Random.Range(0, 100) <= criticalChance;

            float damageToDeal = isCritical
                ? baseDamage * characterController.CharacterStatController.CurrentCriticalHitDamage
                : baseDamage;

            var type = isCritical ? DamageType.Critical : DamageType.NoneCritical;
            var damageModel = new DamageModel(damageToDeal, type, AttackType.Regular);
            for (int i = 0; i < hits; i++)
            {
                if (colliders[i].TryGetComponent<IHealthController>(out var health))
                {
                    chainLightningAbility.UseAbility(health, damageModel);
                    break;
                }
            }
        }

        void HandleDamageFromAbility(Transform target)
        {
            ApplyLifeSteal();
        }
    }
}