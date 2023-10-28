using System;
using System.Collections.Generic;
using Cinemachine;
using HeroesFlight.System.Combat;
using HeroesFlightProject.System.NPC.Controllers;
using Pelumi.ObjectPool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BossSpikesAbility:BossAttackAbilityBase
    {
        [SerializeField] List<BossAttackPattern> patterns = new();
        [SerializeField] ProjectileControllerBase projectilePrefab;
        [SerializeField] float preDamageDelay;
        [SerializeField] float zoneWidth;
      
        protected override void Awake()
        {
            animator = GetComponentInParent<AiAnimatorInterface>();
            currentCooldown = 0;
           
        }
     

        public override void UseAbility(Action onComplete = null)
        {
            if (targetAnimation != null)
            {
                animator.PlayDynamicAnimation(targetAnimation,onComplete);
            }

            var rng = Random.Range(0, patterns.Count);
            var targetZones = patterns[rng].Lines;
            cameraShaker.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Explosion,.5f);
            foreach (var zone in targetZones)
            {
                zone.Trigger(() =>
                {
                    var arrow = ObjectPoolManager.SpawnObject(projectilePrefab, zone.transform.position, Quaternion.identity);
                    arrow.OnEnded += HandleArrowDisable;
                    arrow.SetupProjectile(CalculateDamage(), zone.GetFowardDirection);
                },preDamageDelay,zoneWidth);
            }
         
        }

        void HandleArrowDisable(ProjectileControllerInterface obj)
        {
            obj.OnEnded -= HandleArrowDisable;
            var arrow = obj as ProjectileControllerBase;
            ObjectPoolManager.ReleaseObject(arrow.gameObject);
        }
        
        public override void StopAbility()
        {
            foreach (var lines in patterns)
            {
                foreach (var line in lines.Lines)
                {
                    line.gameObject.SetActive(false);
                }
            }
           
        }
    }
}