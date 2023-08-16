using System;
using System.Collections.Generic;
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
        AttackZoneEnemiesFilterInterface attackZoneFilter;
        float m_TimeSinceLastAttack = 0;
        float attackPointOffset = 1f;

        Vector2 attackPoint;

        Func<List<IHealthController>> enemiesRetriveCallback;
        List<IHealthController> foundedEnemies = new();
        List<IHealthController> enemiesToAttack = new();
        bool isDisabled;
        float attackDuration = 0;


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
            attackPointOffset = attackData.AttackPositionOffset;
            m_TimeSinceLastAttack = playerStatData.AttackSpeed;
            attackPoint = transform.position + Vector3.up + Vector3.left * attackPointOffset;
            visualController.Init(playerStatData.AttackRange);
            visualController.SetPosition(attackPoint);
            isDisabled = false;
            attackDuration = characterController.CharacterSO.AnimationData.AttackAnimation.Animation.Duration;
            attackZoneFilter = new AttackZoneFilter(characterController.CharacterSO,transform);
        }

        public void SetCallback(Func<List<IHealthController>> enemiesCallback)
        {
            enemiesRetriveCallback = enemiesCallback;
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
                ? transform.position + Vector3.up + Vector3.left * attackPointOffset
                : transform.position + Vector3.up + Vector3.right * attackPointOffset;

            visualController.SetPosition(attackPoint);

            ProcessAttackLogic();
        }


        void ProcessAttackLogic()
        {
            if (isDisabled)
            {
                foundedEnemies.Clear();
            }
            else
            {
                attackZoneFilter.FilterEnemies(attackPoint, characterController.IsFacingLeft,
                    enemiesRetriveCallback?.Invoke(), ref foundedEnemies,
                    new AttackAnimationEvent(AttackType.Regular, 0));
            }

            if (foundedEnemies.Count > 0)
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

            attackZoneFilter.FilterEnemies(attackPoint, characterController.IsFacingLeft,
                enemiesRetriveCallback?.Invoke(), ref enemiesToAttack, data);

            int maxEnemiesToHit =
                data.AttackType == AttackType.Regular ? enemiesToHitPerAttack : ultimateData.EnemiesPerAttack;
            var baseDamage = data.AttackType == AttackType.Regular ? Damage : Damage * ultimateData.DamageMultiplier;
            if (enemiesToAttack.Count > 0)
            {
                var enemiesAttacked = 0;
                foreach (var enemy in enemiesToAttack)
                {
                    // if (enemiesAttacked >= maxEnemiesToHit)
                    //     break;

                    enemiesAttacked++;

                    float criticalChance = characterController.CharacterStatController.CurrentCriticalHitChance;
                    bool isCritical = Random.Range(0, 100) <= criticalChance;

                    float damageToDeal = isCritical
                        ? baseDamage * characterController.CharacterStatController.CurrentCriticalHitDamage
                        : baseDamage;
                    var type = isCritical ? DamageType.Critical : DamageType.NoneCritical;
                    enemy.DealDamage(new DamageModel(damageToDeal, type));
                }
            }
        }

        public void ToggleControllerState(bool isEnabled)
        {
            isDisabled = !isEnabled;
            visualController.DisableVisuals(isDisabled);
        }

        void OnDrawGizmos()
        {
            if (playerStatData == null || characterController == null)
                return;
            var checkPosition = characterController.IsFacingLeft
                ? transform.position + Vector3.up + Vector3.left * attackPointOffset
                : transform.position + Vector3.up + Vector3.right * attackPointOffset;
            Gizmos.DrawWireSphere(checkPosition, playerStatData.AttackRange);

            var ultimatePosition = characterController.IsFacingLeft
                ? checkPosition + (Vector3.left * ultimateData.OffsetMultiplier)
                : checkPosition + (Vector3.right * ultimateData.OffsetMultiplier);
            switch (characterController.CharacterSO.CharacterType)
            {
                case CharacterType.Tagon:
                    Gizmos.DrawWireSphere(ultimatePosition, playerStatData.AttackRange * ultimateData.RangeMultiplier);
                    break;
                case CharacterType.Lancer:
                    float totalFOV = 45.0f;
                    float rayRange =Vector2.Distance(transform.position,attackPoint)+ playerStatData.AttackRange * ultimateData.RangeMultiplier;
                    float halfFOV = totalFOV / 2.0f;
                    Quaternion leftRayRotation = Quaternion.AngleAxis( -halfFOV, Vector3.forward );
                    Quaternion rightRayRotation = Quaternion.AngleAxis( halfFOV, Vector3.forward );
                    Vector3 facingvector = characterController.IsFacingLeft ? Vector3.left : Vector3.right;
                    Vector3 leftRayDirection = leftRayRotation * facingvector;
                    Vector3 rightRayDirection = rightRayRotation * facingvector;
                    Gizmos.DrawRay(  transform.position + Vector3.up, leftRayDirection * rayRange );
                    Gizmos.DrawRay(  transform.position + Vector3.up, rightRayDirection * rayRange );
                    Gizmos.DrawWireSphere(attackPoint, playerStatData.AttackRange * ultimateData.RangeMultiplier);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
          
        }
    }
}