using System;
using HeroesFlight.Common.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class HealthController : MonoBehaviour, IHealthController
    {
        [SerializeField] CombatTargetType targetType;
        protected int maxHealth;
        [SerializeField] protected int currentHealh;
        public Transform currentTransform => transform;
        public CombatTargetType TargetType => targetType;
        public int CurrentHealth => currentHealh;
        public event Action<Transform,int> OnBeingDamaged;
        public event Action<IHealthController> OnDeath;

        void Awake()
        {
            Init();
        }

        public virtual void Init()
        {
            currentHealh = maxHealth;
        }

        public virtual void DealDamage(int damage)
        {
            if(IsDead())
                return;
            
            OnBeingDamaged?.Invoke(transform,damage);
            currentHealh -= damage;
            if (IsDead())
                ProcessDeath();
        }

        public virtual bool IsDead()
        {
            return currentHealh <= 0;
        }

        protected virtual void ProcessDeath()
        {
            Debug.Log(name + " is dead");
            OnDeath?.Invoke(this);
        }

        protected void TriggerDamageMessage(int damage)
        {
            OnBeingDamaged.Invoke(transform,damage);
        }
    }
}