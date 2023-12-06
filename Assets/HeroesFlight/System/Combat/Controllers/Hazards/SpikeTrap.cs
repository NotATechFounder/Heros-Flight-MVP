using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using UnityEngine;

namespace HeroesFlight.System.Combat.Controllers.Hazards
{
    public class SpikeTrap : MonoBehaviour
    {
        [SerializeField] private CalculationType damageType;
        [SerializeField] private float damage;
        [SerializeField] private Trigger2DObserver trigger;

        private void Awake()
        {
            trigger.OnEnter += HandleTargetEnteredTrap;
        }

        private void HandleTargetEnteredTrap(Collider2D target)
        {
            if (target.TryGetComponent<IHealthController>(out var healthController))
            {
                healthController.TryDealDamage(new HealthModificationIntentModel(damage, DamageCritType.NoneCritical,
                    AttackType.Regular, damageType, null));
            }
        }
    }
}