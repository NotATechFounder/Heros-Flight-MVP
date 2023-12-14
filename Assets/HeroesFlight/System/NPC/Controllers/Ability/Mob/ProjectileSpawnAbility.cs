using System;
using System.Collections.Generic;
using HeroesFlight.Common.Animation;
using HeroesFlight.Common.Enum;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers.Ability.Mob
{
    public class ProjectileSpawnAbility : AttackAbilityBaseNPC
    {
        [SerializeField] ProjectileControllerBase projectilePrefab;
        [SerializeField] int projectileCount = 3;
        [SerializeField] float spreadAngle = 15f;

        [Header("Optional visuals")] [SerializeField]
        bool useWarningLines;

        [SerializeField] WarningLine warningLinePrefab;
        [SerializeField] List<WarningLine> lines = new();
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
            if (useWarningLines)
            {
                for (int i = 0; i < projectileCount; i++)
                {
                    lines.Add(Instantiate(warningLinePrefab, aiController.transform));
                }
            }
            else
            {
                animator.OnAnimationEvent += HandleAnimationEvents;
            }

            CalculateOffsets();
        }

        // Calculate offsets based on the projectile count
        private void CalculateOffsets()
        {
            if (projectileCount == 1)
            {
                offsets.Add(Vector3.zero);
            }
            else
            {
                float step = 2 * spreadAngle / (projectileCount - 1);
                for (int i = 0; i < projectileCount; i++)
                {
                    float angle = -spreadAngle + step * i;
                    offsets.Add(new Vector3(0, 0, angle));
                }
            }
        }

        private void HandleAnimationEvents(AttackAnimationEvent obj)
        {
            if (obj.Type != AniamtionEventType.Shoot)
                return;

            var direction = aiController.CurrentTarget.position - transform.position;
            for (int i = 0; i < projectileCount; i++)
            {
                var final = Quaternion.Euler(offsets[i]) * direction;
                var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                projectile.SetupProjectile(damage, final, healthController);
                projectile.OnHit += ResetProjectile;
            }
        }

        public override void UseAbility(Action onComplete = null)
        {
            if (targetAnimation != null)
            {
                animator.StartAttackAnimation(targetAnimation, () => { onComplete?.Invoke(); });
                currentCooldown = CoolDown;
            }

            var direction = aiController.CurrentTarget.position - transform.position;
            if (useWarningLines)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    var final = Quaternion.Euler(offsets[i]) * direction;


                    lines[i].transform.up = -final;
                    lines[i].Trigger(() =>
                    {
                        var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                        projectile.OnHit += ResetProjectile;
                        projectile.SetupProjectile(damage, final, healthController);
                    }, aiController.AgentModel.AiData.AttackSpeed - 0.3f, .3f);
                }
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