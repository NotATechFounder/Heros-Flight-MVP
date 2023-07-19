using System;
using HeroesFlight.Common;
using HeroesFlight.Common.Enum;
using UnityEngine;

namespace HeroesFlight.System.Character
{
    public class CharacterAttackController : MonoBehaviour, IAttackController
    {
        [SerializeField] LayerMask m_TargetMask;
        [SerializeField] float m_AttackRange;
        [SerializeField] float m_TimeBetweenAttacks;
        [SerializeField] Collider2D[] m_FoundedColliders;
        public event Action<IHealthController> OnAttackTarget;
        public event Action OnStopAttack;

        IHealthController m_Target;
       [SerializeField] AttackControllerState m_State;
        float m_TimeSinceLastAttack = 0;

        void Awake()
        {
            m_FoundedColliders = new Collider2D[10];
            m_State = AttackControllerState.LookingForTarget;
            m_TimeSinceLastAttack = m_TimeBetweenAttacks;
        }

        void Update()
        {
            ProcessCurrentState();
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
                    Physics2D.OverlapCircleNonAlloc(transform.position, m_AttackRange, m_FoundedColliders,
                        m_TargetMask);
                if (foundedTargetsCount > 0)
                {
                    ChangeState(AttackControllerState.Attacking);
                }
            }
        }

        void ProcessAttackingState()
        {
            var targetSize = m_FoundedColliders[0].bounds.extents;
            var distanceToTarget = Vector2.Distance(transform.position, m_FoundedColliders[0].transform.position);
            if (distanceToTarget - targetSize.x > m_AttackRange || distanceToTarget - targetSize.y > m_AttackRange)
            {
                m_Target = null;
                m_FoundedColliders[0] = null;
                OnStopAttack?.Invoke();
                ChangeState(AttackControllerState.LookingForTarget);
                return;
            }


            if (m_TimeSinceLastAttack >= m_TimeBetweenAttacks)
            {
                OnAttackTarget?.Invoke(m_Target);
                m_TimeSinceLastAttack = 0;
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
                    m_TimeSinceLastAttack = m_TimeBetweenAttacks;
                    break;
            }
            m_State = newState;
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, m_AttackRange);
        }
    }
}