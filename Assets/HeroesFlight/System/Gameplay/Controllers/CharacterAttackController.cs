using System;
using HeroesFlight.System.Character;
using HeroesFlight.System.Gameplay.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class CharacterAttackController : MonoBehaviour, IAttackControllerInterface
    {
        [SerializeField] LayerMask m_TargetMask;
        [SerializeField] float m_AttackRange;
        [SerializeField] float m_TimeBetweenAttacks;
        [SerializeField] Collider2D[] m_FoundedColliders;
        public event Action OnAttackAnimation;
        public event Action<IAttackControllerInterface,IHealthController> OnDealDamageRequest;

        public event Action OnStopAttack;

        IHealthController m_Target;

        CharacterAnimationControllerInterface m_CharacterAnimationController;

        [SerializeField] AttackControllerState m_State;

        IHealthController m_CurrentTarget;

        float m_TimeSinceLastAttack = 0;

        public int Damage { get; }
        public float TimeSinceLastAttack => m_TimeSinceLastAttack;

        void Awake()
        {
            m_CharacterAnimationController = GetComponent<CharacterAnimationController>();
            m_CharacterAnimationController.OnDealDamageRequest += HandleDamageDealRequest;
            m_FoundedColliders = new Collider2D[10];
            m_State = AttackControllerState.LookingForTarget;
            m_TimeSinceLastAttack = m_TimeBetweenAttacks;
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
                    Physics2D.OverlapCircleNonAlloc(transform.position, m_AttackRange, m_FoundedColliders,
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
            if (distanceToTarget - targetSize.x > m_AttackRange || distanceToTarget - targetSize.y > m_AttackRange)
            {
                m_Target = null;
                m_FoundedColliders[0] = null;
                m_CurrentTarget = null;
                m_CharacterAnimationController.StopAttackSequence();
                ChangeState(AttackControllerState.LookingForTarget);
                return;
            }

            
            if (m_TimeSinceLastAttack >= m_TimeBetweenAttacks)
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
                    m_TimeSinceLastAttack = m_TimeBetweenAttacks;
                    break;
            }
            m_State = newState;
        }

        void HandleDamageDealRequest(string attackId)
        {
            OnDealDamageRequest?.Invoke(this,m_CurrentTarget);
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, m_AttackRange);
        }
    }
}