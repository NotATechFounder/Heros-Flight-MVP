using System;
using System.Collections;
using HeroesFlight.Common.Animation;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Effects.Effects;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlight.System.NPC.Controllers.Ability.Mob
{
    public class JumpSlamAbility : AttackAbilityBaseNPC
    {
        [Header("Damage zones")] [SerializeField]
        OverlapChecker[] abilityZones;

        [Header("Jump parameters")] 
        [SerializeField] private float preJumpDelay=0.6f;
        [SerializeField] private float jumpForce;

        [Header("Optional status effects")] [SerializeField]
        private StatusEffect[] effects;

        private Rigidbody2D rigidbody2D;
        private IHealthController healthController;
        private EnemyAttackControllerBase attackController;

        protected override void Awake()
        {
            rigidbody2D = GetComponentInParent<Rigidbody2D>();
            animator = GetComponentInParent<AiAnimatorInterface>();
            attackController = GetComponentInParent<EnemyAttackControllerBase>();
            healthController = GetComponentInParent<AiHealthController>();
            currentCooldown = 0;
            foreach (var zone in abilityZones)
            {
                zone.OnDetect += NotifyTargetDetected;
            }
        }

        void NotifyTargetDetected(int count, Collider2D[] targets)
        {
            for (int i = 0; i < count; i++)
            {
                DealDamage(targets[i]);
                ApplyStatusEffects(targets[i]);
            }
        }

        private void ApplyStatusEffects(Collider2D target)
        {
            if (effects.Length == 0)
                return;
            if (target.TryGetComponent<CombatEffectsController>(out var effectController))
            {
                foreach (var effect in effects)
                {
                    effectController.ApplyStatusEffect(effect, 1);
                }
            }
        }

        private void DealDamage(Collider2D target)
        {
            if (target.TryGetComponent<IHealthController>(out var health))
            {
                if (canCrit)
                {
                    bool isCritical = Random.Range(0, 100) <= SkillCritChance;

                    float damageToDeal = isCritical
                        ? SkillDamage * critModifier
                        : damage;

                    var type = isCritical ? DamageCritType.Critical : DamageCritType.NoneCritical;
                    var damageModel = new HealthModificationIntentModel(damageToDeal,
                        type, AttackType.Regular,damageType, healthController);
                    health.TryDealDamage(damageModel);
                }
                else
                {
                    health.TryDealDamage(new HealthModificationIntentModel(SkillDamage,
                        DamageCritType.NoneCritical, AttackType.Regular, damageType, healthController));
                }
            }
        }

        public override void UseAbility(Action onComplete = null)
        {
            animator.OnAnimationEvent += HandleAnimationEvents;
            if (targetAnimation != null)
            {
                animator.PlayDynamicAnimation(targetAnimation, () =>
                {
                    rigidbody2D.velocity=Vector2.zero;
                    animator.OnAnimationEvent -= HandleAnimationEvents;
                    onComplete?.Invoke();
                });
                currentCooldown = CoolDown;
            }

            var direction = attackController.Target.HealthTransform.position.x > transform.position.x
                ? Vector2.right
                : Vector2.left;
            currentCooldown = CoolDown;
            StartCoroutine(ChargeMovement(direction));
        }

        void HandleAnimationEvents(AttackAnimationEvent obj)
        {
            if (abilityParticle != null)
            {
                abilityParticle.Play();
            }

            foreach (var zone in abilityZones)
            {
                zone.DetectOverlap();
            }
        }
        
        IEnumerator ChargeMovement(Vector3 direction)
        {
            yield return new WaitForSeconds(preJumpDelay);
            if (!healthController.IsDead())
            {
               rigidbody2D.AddForce(direction * jumpForce, ForceMode2D.Impulse);    
            }
            
        }
    }
}