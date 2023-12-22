using System;
using System.Collections;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers.Ability.Mob
{
    public class DamageDashAbility : AttackAbilityBaseNPC
    {
        [SerializeField] GameObject targetObject;
        [SerializeField] private float preDashDelay = 1f;
        [SerializeField] private float dashForce=100;
        [SerializeField] Trigger2DObserver observer;
        [SerializeField] private bool useYAxis = true;
        private Rigidbody2D rigidbody2D;
      

        protected override void Awake()
        {
            animator = GetComponentInParent<AiAnimatorInterface>();
            attackController = GetComponentInParent<EnemyAttackControllerBase>();
            healthController = GetComponentInParent<AiHealthController>();
            rigidbody2D = GetComponentInParent<Rigidbody2D>();
            currentCooldown = 0;
            
        }
        
        public override void UseAbility(Action onComplete = null)
        {
            observer.OnEnter += HandleTargetEntered;
         
            if (targetAnimation != null)
            {
                animator.PlayDynamicAnimation(targetAnimation, () =>
                {
                    rigidbody2D.velocity=Vector2.zero;
                    observer.OnEnter -= HandleTargetEntered;
                    targetObject.SetActive(false);
                    onComplete?.Invoke();
                });
                var direction = CalculateDirection();
                currentCooldown = CoolDown;
                StartCoroutine(DashMovement(direction));
            }
        }

        private Vector3 CalculateDirection()
        {
            var direction = (attackController.Target.HealthTransform.position - transform.position).normalized;
           if (!useYAxis)
            {
                direction.y = 0;
            }
            return direction;
        }

        private void HandleTargetEntered(Collider2D obj)
        {
            var health = obj.GetComponent<IHealthController>();
            if (health != attackController.Target)
                return;
            
            TryDealDamage(health);
        }

        IEnumerator DashMovement(Vector3 direction)
        {
            yield return new WaitForSeconds(preDashDelay);
            if (!healthController.IsDead())
            {
                targetObject.SetActive(true);
                rigidbody2D.velocity = direction * dashForce;
            }
            
        }
    }
    
    
}