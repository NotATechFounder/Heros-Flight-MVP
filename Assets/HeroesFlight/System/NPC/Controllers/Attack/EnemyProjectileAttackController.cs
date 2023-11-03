using System;
using System.Collections.Generic;
using HeroesFlight.Common.Animation;
using UnityEngine;


namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class EnemyProjectileAttackController : EnemyAttackControllerBase
    {
        [SerializeField] ProjectileControllerBase projectilePrefab;
        [SerializeField] int projectileCount = 3;
        [SerializeField] float spreadAngle = 15f;
        [Header("Optional visuals")]
        [SerializeField] bool useWarningLines;
        [SerializeField] WarningLine warningLinePrefab;
        [SerializeField] List<WarningLine> lines = new();
        [SerializeField] List<Vector3> offsets= new List<Vector3>();
        
     
        public override void Init()
        {
            base.Init();
            if (useWarningLines)
            {
                for (int i = 0; i < projectileCount; i++)
                {
                    lines.Add(Instantiate(warningLinePrefab,transform));
                }
            }
            
            offsets.Add(new Vector3(0,0,-spreadAngle));
            offsets.Add(Vector3.zero);
            offsets.Add(new Vector3(0,0,spreadAngle));
        }

        protected override void Update()
        {
            if (health.IsDead())
                return;

            if (target.IsDead())
            {
               return;
            }


            timeSinceLastAttack += Time.deltaTime;
        }


        protected override void InitAttack(Action onComplete=null)
        {
            timeSinceLastAttack = 0;
            if (useWarningLines)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    Vector2 direction = target.HealthTransform.position - transform.position;
                   
                    var final = Quaternion.Euler(offsets[i]) *direction ;
                    
                    
                    lines[i].transform.up = -final;
                    lines[i].Trigger(() =>
                    {
                        var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                        projectile.OnEnded += ResetProjectile;
                        projectile.SetupProjectile(Damage,final);
                    },timeBetweenAttacks-0.3f,.3f);
                }
            }
            
            animator.StartAttackAnimation(() =>
            {
                // for (int i = 0; i < projectileCount; i++)
                // {
                //     Vector2 direction = aiController.CurrentTarget.position - transform.position;
                //     var rng = new Vector2(Random.Range(-spreadValue, spreadValue),
                //         Random.Range(-spreadValue, spreadValue));
                //     var final = direction + rng;
                //
                //     var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                //     projectile.OnEnded += ResetProjectile;
                //     projectile.SetupProjectile(Damage,final);
                // }
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