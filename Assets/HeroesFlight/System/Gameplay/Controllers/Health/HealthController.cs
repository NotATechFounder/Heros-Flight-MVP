using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Model;
using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class HealthController : MonoBehaviour, IHealthController
    {
        [SerializeField] protected bool autoInit;
        [SerializeField] CombatTargetType targetType;
        [SerializeField] protected float maxHealth;
        [SerializeField] protected float currentHealth;
        [SerializeField] protected HeathBarUI heathBarUI;
        [SerializeField] protected float defence;
        [SerializeField] protected float dodgeChance;

        [Header("Hit Settings")]
        [SerializeField] protected bool useHit;
        [SerializeField] protected int maxHit;
        [SerializeField] protected int currentHit;
        public event Action<Transform> OnBeingHitDamaged;

        public int MaxHit => maxHit;
        public int CurrentHit => currentHit;

        public bool IsImmortal { get; protected set; }
        public Transform HealthTransform => transform;
        public CombatTargetType TargetType => targetType;
        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public float CurrentHealthProportion => (float)currentHealth / maxHealth;
        public event Action<DamageModel> OnBeingDamaged;
        public event Action<IHealthController> OnDeath;
        public event Action<float, Transform> OnHeal;
        public event Action OnDodged;

        private void OnEnable()
        {
            if (autoInit) Init();
        }

        public virtual void Init()
        {
            currentHealth = maxHealth;
            heathBarUI?.ChangeValue((float)currentHealth / maxHealth);
            currentHit = maxHit;
        }

        public virtual void DealDamage(DamageModel damage)
        {
            // To remove
            if(useHit)
            {
                DealHit();
                return;
            }


            if (IsImmortal)
                return;
            
            if(IsDead())
                return;

            if (DodgeAttack())
            {
                OnDodged?.Invoke();
                return;
            }

            var resultDamage = damage.Amount -
                StatCalc.GetValueOfPercentage(damage.Amount, defence);
            damage.ModifyAmount(resultDamage);
            currentHealth -= resultDamage;
            heathBarUI?.ChangeValue((float)currentHealth / maxHealth);
            damage.SetTarget(transform);
            OnBeingDamaged?.Invoke(damage);

            if (IsDead())
                ProcessDeath();
        }

        public virtual void Heal(float amount)
        {
            if (IsDead())
                return;

            currentHealth += amount;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
            heathBarUI?.ChangeValue((float)currentHealth / maxHealth);
            OnHeal?.Invoke(amount, transform);
        }

        public virtual bool IsDead()
        {
            return currentHealth <= 0;
        }

        public virtual void Reset()
        {
            Init();
            OnBeingDamaged = null;
            OnDeath = null;
        }

        public virtual void Revive()
        {
            Init();
        }

        public virtual void SetInvulnerableState(bool isImmortal)
        {
            IsImmortal = isImmortal;
        }

        protected virtual void ProcessDeath()
        {
           OnDeath?.Invoke(this);
        }

        protected void TriggerDamageMessage(DamageModel damage)
        {
            damage.SetTarget(transform);
            OnBeingDamaged?.Invoke(damage);
        }

        protected void SetMaxHealth(float health)
        {
            maxHealth = health;
            heathBarUI?.ChangeValue((float)currentHealth / maxHealth);
        }

        public bool DodgeAttack()
        {
            return Random.Range(0, 100) < dodgeChance;
        }

        public virtual void DealHit()
        {
            if (currentHealth <= 0) return;

            --currentHit;
            OnBeingHitDamaged?.Invoke(transform);
        }
    }
}