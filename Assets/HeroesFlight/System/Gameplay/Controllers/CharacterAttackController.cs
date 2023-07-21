using System;
using HeroesFlight.Common;
using HeroesFlight.System.Character;
using HeroesFlight.System.Gameplay.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class CharacterAttackController : MonoBehaviour, IAttackControllerInterface
    {
        [SerializeField] LayerMask m_TargetMask;
        [SerializeField] Collider2D[] m_FoundedColliders;
        IHealthController m_Target;
        ICharacterController controller;
        CharacterAnimationControllerInterface m_CharacterAnimationController;

        [SerializeField] AttackControllerState m_State;

        CombatModel combatModel;
        IHealthController m_CurrentTarget;

        float m_TimeSinceLastAttack = 0;

        public int Damage => controller.Data.CombatModel.Damage;
        public float TimeSinceLastAttack => m_TimeSinceLastAttack;

        void Awake()
        {
            controller = GetComponent<ICharacterController>();
            m_CharacterAnimationController = GetComponent<CharacterAnimationController>();
            m_CharacterAnimationController.OnDealDamageRequest += HandleDamageDealRequest;
            m_FoundedColliders = new Collider2D[10];
            m_State = AttackControllerState.LookingForTarget;
            combatModel = controller.Data.CombatModel;
            m_TimeSinceLastAttack =  combatModel.TimeBetweenAttacks;
        }

        void Update()
        {
            ProcessCurrentState();
        }

        public void AttackTarget()
        {
            m_CharacterAnimationController.PlayAttackSequence();
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
            if (m_FoundedColliders[0] == null)
            {
                var foundedTargetsCount =
                    Physics2D.OverlapCircleNonAlloc(transform.position,  combatModel.AttackRange,
                        m_FoundedColliders,
                        m_TargetMask);
                if (foundedTargetsCount > 0)
                {
                    m_CurrentTarget = m_FoundedColliders[0].GetComponent<IHealthController>();
                    ChangeState(AttackControllerState.Attacking);
                }
            }
        }

        void ProcessAttackingState()
        {
            var targetSize = m_FoundedColliders[0].bounds.extents;
            var distanceToTarget = Vector2.Distance(transform.position, 
                m_FoundedColliders[0].transform.position);
            if (distanceToTarget - targetSize.x >  combatModel.AttackRange
                || distanceToTarget - targetSize.y > combatModel.AttackRange)
            {
                m_Target = null;
                m_FoundedColliders[0] = null;
                m_CurrentTarget = null;
                m_CharacterAnimationController.StopAttackSequence();
                ChangeState(AttackControllerState.LookingForTarget);
                return;
            }

            
            if (m_TimeSinceLastAttack >= combatModel.TimeBetweenAttacks)
            {
                AttackTarget();
            }

            m_TimeSinceLastAttack += Time.deltaTime;
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
           m_Target.DealDamage(Damage);
        }

        void OnDrawGizmos()
        {
            if (combatModel == null)
                return;
            Gizmos.DrawWireSphere(transform.position, combatModel.AttackRange);
        }
    }
}