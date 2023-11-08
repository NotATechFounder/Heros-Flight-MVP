using System;
using HeroesFlight.Common.Animation;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class EnemyAttackControllerBase : MonoBehaviour, AiSubControllerInterface, IAttackControllerInterface
    {
     
        protected IHealthController target;
        protected float attackRange;
        protected float timeBetweenAttacks;
        protected float timeSinceLastAttack;
        protected AiAnimatorInterface animator;
        protected IHealthController health;
        protected float currentDamage;
        protected float criticalChance;
        protected OverlapChecker damageZone;
        protected bool isEnabled;
        public event Action OnHitTarget;
        public event Action<AttackControllerState> OnStateChange;

        public float Damage => currentDamage;
        public float TimeSinceLastAttack => timeSinceLastAttack;

        public virtual void Init()
        {
           
            animator = GetComponent<AiAnimatorInterface>();
            damageZone = GetComponentInChildren<OverlapChecker>();
            if (damageZone != null)
                damageZone.OnDetect += DealDamage;

            animator.OnAnimationEvent += HandleAnimationEvents;
            health = GetComponent<IHealthController>();
            health.OnDeath += HandleDeath;
            isEnabled = true;

        }


        void HandleDeath(IHealthController obj)
        {
            animator.StopAttackAnimation();
        }

        protected virtual void Update()
        {
            if (health.IsDead())
                return;

            if (target.IsDead())
            {
                return;
            }

            timeSinceLastAttack += Time.deltaTime;
        }

        protected virtual void InitAttack(Action onComplete)
        {
            timeSinceLastAttack = 0;
            OnStateChange?.Invoke(AttackControllerState.Attacking);
            animator.StartAttackAnimation(onComplete);
        }

        protected virtual void DealDamage(int i, Collider2D[] collider2Ds)
        {
            var baseDamage = Damage;

           
            bool isCritical = Random.Range(0, 100) <= criticalChance;

            float damageToDeal = isCritical
                ? baseDamage * criticalChance
                : baseDamage;

            var type = isCritical ? DamageType.Critical : DamageType.NoneCritical;
            var damageModel = new HealthModificationIntentModel(damageToDeal, type,
                AttackType.Regular, DamageCalculationType.Flat);
            target.TryDealDamage(damageModel);
            OnHitTarget?.Invoke();
        }

        public virtual void AttackTargets(Action OnComplete)
        {
            if (CanAttack())
            {
                InitAttack(OnComplete);
            }
        }

        public virtual void ToggleControllerState(bool isEnabled) { this.isEnabled = isEnabled; }

        public bool CanAttack()
        {
            return isEnabled && timeSinceLastAttack >= timeBetweenAttacks;
        }

        protected virtual void HandleAnimationEvents(AttackAnimationEvent obj)
        {
            OnStateChange?.Invoke(AttackControllerState.Cooldown);
            damageZone.DetectOverlap();
        }

        public void SetAttackStats(float damage, float attackRange, float attackSpeed,float criticalChance)
        {
            timeBetweenAttacks = attackSpeed;
            this.attackRange = attackRange;
            currentDamage = damage;
            timeSinceLastAttack = timeBetweenAttacks;
            this.criticalChance = criticalChance;
        }

        public void SetTarget(IHealthController target) => this.target = target;

        public bool InAttackRange()
        {
            return Vector2.Distance(target.HealthTransform.position, transform.position)
                   <= attackRange;
        }
    }
}