using System;
using HeroesFlightProject.System.NPC.Controllers;
using Pelumi.ObjectPool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BossSpikesAbility:BossAttackAbilityBase
    {
        [SerializeField] AbilityZone[] horizontalZones;
        [SerializeField] AbilityZone[] verticalZones;
        [SerializeField] ProjectileControllerBase projectilePrefab;
        [SerializeField] float preDamageDelay;
        [SerializeField] float zoneWidth;
      
        protected override void Awake()
        {
            animator = GetComponentInParent<AiAnimatorInterface>();
            timeSincelastUse = 0;
           
        }
     

        public override void UseAbility(Action onComplete = null)
        {
            if (targetAnimation != null)
            {
                animator.PlayAnimation(targetAnimation,onComplete);
            }

            var rng = Random.Range(0, 2);
            var targetZones = rng == 0 ? horizontalZones : verticalZones;
            foreach (var zone in targetZones)
            {
                zone.ZoneVisual.Trigger(() =>
                {
                    var arrow = ObjectPoolManager.SpawnObject(projectilePrefab, zone.ZoneVisual.transform.position, Quaternion.identity);
                    arrow.OnEnded += HandleArrowDisable;
                    arrow.SetupProjectile(CalculateDamage(), zone.ZoneVisual.GetFowardDirection);
                },preDamageDelay,zoneWidth);
            }

            timeSincelastUse = coolDown;
        }

        void HandleArrowDisable(ProjectileControllerInterface obj)
        {
            obj.OnEnded -= HandleArrowDisable;
            var arrow = obj as ProjectileControllerBase;
            ObjectPoolManager.ReleaseObject(arrow.gameObject);
        }
    }
}