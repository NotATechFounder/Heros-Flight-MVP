using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BossMushroomsAbility : AbilityBaseNPC
    {
        [SerializeField] AreaDamageEntity[] mushrooms;
        [SerializeField] float damage;

        protected override void Awake()
        {
            base.Awake();
            foreach (var mushroom in mushrooms)
            {
                mushroom.OnTargetsDetected += HandleTargetsDetected;
            }
        }

        void HandleTargetsDetected(int count, Collider2D[] targets)
        {
            for (int i = 0; i < count; i++)
            {
                if(targets[i].TryGetComponent<IHealthController>(out var health))
                {
                    health.DealDamage(new DamageModel(damage,DamageType.NoneCritical,AttackType.Regular));
                }
            }
        }
    }
}