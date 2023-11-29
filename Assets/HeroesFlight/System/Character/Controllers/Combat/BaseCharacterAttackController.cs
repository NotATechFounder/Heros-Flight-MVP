using System;
using HeroesFlight.Common;
using HeroesFlight.Common.Animation;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Character;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;
using Random = UnityEngine.Random;


namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BaseCharacterAttackController : MonoBehaviour, IAttackControllerInterface
    {
        [SerializeField] int enemiesToHitPerAttack = 4;
        [SerializeField] protected OverlapChecker reguarAttackOverlap;
        [SerializeField] protected OverlapChecker ultAttackOverlap;
        [SerializeField] WorldBarUI specialBar;
        public event Action<AttackControllerState> OnStateChange;

        public float Damage => characterController.CharacterStatController.GetStatModel.GetCurrentStatValue(StatType.PhysicalDamage);

        public float TimeSinceLastAttack => m_TimeSinceLastAttack;

        WorldBarUI IAttackControllerInterface.specialBar => specialBar;

        protected CharacterControllerInterface characterController;
        protected CharacterAnimationControllerInterface m_CharacterAnimationController;
        protected AttackRangeVisualsController visualController;
        protected AttackControllerState m_State;
        protected CharacterStatController statController;
        protected PlayerStatData playerStatData = null;
        protected UltimateData ultimateData;
        protected AttackData attackData;
        protected IHealthController healthController;
        float m_TimeSinceLastAttack = 0;
        Vector2 attackPointOffset;

        Vector2 attackPoint;

        bool isDisabled;
        float attackDuration = 0;

        public event Action OnHitTarget;

        public virtual void Init()
        {
            characterController = GetComponent<CharacterControllerInterface>();
            visualController = GetComponent<AttackRangeVisualsController>();
            m_CharacterAnimationController = GetComponent<CharacterAnimationController>();
            statController = GetComponent<CharacterStatController>();
            m_CharacterAnimationController.OnAnimationEvent += HandleDamageDealRequest;
            m_State = AttackControllerState.LookingForTarget;
            playerStatData = characterController.CharacterSO.GetPlayerStatData;
            ultimateData = characterController.CharacterSO.UltimateData;
            attackData = characterController.CharacterSO.AttackData;
            enemiesToHitPerAttack = attackData.EnemiesPerAttack;
            attackPointOffset = reguarAttackOverlap.Offset;
            m_TimeSinceLastAttack = playerStatData.AttackSpeed;
            attackPoint = (Vector2)transform.position + (new Vector2(-1.5f, 1) * attackPointOffset);
            visualController.Init(reguarAttackOverlap.GetSizeX());
            healthController = GetComponent<HealthController>();
            // visualController.SetPosition(attackPoint);
            isDisabled = false;
            //attackDuration = characterController.CharacterSO.CharacterAnimations.AttackAnimation.Animation.Duration;
            attackDuration = 999;
            reguarAttackOverlap.OnDetect += DealNormalDamage;
            ultAttackOverlap.OnDetect += DealUltDamage;
        }


        void Update()
        {
            if (isDisabled)
            {
                ResetAttack();
                return;
            }

            m_TimeSinceLastAttack += Time.deltaTime;


            attackPoint = characterController.IsFacingLeft
                ? new Vector2(-1f, 1) * attackPointOffset
                : new Vector2(1f, 1) * attackPointOffset;
            visualController.SetPosition(attackPoint);
            var direction = characterController.IsFacingLeft
                ? OverlapChecker.Direction.Left
                : OverlapChecker.Direction.Right;
            //visualController.SetFacing(characterController.IsFacingLeft);
            reguarAttackOverlap.SetDirection(direction);
            ultAttackOverlap.SetDirection(direction);
            ProcessAttackLogic();
        }


        void ProcessAttackLogic()
        {
            if (reguarAttackOverlap.TargetInRange())
            {
                AttackTargets();
            }
            else
            {
                ResetAttack();
            }
        }


        public void AttackTargets(Action onComplete = null)
        {
            if (!CanAttack())
            {
                return;
            }


            attackDuration = m_CharacterAnimationController.PlayAttackSequence(statController.GetStatModel.GetCurrentStatValue(StatType.AttackSpeed));
            m_TimeSinceLastAttack = 0;
        }


        void ResetAttack()
        {
            m_CharacterAnimationController.StopAttackSequence();
            m_TimeSinceLastAttack = attackDuration / statController.GetStatModel.GetCurrentStatValue(StatType.AttackSpeed);
        }


        void HandleDamageDealRequest(AnimationEventInterface animationEvent)
        {
            if (animationEvent.Type != AniamtionEventType.Attack)
                return;

            var data = animationEvent as AttackAnimationEvent;

            if (data.AttackType == AttackType.Regular)
            {
               reguarAttackOverlap.DetectOverlap();
            }
            else
            {
                ultAttackOverlap.DetectOverlap();
            }        
        }

        protected void ApplyLifeSteal()
        {
            if (characterController.CharacterStatController.GetStatModel.GetCurrentStatValue(StatType.LifeSteal) <= 0)
                return;
            float healthInc = StatCalc.GetPercentage(characterController.CharacterStatController.PlayerStatData.Health,
                characterController.CharacterStatController.GetStatModel.GetCurrentStatValue(StatType.LifeSteal));
            characterController.CharacterStatController.ModifyHealth(healthInc, true);
        }

        public void ToggleControllerState(bool isEnabled)
        {
            isDisabled = !isEnabled;
          //  visualController.DisableVisuals(isDisabled);
        }

        public bool CanAttack()
        {
            return m_TimeSinceLastAttack > attackDuration / statController.GetStatModel.GetCurrentStatValue(StatType.AttackSpeed);
        }


        protected virtual void DealUltDamage(int hits, Collider2D[] colliders)
        {
            var baseDamage = Damage * ultimateData.DamageMultiplier;
            for (int i = 0; i < hits; i++)
            {
                if (colliders[i].TryGetComponent<IHealthController>(out var health))
                {
                    float criticalChance = characterController.CharacterStatController.GetStatModel.GetCurrentStatValue(StatType.AttackSpeed);
                    bool isCritical = Random.Range(0, 100) <= criticalChance;

                    float damageToDeal = isCritical
                        ? baseDamage * characterController.CharacterStatController.GetStatModel.GetCurrentStatValue(StatType.CriticalHitDamage)
                        : baseDamage;

                    var type = isCritical ? DamageCritType.Critical : DamageCritType.NoneCritical;
                    health.TryDealDamage(new HealthModificationIntentModel(damageToDeal,
                        type, AttackType.Ultimate, CalculationType.Flat,health));
                    ApplyLifeSteal();
                    OnHitTarget?.Invoke();
                }
            }
        }

        protected virtual void DealNormalDamage(int hits, Collider2D[] colliders)
        {
            var baseDamage = Damage;
            for (int i = 0; i < hits; i++)
            {
                if (colliders[i].TryGetComponent<IHealthController>(out var health))
                {
                    float criticalChance = characterController.CharacterStatController.GetStatModel.GetCurrentStatValue(StatType.CriticalHitChance);
                    bool isCritical = Random.Range(0, 100) <= criticalChance;

                    float damageToDeal = isCritical
                        ? baseDamage * characterController.CharacterStatController.GetStatModel.GetCurrentStatValue(StatType.CriticalHitDamage)
                        : baseDamage;

                    var type = isCritical ? DamageCritType.Critical : DamageCritType.NoneCritical;
                    health.TryDealDamage(new HealthModificationIntentModel(damageToDeal,
                        type, AttackType.Regular, CalculationType.Flat,this.healthController));
                    ApplyLifeSteal();
                    OnHitTarget?.Invoke();
                }
            }
        }
    }
}