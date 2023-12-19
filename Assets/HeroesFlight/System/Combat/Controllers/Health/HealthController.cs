using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Combat.Model;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Controllers;
using UnityEngine;
using Random = UnityEngine.Random;

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

        [Header("Hit Settings")] [SerializeField]
        protected bool useHit;

        [SerializeField] protected int maxHit;
        [SerializeField] protected int currentHit;

        [Header("Immortality")] [SerializeField]
        protected bool becomeImmortalOnHit;

        [SerializeField] protected float immortalityDuration = .5f;
        [SerializeField] private bool isCurrentlyImmortalByTime;
        [SerializeField] private bool isCurrentlyImmortalByState;
        public event Action<Transform> OnBeingHitDamaged;

        public int MaxHit => maxHit;
        public int CurrentHit => currentHit;

        public bool IsImmortal
        {
            get { return isCurrentlyImmortalByTime; }
            protected set { isCurrentlyImmortalByTime = value; }
        }

        public bool IsShielded { get; protected set; }
        public HealthType HealthType => healthType;
        public Transform HealthTransform => transform;
        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public float CurrentHealthProportion => (float)currentHealth / maxHealth;
        public float ImmortalityDuration => immortalityDuration;
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

        /// <summary>
        /// Tries to deal damage with the given health modification intent.
        /// </summary>
        /// <param name="healthModificationIntent">The health modification intent model to deal damage with.</param>
        public virtual void TryDealDamage(HealthModificationIntentModel healthModificationIntent)
        {
            if (CanAvoidDamage(healthModificationIntent))
            {
                return;
            }

            ProcessDamage(healthModificationIntent);
        }

        /// <summary>
        /// Determines if an entity can avoid damage based on the provided health modification intent.
        /// </summary>
        /// <param name="healthModificationIntent">The health modification intent model.</param>
        /// <returns>True if the entity can avoid damage, otherwise false.</returns>
        private bool CanAvoidDamage(HealthModificationIntentModel healthModificationIntent)
        {
            return IsDead() ||
                   CheckIfAlreadyImmortal(healthModificationIntent) ||
                   HasDodgedAttack() ||
                   ProccesHitTypedHealth() ||
                   ProcessShieldedState(healthModificationIntent);
        }

        /// <summary>
        /// Process the damage received by the entity.
        /// </summary>
        /// <param name="healthModificationIntent">The health modification intent.</param>
        private void ProcessDamage(HealthModificationIntentModel healthModificationIntent)
        {
            UpdateImmortalityState(healthModificationIntent);
            UpdateIntentsModelTarget(healthModificationIntent);
            InvokeDamageReceiveRequest(healthModificationIntent);
        }

        /// <summary>
        /// Invokes the damage receive request event, passing the health modification intent.
        /// </summary>
        /// <param name="healthModificationIntent">The health modification intent model.</param>
        private void InvokeDamageReceiveRequest(HealthModificationIntentModel healthModificationIntent)
        {
            OnDamageReceiveRequest?.Invoke(new HealthModificationRequestModel(healthModificationIntent, this));
        }

     

        private void UpdateIntentsModelTarget(HealthModificationIntentModel healthModificationIntent)
        {
            healthModificationIntent.SetTarget(transform);
        }

        private bool ProccesHitTypedHealth()
        {
            if (useHit)
            {
                DealHit();
                return true;
            }

            return false;
        }

        private bool ProcessShieldedState(HealthModificationIntentModel healthModificationIntent)
        {
            if (IsShielded)
            {
                OnHitWhileIsShielded?.Invoke(healthModificationIntent);
                return true;
            }

            return false;
        }

        private bool HasDodgedAttack()
        {
            if (DodgeAttack())
            {
                OnDodged?.Invoke();
                return true;
            }

            return false;
        }

        private void UpdateImmortalityState(HealthModificationIntentModel healthModificationIntent)
        {
            if (becomeImmortalOnHit && healthModificationIntent.AttackType != AttackType.DoT)
            {
                IsImmortal = true;
                currentImmortalityDuration = immortalityDuration;
            }
        }

        private bool CheckIfAlreadyImmortal(HealthModificationIntentModel healthModificationIntent)
        {
            if (isCurrentlyImmortalByTime || isCurrentlyImmortalByState)
            {
                return true;
            }


            return false;
        }

        public virtual void ModifyHealth(HealthModificationIntentModel modificationIntentModel)
        {
            if (modificationIntentModel.AttackType != AttackType.Healing)
            {
                var resultDamage = modificationIntentModel.Amount -
                                   StatCalc.GetPercentage(modificationIntentModel.Amount, defence);
                currentHealth -= resultDamage;
            }
            else
            {
                currentHealth += modificationIntentModel.Amount;
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
                var intent = new HealthModificationIntentModel(amount,
                    DamageCritType.NoneCritical, AttackType.Healing, CalculationType.Flat, null);
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


        public virtual void Revive(float healthPercentage)
        {
            currentHealth = maxHealth / 100 * healthPercentage;
            heathBarUI?.ChangeValue((float)currentHealth / maxHealth);
            currentHit = maxHit;
        }

        public virtual void SetInvulnerableState(bool isImmortal)
        {
            isCurrentlyImmortalByState = isImmortal;
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


        protected void SetMaxHealth(float health)
        {
            maxHealth = health;
            heathBarUI?.ChangeValue((float)currentHealth / maxHealth);
        }

        protected bool DodgeAttack()
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
            HealthModificationIntentModel healthModificationIntentModel =
                new HealthModificationIntentModel(damage, damageCritType, attackType, CalculationType.Flat, null);
            TryDealDamage(healthModificationIntentModel);
        }

        public virtual void ReactToDamage(AttackType attackType)
        {
            flashEffect?.Flash(immortalityDuration);
        }
    }
}