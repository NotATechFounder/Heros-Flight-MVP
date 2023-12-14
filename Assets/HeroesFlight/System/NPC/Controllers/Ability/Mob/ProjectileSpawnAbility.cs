using System;
using System.Collections.Generic;
using HeroesFlight.Common.Animation;
using HeroesFlight.Common.Enum;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using Pelumi.ObjectPool;
using UnityEngine;
using Random = UnityEngine.Random;


namespace HeroesFlight.System.NPC.Controllers.Ability.Mob
{
    public class ProjectileSpawnAbility : AttackAbilityBaseNPC
    {
        [Header("Main  data segment")]
        [SerializeField] ProjectileControllerBase projectilePrefab;
        [SerializeField] int projectileCount = 3;
        [SerializeField] float spreadAngle = 15f;
        [SerializeField] private Transform spawnPoint;
        [Header("Optional mode segment")] [SerializeField]
        bool useWarningLines;
        [SerializeField] WarningLine warningLinePrefab;
      
        [Header("Double trigger segment")] [SerializeField]
        private bool canDoubleTrigger;
        [SerializeField] private float doubleTriggerChance;

        private List<Vector3> offsets = new List<Vector3>();
        private IHealthController healthController;
        private IAttackControllerInterface attackController;
        private AiControllerBase aiController;

        protected override void Awake()
        {
            animator = GetComponentInParent<AiAnimatorInterface>();
            attackController = GetComponentInParent<EnemyAttackControllerBase>();
            healthController = GetComponentInParent<AiHealthController>();
            aiController = GetComponentInParent<AiControllerBase>();
            currentCooldown = 0;
            if (!useWarningLines)
            {
                animator.OnAnimationEvent += HandleAnimationEvents;
            }

            CalculateOffsets();
        }

        // Calculate offsets based on the projectile count
        private void CalculateOffsets()
        {
            int count = 0;
            bool singleProjectile = false;

            if (canDoubleTrigger)
            {
                count = projectileCount * 2;
            }
            else
            {
                count = projectileCount;
                singleProjectile = projectileCount == 1;
            }

            AddOffsets(count, singleProjectile);
        }

        private void AddOffsets(int count, bool singleProjectile)
        {
            if (singleProjectile)
            {
                offsets.Add(Vector3.zero);
            }
            else
            {
                float step = 2 * spreadAngle / (count - 1);

                for (int i = 0; i < count; i++)
                {
                    float angle = -spreadAngle + step * i;
                    offsets.Add(new Vector3(0, 0, angle));
                }
            }
        }

        protected virtual void HandleAnimationEvents(AttackAnimationEvent obj)
        {
            if (obj.Type == AniamtionEventType.Shoot)
            {
                SpawnProjectiles(CalculateProjectilesCount());
            }
        }

        private int CalculateProjectilesCount()
        {
            var count = projectileCount;
            if (canDoubleTrigger)
            {
                var rng = Random.Range(0, 101);
                if (rng <= doubleTriggerChance)
                {
                    count = canDoubleTrigger ? projectileCount * 2 : projectileCount;
                }
            }

            return count;
        }

        public override void UseAbility(Action onComplete = null)
        {
            if (targetAnimation != null)
            {
                animator.StartAttackAnimation(targetAnimation, () => { onComplete?.Invoke(); });
                currentCooldown = CoolDown;
            }


            if (useWarningLines)
            {
                SpawnProjectilesWithDangerLines(CalculateProjectilesCount());
            }
        }

        private void SpawnProjectiles(int count)
        {
            var direction = aiController.CurrentTarget.position - spawnPoint.position;
            for (int i = 0; i < count; i++)
            {
                var final = Quaternion.Euler(offsets[i]) * direction;
                var projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
                projectile.SetupProjectile(damage, final, healthController);
                projectile.OnHit += ResetProjectile;
            }
        }

        private void SpawnProjectilesWithDangerLines(int count)
        {
            var direction = aiController.CurrentTarget.position - spawnPoint.position;
            for (int i = 0; i < count; i++)
            {
                var final = Quaternion.Euler(offsets[i]) * direction;

                var line = ObjectPoolManager.SpawnObject(warningLinePrefab, spawnPoint.position,
                    Quaternion.LookRotation(Vector3.forward, -final));
                line.Init();

                line.Trigger(() =>
                {
                    var projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
                    projectile.OnHit += ResetProjectile;
                    projectile.SetupProjectile(damage, final, healthController);
                    ObjectPoolManager.ReleaseObject(line);
                }, targetAnimation.Animation.Duration - 0.3f, .3f);
            }
        }

        private void ResetProjectile(ProjectileControllerInterface obj)
        {
            var projectile = obj as ProjectileControllerBase;
            projectile.OnHit -= ResetProjectile;
            Destroy(projectile.gameObject);
        }
    }
}