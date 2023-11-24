using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using System;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Combat.Model;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using HeroesFlight.System.NPC.Controllers;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class HealthController : MonoBehaviour, IHealthController
    {
        [SerializeField] protected HealthType healthType;
        [SerializeField] protected bool autoInit;
        [SerializeField] protected float maxHealth;
        [SerializeField] protected float currentHealth;
        [SerializeField] protected WorldBarUI heathBarUI;
        [SerializeField] protected float defence;
        [SerializeField] protected float dodgeChance;

        [Header("Hit Settings")]
        [SerializeField] protected bool useHit;
        [SerializeField] protected int maxHit;
        [SerializeField] protected int currentHit;

        [Header("Immortality")] 
        [SerializeField] protected bool becomeImmortalOnHit;
        [SerializeField] protected float immortalityDuration=.5f;
        [SerializeField] private bool isCurrentlyImmortal;
        public event Action<Transform> OnBeingHitDamaged;

        public int MaxHit => maxHit;
        public int CurrentHit => currentHit;

        public bool IsImmortal
        {
            get
            {
                return isCurrentlyImmortal;
            }
            protected set
            {
                isCurrentlyImmortal = value;
            }
        }

        public bool IsShielded { get; protected set; }
        public HealthType HealthType => healthType;
        public Transform HealthTransform => transform;
        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public float CurrentHealthProportion => (float)currentHealth / maxHealth;
        public event Action<HealthModificationRequestModel> OnDamageReceiveRequest;
        public event Action<HealthModificationIntentModel> OnHitWhileIsShielded;
        public event Action<IHealthController> OnDeath;
        public event Action OnDodged;
        private FlashEffect flashEffect;

        private float currentImmortalityDuration;
        private void OnEnable()
        {
            if (autoInit) Init();
            flashEffect = GetComponentInChildren<FlashEffect>();
        }

        public virtual void Init()
        {
            currentHealth = maxHealth;
            heathBarUI?.ChangeValue((float)currentHealth / maxHealth);
            currentHit = maxHit;
        }

        protected virtual void Update()
        {
            UpdateTimers();
        }

        protected virtual void UpdateTimers()
        {
            if (currentImmortalityDuration > 0)
            {
                currentImmortalityDuration -= Time.deltaTime;
                if (currentImmortalityDuration <= 0)
                {
                    IsImmortal = false;
                }
            }
        }

        public virtual void TryDealDamage(HealthModificationIntentModel healthModificationIntent)
        {
            // To remove
            if (useHit)
            {
                DealHit();
                return;
            }

            if (IsShielded)
            {
                OnHitWhileIsShielded?.Invoke(healthModificationIntent);
                return;
            }

            if (IsImmortal)
            {
                return;
            }
  
           
            if (IsDead())
                return;
           
            if (DodgeAttack())
            {
                OnDodged?.Invoke();
                return;
            }
            
           
            healthModificationIntent.SetTarget(transform);
            OnDamageReceiveRequest?.Invoke(new HealthModificationRequestModel(healthModificationIntent,this));
            

        }

        public virtual void ModifyHealth(HealthModificationIntentModel modificationIntentModel)
        {
            if (modificationIntentModel.AttackType != AttackType.Healing)
            {
               var resultDamage = modificationIntentModel.Amount -
                                   StatCalc.GetPercentage(modificationIntentModel.Amount, defence);
                currentHealth -= resultDamage;
                flashEffect?.Flash(immortalityDuration);
                if (becomeImmortalOnHit && modificationIntentModel.AttackType != AttackType.DoT)
                {
                    IsImmortal = true;
                    currentImmortalityDuration = immortalityDuration;
                }
            }
            else
            {
                currentHealth +=  modificationIntentModel.Amount;
                if (currentHealth > maxHealth)
                    currentHealth = maxHealth;
            }
           
            heathBarUI?.ChangeValue((float)currentHealth / maxHealth);

            if (IsDead())
            {
                ProcessDeath();
            }
               
        }

        public virtual void Heal(float amount, bool notify = true)
        {
            if (IsDead())
                return;
            if (currentHealth >= maxHealth)
            {
                currentHealth = maxHealth;
                return;
            }

            if (notify)
            {
                var intent =new HealthModificationIntentModel(amount,
                    DamageCritType.NoneCritical, AttackType.Healing, CalculationType.Flat,null);
                OnDamageReceiveRequest?.Invoke(new HealthModificationRequestModel(intent, this));
            }
            else
            {
                currentHealth += amount;
                if (currentHealth > maxHealth)
                    currentHealth = maxHealth;
                heathBarUI?.ChangeValue((float)currentHealth / maxHealth);
            }
        }

        public virtual bool IsDead()
        {
            return currentHealth <= 0;
        }

        public virtual void Reset()
        {
            Init();
            OnDamageReceiveRequest = null;
            OnDeath = null;
        }

        public virtual void Revive(float healthPercentage)
        {
            currentHealth = maxHealth/100 * healthPercentage;
            heathBarUI?.ChangeValue((float)currentHealth / maxHealth);
            currentHit = maxHit;
        }

        public virtual void SetInvulnerableState(bool isImmortal)
        {
            this.IsImmortal = isImmortal;
        }

        public virtual void SetShieldedState(bool isShielded)
        {
            IsShielded = isShielded;
        }

        protected virtual void ProcessDeath()
        {
            OnDeath?.Invoke(this);
        }

        protected void TriggerDamageMessage(HealthModificationIntentModel healthModificationIntent)
        {
            healthModificationIntent.SetTarget(transform);
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

        public void DealHealthPercentageDamage(float percentage, DamageCritType damageCritType, AttackType attackType)
        {
            float damage = StatCalc.GetPercentage(maxHealth, percentage);
            HealthModificationIntentModel healthModificationIntentModel = new HealthModificationIntentModel(damage, damageCritType, attackType,CalculationType.Flat,null);
            TryDealDamage(healthModificationIntentModel);
        }

        public void TryDealLineDamage(int numberOfLines, float delayBetweenLines, HealthModificationIntentModel healthModificationIntent)
        {
            StartCoroutine(LineDamageRoutine(numberOfLines, delayBetweenLines, healthModificationIntent));
        }

        public IEnumerator LineDamageRoutine(int numberOfLines, float delayBetweenLines, HealthModificationIntentModel healthModificationIntent)
        {
            for (int i = 0; i < numberOfLines; i++)
            {
                TryDealDamage(healthModificationIntent);
                yield return new WaitForSeconds(delayBetweenLines);
            }
        }   
    }
}