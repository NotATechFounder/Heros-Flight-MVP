using System;
using HeroesFlight.Common.Enum;
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
        public event Action<Transform, float> OnBeingDamaged;
        public event Action<IHealthController> OnDeath;

        void Awake()
        {
            Init();
        }

        public virtual void Init()
        {
            currentHealh = maxHealth;
        }

        public virtual void DealDamage(float damage)
        {
            if(IsDead())
                return;

            currentHealh -= damage;
            heathBarUI?.ChangeValue(currentHealh / maxHealth);
            OnBeingDamaged?.Invoke(transform, damage);

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

        protected void TriggerDamageMessage(int damage)
        {
            OnBeingDamaged?.Invoke(transform,damage);
        }
    }
}