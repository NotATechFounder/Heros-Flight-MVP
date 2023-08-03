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
        [SerializeField] protected float currentHealh;
        [SerializeField] protected HeathBarUI heathBarUI;
        public Transform currentTransform => transform;
        public CombatTargetType TargetType => targetType;
        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealh;
        public float CurrentHealthProportion => (float)currentHealh / maxHealth;
        public event Action<DamageModel> OnBeingDamaged;
        public event Action<IHealthController> OnDeath;

       
        public virtual void Init()
        {
            currentHealh = maxHealth;
            heathBarUI?.ChangeValue((float)currentHealh / maxHealth);
        }

        public virtual void DealDamage(DamageModel damage)
        {
            if(IsDead())
                return;
            
            currentHealh -= damage.Amount;
            heathBarUI?.ChangeValue((float)currentHealh / maxHealth);
            damage.SetTarget(transform);
            OnBeingDamaged?.Invoke(damage);

            if (IsDead())
                ProcessDeath();
        }

        public virtual bool IsDead()
        {
            return currentHealh <= 0;
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