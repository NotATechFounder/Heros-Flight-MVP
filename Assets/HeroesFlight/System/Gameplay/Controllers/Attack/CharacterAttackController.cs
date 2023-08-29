using System;
using HeroesFlight.Common;
using HeroesFlight.System.Character;
using HeroesFlight.System.Character.Enum;
using HeroesFlight.System.Gameplay.Data.Animation;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;
using Random = UnityEngine.Random;


namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class CharacterAttackController : MonoBehaviour, IAttackControllerInterface
    {
        [SerializeField] int enemiesToHitPerAttack = 4;
        [SerializeField] OverlapChecker reguarAttackOverlap;
        [SerializeField] OverlapChecker ultAttackOverlap;
        public float Damage => characterController.CharacterStatController.CurrentPhysicalDamage;

        public float TimeSinceLastAttack => m_TimeSinceLastAttack;

        CharacterControllerInterface characterController;
        CharacterAnimationControllerInterface m_CharacterAnimationController;
        AttackRangeVisualsController visualController;
        AttackControllerState m_State;
        CharacterStatController statController;
        PlayerStatData playerStatData = null;
        UltimateData ultimateData;
        AttackData attackData;
      
        float m_TimeSinceLastAttack = 0;
        Vector2 attackPointOffset ;

        Vector2 attackPoint;
       
        bool isDisabled;
        float attackDuration = 0;

        public event Action OnHitTarget;

        public void Init()
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
            attackPoint = (Vector2)transform.position +  (  new Vector2(-1.5f,1) * attackPointOffset);
            visualController.Init(playerStatData.AttackRange);
            visualController.SetPosition(attackPoint);
            isDisabled = false;
            attackDuration = characterController.CharacterSO.AnimationData.AttackAnimation.Animation.Duration;
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
                ?  new Vector2(-1.5f,1) * attackPointOffset
                :  new Vector2(1.5f,1) * attackPointOffset;
            visualController.SetPosition(attackPoint);
            var direction = characterController.IsFacingLeft ? OverlapChecker.Direction.Left : OverlapChecker.Direction.Right;
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


        public void AttackTargets()
        {
            if (m_TimeSinceLastAttack < attackDuration / statController.CurrentAttackSpeed)
            {
                return;
            }


            m_CharacterAnimationController.PlayAttackSequence(statController.CurrentAttackSpeed);
            m_TimeSinceLastAttack = 0;
        }


        void ResetAttack()
        {
            m_CharacterAnimationController.StopAttackSequence();
            m_TimeSinceLastAttack = attackDuration / statController.CurrentAttackSpeed;
        }


        void HandleDamageDealRequest(AnimationEventInterface animationEvent)
        {
            if (animationEvent.Type != AniamtionEventType.Attack)
                return;

            var data = animationEvent as AttackAnimationEvent;

            // attackZoneFilter.FilterEnemies(attackPoint, characterController.IsFacingLeft,
            //     enemiesRetriveCallback?.Invoke(), ref enemiesToAttack, data);

            if (data.AttackType == AttackType.Regular)
            {
                reguarAttackOverlap.Detect();
            }
            else
            {
                ultAttackOverlap.Detect();
            }
           
        }

        private void ApplyLifeSteal()
        {
            if (characterController.CharacterStatController.CurrentLifeSteal <= 0)
                return;
            float healthInc = StatCalc.GetPercentage(characterController.CharacterStatController.PlayerStatData.Health, characterController.CharacterStatController.CurrentLifeSteal);
            characterController.CharacterStatController.ModifyHealth(healthInc, true);
        }

        public void ToggleControllerState(bool isEnabled)
        {
            isDisabled = !isEnabled;
            visualController.DisableVisuals(isDisabled);
        }

       
        void DealUltDamage(int hits, Collider2D[] colliders)
        {
            var baseDamage = Damage * ultimateData.DamageMultiplier;
            for (int i = 0; i < hits; i++)
            {
                if (colliders[i].TryGetComponent<IHealthController>(out var health))
                {
                    float criticalChance = characterController.CharacterStatController.CurrentCriticalHitChance;
                    bool isCritical = Random.Range(0, 100) <= criticalChance;

                    float damageToDeal = isCritical
                        ? baseDamage * characterController.CharacterStatController.CurrentCriticalHitDamage
                        : baseDamage;

                    var type = isCritical ? DamageType.Critical : DamageType.NoneCritical;
                    health.DealDamage(new DamageModel(damageToDeal, type,AttackType.Ultimate));
                    ApplyLifeSteal();
                    OnHitTarget?.Invoke();
                }
            }
            // int maxEnemiesToHit =
            //     data.AttackType == AttackType.Regular ? enemiesToHitPerAttack : ultimateData.EnemiesPerAttack;
           
            // if (enemiesToAttack.Count > 0)
            // {
            //     var enemiesAttacked = 0;
            //     foreach (var enemy in enemiesToAttack)
            //     {
            //         // if (enemiesAttacked >= maxEnemiesToHit)
            //         //     break;
            //
            //         enemiesAttacked++;
            //
            //         float criticalChance = characterController.CharacterStatController.CurrentCriticalHitChance;
            //         bool isCritical = Random.Range(0, 100) <= criticalChance;
            //
            //         float damageToDeal = isCritical
            //             ? baseDamage * characterController.CharacterStatController.CurrentCriticalHitDamage
            //             : baseDamage;
            //
            //         var type = isCritical ? DamageType.Critical : DamageType.NoneCritical;
            //         enemy.DealDamage(new DamageModel(damageToDeal, type,data.AttackType));
            //         ApplyLifeSteal();
            //         OnHitTarget?.Invoke();
            //     }
            // }
        }

        void DealNormalDamage(int hits, Collider2D[] colliders)
        {
            var baseDamage = Damage ;
            for (int i = 0; i < hits; i++)
            {
                if (colliders[i].TryGetComponent<IHealthController>(out var health))
                {
                    float criticalChance = characterController.CharacterStatController.CurrentCriticalHitChance;
                    bool isCritical = Random.Range(0, 100) <= criticalChance;

                    float damageToDeal = isCritical
                        ? baseDamage * characterController.CharacterStatController.CurrentCriticalHitDamage
                        : baseDamage;

                    var type = isCritical ? DamageType.Critical : DamageType.NoneCritical;
                    health.DealDamage(new DamageModel(damageToDeal, type,AttackType.Regular));
                    ApplyLifeSteal();
                    OnHitTarget?.Invoke();
                }
            }
        }
        
     
    }
}