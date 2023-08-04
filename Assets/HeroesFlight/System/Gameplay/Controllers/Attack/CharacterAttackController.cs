using System;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.Common;
using HeroesFlight.System.Character;
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

        public float Damage => controller.CharacterStatController.CurrentPhysicalDamage;

        public float TimeSinceLastAttack => m_TimeSinceLastAttack;


        CharacterControllerInterface controller;

        CharacterAnimationControllerInterface m_CharacterAnimationController;

        AttackRangeVisualsController visualController;

        AttackControllerState m_State;

        PlayerStatData playerStatData = null;

        float m_TimeSinceLastAttack = 0;

        Vector2 attackPoint;

        Func<List<IHealthController>> enemiesRetriveCallback;
        List<IHealthController> foundedEnemies = new();
        List<IHealthController> enemiesToAttack = new();
        bool isDisabled;
        WaitForSeconds tick;

        void Awake()
        {
            controller = GetComponent<CharacterControllerInterface>();
            visualController = GetComponent<AttackRangeVisualsController>();
            m_CharacterAnimationController = GetComponent<CharacterAnimationController>();
            m_CharacterAnimationController.OnDealDamageRequest += HandleDamageDealRequest;
            m_State = AttackControllerState.LookingForTarget;
            playerStatData = controller.CharacterSO.GetPlayerStatData;
            m_TimeSinceLastAttack = playerStatData.TimeBetweenAttacks;
            attackPoint = transform.position + Vector3.up + Vector3.left * attackPointOffset;
            visualController.Init(playerStatData.AttackRange);
            visualController.SetPosition(attackPoint);
            isDisabled = false;
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

            attackPoint = controller.IsFacingLeft
                ? transform.position + Vector3.up + Vector3.left * attackPointOffset
                : transform.position + Vector3.up + Vector3.right * attackPointOffset;

            visualController.SetPosition(attackPoint);

            ProcessAttackLogic();
        }


      

        void ProcessAttackLogic()
        {
            FilterEnemies(enemiesRetriveCallback?.Invoke(), ref foundedEnemies);
            if (foundedEnemies.Count > 0)
            {
                AttackTargets();
            }
            else
            {
                ResetAttack();
            }
        }

        void FilterEnemies(List<IHealthController> enemies, ref List<IHealthController> enemiesToUpdate)
        {
            enemiesToUpdate.Clear();
            foreach (var controller in enemies)
            {
                if (Vector2.Distance(controller.currentTransform.position, attackPoint) <= playerStatData.AttackRange)
                {
                    enemiesToUpdate.Add(controller);
                }
            }
        }


        public void AttackTargets()
        {
            if (m_TimeSinceLastAttack < playerStatData.TimeBetweenAttacks)
            {
                return;
            }


            m_CharacterAnimationController.PlayAttackSequence(1f);
            m_TimeSinceLastAttack = 0;
        }

        public void Init()
        {
        }


        void ResetAttack()
        {
            m_CharacterAnimationController.StopAttackSequence();
            m_TimeSinceLastAttack = playerStatData.TimeBetweenAttacks;
        }


        void HandleDamageDealRequest(string attackId)
        {
            FilterEnemies(enemiesRetriveCallback?.Invoke(), ref enemiesToAttack);
            if (enemiesToAttack.Count > 0)
            {
                var enemiesAttacked = 0;
                foreach (var enemy in enemiesToAttack)
                {
                    if (enemiesAttacked >= enemiesToHitPerAttack)
                        break;

                    enemiesAttacked++;

                    float criticalChance = controller.CharacterStatController.CurrentCriticalHitChance;
                    bool isCritical = Random.Range(0, 100) <= criticalChance;

                    float damageToDeal = isCritical ? Damage * controller.CharacterStatController.CurrentCriticalHitDamage : Damage;
                    var type = isCritical ? DamageType.Critical : DamageType.NoneCritical;
                    enemy.DealDamage(new DamageModel(damageToDeal,type));
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
            if (playerStatData == null || controller==null)
                return;
            var checkPosition = controller.IsFacingLeft
                ? transform.position + Vector3.up + Vector3.left * attackPointOffset
                : transform.position + Vector3.up + Vector3.right * attackPointOffset;
            Gizmos.DrawWireSphere(checkPosition, playerStatData.AttackRange);
        }
    }
}