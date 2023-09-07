using HeroesFlight.Common.Animation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class EnemyProjectileAttackController : EnemyAttackControllerBase
    {
        [SerializeField] ProjectileControllerBase projectilePrefab;
        [SerializeField] int projectileCount = 3;
        [SerializeField] float spreadValue = 1f;


        protected override void Update()
        {
            if (health.IsDead())
                return;

            if (target.IsDead())
            {
                aiController.SetAttackState(false);
                return;
            }


            timeSinceLastAttack += Time.deltaTime;
            var distanceToPlayer = Vector2.Distance(transform.position, aiController.CurrentTarget.position);
            if (distanceToPlayer <= attackRange && timeSinceLastAttack >= timeBetweenAttacks)
            {
                aiController.SetAttackState(true);
                InitAttack();
            }
        }


        protected override void InitAttack()
        {
            timeSinceLastAttack = 0;
            aiController.SetAttackState(false);
            animator.StartAttackAnimation(() =>
            {
                for (int i = 0; i < projectileCount; i++)
                {
                    Vector2 direction = aiController.CurrentTarget.position - transform.position;
                    var rng = new Vector2(Random.Range(-spreadValue, spreadValue),
                        Random.Range(-spreadValue, spreadValue));
                    var final = direction + rng;

                    var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                    projectile.OnEnded += ResetProjectile;
                    projectile.SetupProjectile(Damage,final);
                }
            });
        }

        protected override void HandleAnimationEvents(AttackAnimationEvent obj) { }

        void ResetProjectile(ProjectileControllerInterface obj)
        {
            var projectile = obj as ProjectileControllerBase;
            projectile.OnEnded -= ResetProjectile;
            Destroy(projectile.gameObject);
        }
    }
}