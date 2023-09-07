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
        public bool IsImmortal { get; protected set; }
        public Transform HealthTransform => transform;
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
        }

        public virtual void DealDamage(DamageModel damage)
        {
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
    }
}