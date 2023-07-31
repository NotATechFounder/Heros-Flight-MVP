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

        public int Damage => controller.Data.CombatModel.Damage;

        public float TimeSinceLastAttack => m_TimeSinceLastAttack;


        CharacterControllerInterface controller;

        CharacterAnimationControllerInterface m_CharacterAnimationController;

        AttackRangeVisualsController visualController;

        AttackControllerState m_State;

        CombatModel combatModel;

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
            combatModel = controller.Data.CombatModel;
            m_TimeSinceLastAttack = combatModel.TimeBetweenAttacks;
            attackPoint = transform.position + Vector3.up + Vector3.left * attackPointOffset;
            visualController.Init(combatModel.AttackRange);
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


        IEnumerator AttackLogicRoutine()
        {
            while (true)
            {
                ProcessAttackLogic();
                yield return tick;
            }
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
                if (Vector2.Distance(controller.currentTransform.position, attackPoint) <= combatModel.AttackRange)
                {
                    enemiesToUpdate.Add(controller);
                }
            }
        }


        public void AttackTargets()
        {
            if (m_TimeSinceLastAttack < combatModel.TimeBetweenAttacks)
            {
                return;
            }


            m_CharacterAnimationController.PlayAttackSequence();
            m_TimeSinceLastAttack = 0;
        }

        public void Init()
        {
        }


        void ResetAttack()
        {
            m_CharacterAnimationController.StopAttackSequence();
            m_TimeSinceLastAttack = combatModel.TimeBetweenAttacks;
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

                    var rng = Random.Range(0, 101);
                    var isCritical = rng <= 30;
                    int damageToDeal = isCritical ? Damage * 2 : Damage;
                    var type = isCritical ? DamageType.Critical : DamageType.NoneCritical;
                    enemy.DealDamage(new DamageModel(damageToDeal,type));
                }
            }
        }

        public void DisableActions()
        {
            isDisabled = true;
        }

        void OnDrawGizmos()
        {
            if (combatModel == null)
                return;
            var checkPosition = controller.IsFacingLeft
                ? transform.position + Vector3.up + Vector3.left * attackPointOffset
                : transform.position + Vector3.up + Vector3.right * attackPointOffset;
            Gizmos.DrawWireSphere(checkPosition, combatModel.AttackRange);
        }
    }
}