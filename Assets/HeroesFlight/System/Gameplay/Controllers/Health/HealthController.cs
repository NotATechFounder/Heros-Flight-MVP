using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class HealthController : MonoBehaviour, IHealthController
    {
        [SerializeField] CombatTargetType targetType;
        protected int maxHealth;
        [SerializeField] protected int currentHealh;
        [SerializeField] protected HeathBarUI heathBarUI;
        public Transform currentTransform => transform;
        public CombatTargetType TargetType => targetType;
        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealh;
        public float CurrentHealthProportion => (float)currentHealh / maxHealth;
        public event Action<DamageModel> OnBeingDamaged;
        public event Action<IHealthController> OnDeath;

        void Awake()
        {
            Init();
        }

        public virtual void Init()
        {
            currentHealh = maxHealth;
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