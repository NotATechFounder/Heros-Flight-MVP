using System;
using System.Collections.Generic;
using HeroesFlight.System.Character;
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
        [SerializeField] float attackPointOffset = 1f;

        public float Damage => characterController.CharacterStatController.CurrentPhysicalDamage;

        public float TimeSinceLastAttack => m_TimeSinceLastAttack;


        CharacterControllerInterface characterController;

        CharacterAnimationControllerInterface m_CharacterAnimationController;

        AttackRangeVisualsController visualController;

        AttackControllerState m_State;
        CharacterStatController statController;
        PlayerStatData playerStatData = null;

        float m_TimeSinceLastAttack = 0;

        Vector2 attackPoint;

        Func<List<IHealthController>> enemiesRetriveCallback;
        List<IHealthController> foundedEnemies = new();
        List<IHealthController> enemiesToAttack = new();
        bool isDisabled;
        WaitForSeconds tick;
        float attackDuration = 0;

        void Awake()
        {
            characterController = GetComponent<CharacterControllerInterface>();
            visualController = GetComponent<AttackRangeVisualsController>();
            m_CharacterAnimationController = GetComponent<CharacterAnimationController>();
            statController = GetComponent<CharacterStatController>();
            m_CharacterAnimationController.OnAnimationEvent += HandleDamageDealRequest;
            m_State = AttackControllerState.LookingForTarget;
            playerStatData = characterController.CharacterSO.GetPlayerStatData;
            m_TimeSinceLastAttack = playerStatData.AttackSpeed;
            attackPoint = transform.position + Vector3.up + Vector3.left * attackPointOffset;
            visualController.Init(playerStatData.AttackRange);
            visualController.SetPosition(attackPoint);
            isDisabled = false;
            attackDuration = characterController.CharacterSO.AnimationData.AttackAnimation.Animation.Duration;
            tick = new WaitForSeconds(.25f);
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
                FilterEnemies(enemiesRetriveCallback?.Invoke(), ref foundedEnemies,
                    new AttackAnimationEvent(AttackType.Regular,0));
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

        void FilterEnemies(List<IHealthController> enemies, ref List<IHealthController> enemiesToUpdate,
            AttackAnimationEvent dataAttackType)
        {
            enemiesToUpdate.Clear();
       
            switch (dataAttackType.AttackType)
            {
                case AttackType.Regular:
                    FilterEnemiesForBaseAttack(enemies, ref enemiesToUpdate);
                    
                    break;
                case AttackType.Ultimate_Base:
                    FilterEnemiesForBaseUltimate(enemies,ref  enemiesToUpdate, dataAttackType);
                    break;
                case AttackType.Ultimate_Lancer:
                    break;
               
            }
           
        }

        void FilterEnemiesForBaseAttack(List<IHealthController> enemies, ref List<IHealthController> enemiesToUpdate)
        {
            foreach (var controller in enemies)
            {
                if (Vector2.Distance(controller.currentTransform.position, attackPoint) <= playerStatData.AttackRange)
                {
                    enemiesToUpdate.Add(controller);
                }
            }
        }

        void FilterEnemiesForBaseUltimate(List<IHealthController> enemies,ref List<IHealthController> enemiesToUpdate, AttackAnimationEvent dataAttackType)
        {
                switch (dataAttackType.AttackIndex)
            {
                case 1:
                    foreach (var controller in enemies)
                    {
                        if (characterController.IsFacingLeft)
                        {
                            if (Mathf.Abs(controller.currentTransform.position.y - attackPoint.y) <= playerStatData.AttackRange
                                && controller.currentTransform.position.x <= transform.position.x)
                            {
                                enemiesToUpdate.Add(controller);
                            }
                        }
                        else
                        {
                            if (Mathf.Abs(controller.currentTransform.position.y - attackPoint.y) <= playerStatData.AttackRange
                                && controller.currentTransform.position.x >= transform.position.x)
                            {
                                enemiesToUpdate.Add(controller);
                            }
                        }
                    }

                    break;
                case 2:
                    foreach (var controller in enemies)
                    {
                        if (characterController.IsFacingLeft)
                        {
                            if (Mathf.Abs(controller.currentTransform.position.y - attackPoint.y-1) <= playerStatData.AttackRange
                                && controller.currentTransform.position.x <= transform.position.x)
                            {
                                enemiesToUpdate.Add(controller);
                            }
                        }
                        else
                        {
                            if (Mathf.Abs(controller.currentTransform.position.y - attackPoint.y-1) <= playerStatData.AttackRange
                                && controller.currentTransform.position.x >= transform.position.x)
                            {
                                enemiesToUpdate.Add(controller);
                            }
                        }
                    }

                    break;
                case 3:
                    foreach (var controller in enemies)
                    {
                        if (characterController.IsFacingLeft)
                        {
                            if (Mathf.Abs(controller.currentTransform.position.y - attackPoint.y-1) <= playerStatData.AttackRange
                                && controller.currentTransform.position.x >= transform.position.x - 2.5f)
                            {
                                enemiesToUpdate.Add(controller);
                            }
                        }
                        else
                        {
                            if (Mathf.Abs(controller.currentTransform.position.y - attackPoint.y-1) <= playerStatData.AttackRange
                                && controller.currentTransform.position.x <= transform.position.x + 2.5f)
                            {
                                enemiesToUpdate.Add(controller);
                            }
                        }
                    }

                    break;
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

        public void Init()
        {
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
            
            FilterEnemies(enemiesRetriveCallback?.Invoke(), ref enemiesToAttack,data);
            if (enemiesToAttack.Count > 0)
            {
                var enemiesAttacked = 0;
                foreach (var enemy in enemiesToAttack)
                {
                    if (enemiesAttacked >= enemiesToHitPerAttack)
                        break;

                    enemiesAttacked++;

                    float criticalChance = characterController.CharacterStatController.CurrentCriticalHitChance;
                    bool isCritical = Random.Range(0, 100) <= criticalChance;

                    float damageToDeal = isCritical
                        ? Damage * characterController.CharacterStatController.CurrentCriticalHitDamage
                        : Damage;
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
        }
    }
}