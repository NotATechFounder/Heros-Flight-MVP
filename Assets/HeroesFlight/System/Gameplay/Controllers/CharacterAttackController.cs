using System;
using System.Collections.Generic;
using HeroesFlight.Common;
using HeroesFlight.System.Character;
using HeroesFlight.System.Gameplay.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class CharacterAttackController : MonoBehaviour, IAttackControllerInterface
    {
        [SerializeField] LayerMask m_TargetMask;
        Collider2D[] m_FoundedColliders;
        CharacterControllerInterface controller;
        CharacterAnimationControllerInterface m_CharacterAnimationController;

        [SerializeField] AttackControllerState m_State;

        CombatModel combatModel;
        IHealthController currentTarget;

        [SerializeField] float m_TimeSinceLastAttack = 0;

        public int Damage => controller.Data.CombatModel.Damage;
        public float TimeSinceLastAttack => m_TimeSinceLastAttack;

        void Awake()
        {
            controller = GetComponent<CharacterControllerInterface>();
            m_CharacterAnimationController = GetComponent<CharacterAnimationController>();
            m_CharacterAnimationController.OnDealDamageRequest += HandleDamageDealRequest;
            m_FoundedColliders = new Collider2D[10];
            m_State = AttackControllerState.LookingForTarget;
            combatModel = controller.Data.CombatModel;
            m_TimeSinceLastAttack = combatModel.TimeBetweenAttacks;
        }

        void Update()
        {
            m_TimeSinceLastAttack += Time.deltaTime;
            ProcessCurrentState();
        }

        public void AttackTarget()
        {
            m_CharacterAnimationController.PlayAttackSequence();
            currentTarget.DealDamage(Damage);
            m_TimeSinceLastAttack = 0;
        }

        void ProcessCurrentState()
        {
            switch (m_State)
            {
                case AttackControllerState.Attacking:
                    ProcessAttackingState();
                    break;
                case AttackControllerState.LookingForTarget:
                    ProcessLookingState();
                    break;
            }
        }


        void ProcessLookingState()
        {
            var foundedTargetsCount =
                Physics2D.OverlapCircleNonAlloc(transform.position, combatModel.AttackRange,
                    m_FoundedColliders,
                    m_TargetMask);
            if (foundedTargetsCount > 0)
            {
                var foundedValidTargets = new List<IHealthController>();
                foreach (var collider in m_FoundedColliders)
                {
                    if (collider == null)
                        continue;

                    if (collider.TryGetComponent<IHealthController>(out var healthController))
                    {
                        if (!healthController.IsDead())
                            foundedValidTargets.Add(healthController);
                    }
                }

                if (foundedValidTargets.Count > 0)
                {
                    currentTarget = foundedValidTargets[0];
                    ChangeState(AttackControllerState.Attacking);
                }
            }
            else
            {
                currentTarget = null;
                ChangeState(AttackControllerState.LookingForTarget);
            }
        }

        void ProcessAttackingState()
        {
            if (currentTarget.IsDead())
            {
                m_CharacterAnimationController.StopAttackSequence();
                currentTarget = null;
                ChangeState(AttackControllerState.LookingForTarget);
                return;
            }

            if (m_TimeSinceLastAttack < combatModel.TimeBetweenAttacks)
                return;

            var targetSize = m_FoundedColliders[0].bounds.extents;
            var distanceToTarget = Vector2.Distance(transform.position,
                m_FoundedColliders[0].transform.position);
            if (distanceToTarget - targetSize.x > combatModel.AttackRange
                || distanceToTarget - targetSize.y > combatModel.AttackRange)
            {
                m_FoundedColliders[0] = null;
                currentTarget = null;
                m_CharacterAnimationController.StopAttackSequence();
                ChangeState(AttackControllerState.LookingForTarget);
                return;
            }


            AttackTarget();
        }

        void ChangeState(AttackControllerState newState)
        {
            if (m_State == newState)
                return;

            switch (newState)
            {
                case AttackControllerState.Attacking:
                    break;
                case AttackControllerState.LookingForTarget:
                    m_TimeSinceLastAttack = combatModel.TimeBetweenAttacks;
                    break;
            }

            m_State = newState;
        }

        void HandleDamageDealRequest(string attackId)
        {
            currentTarget.DealDamage(Damage);
        }

        void OnDrawGizmos()
        {
            if (combatModel == null)
                return;
            Gizmos.DrawWireSphere(transform.position, combatModel.AttackRange);
        }
    }
}