using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class HealthController : MonoBehaviour, IHealthController
    {
        [SerializeField] CombatTargetType targetType;
        protected float maxHealth;
        [SerializeField] protected float currentHealth;
        [SerializeField] protected HeathBarUI heathBarUI;
        public Transform currentTransform => transform;
        public CombatTargetType TargetType => targetType;
        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public float CurrentHealthProportion => (float)currentHealth / maxHealth;
        public event Action<DamageModel> OnBeingDamaged;
        public event Action<IHealthController> OnDeath;

       
        public virtual void Init()
        {
            currentHealth = maxHealth;
            heathBarUI?.ChangeValue((float)currentHealth / maxHealth);
        }

        public virtual void DealDamage(DamageModel damage)
        {
            if(IsDead())
                return;
            
            currentHealth -= damage.Amount;
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

        protected virtual void ProcessDeath()
        {
           OnDeath?.Invoke(this);
        }

        protected void TriggerDamageMessage(DamageModel damage)
        {
            damage.SetTarget(transform);
            OnBeingDamaged?.Invoke(damage);
        }
    }
}