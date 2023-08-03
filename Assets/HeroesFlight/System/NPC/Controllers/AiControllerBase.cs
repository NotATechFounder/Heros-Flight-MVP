using System;
using HeroesFlight.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Data;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public abstract class AiControllerBase : MonoBehaviour, AiControllerInterface
    {
        [SerializeField] protected AiAgentModel m_Model;
        [SerializeField] protected float wanderDistance = 10f;
        [SerializeField] protected float knockbackForce = 10f;
        protected AiViewController viewController;
        protected AiAnimatorInterface animator;
        protected Collider2D attackCollider;
        public event Action OnInitialized;
        public event Action OnDisabled;
        public EnemyType EnemyType => m_Model.EnemyType;
        public AiAgentModel AgentModel => m_Model;
        public Transform CurrentTarget => currentTarget;

        protected Rigidbody2D rigidBody;
        protected Transform currentTarget;
        protected bool isDisabled;
        bool canAttack;
        Vector2 wanderPosition;


        public virtual void Init(Transform player)
        {
            rigidBody = GetComponent<Rigidbody2D>();
            attackCollider = GetComponent<Collider2D>();
            animator = GetComponent<AiAnimatorInterface>();
            viewController = GetComponent<AiViewController>();
            currentTarget = player;
            viewController.Init();
            OnInit();
            viewController.StartFadeIn(2f, Enable);
        }

        void Update()
        {
            if (isDisabled)
                return;

            if (OutOfAgroRange() || !canAttack)
            {
                ProcessWanderingState();
            }
            else
            {
                ProcessFollowingState();
            }
        }


        public virtual Vector2 GetVelocity()
        {
            return Vector2.zero;
        }

        public void SetAttackState(bool attackState)
        {
            canAttack = attackState;
        }

        public virtual void ProcessKnockBack()
        {
        }

        public virtual void Enable()
        {
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
            gameObject.SetActive(true);
            attackCollider.enabled = true;
            isDisabled = false;
        }

        public virtual void Disable()
        {
            isDisabled = true;
            attackCollider.enabled = false;
            rigidBody.bodyType = RigidbodyType2D.Static;
            animator.PlayDeathAnimation(() =>
            {
                if (gameObject != null)
                {
                    gameObject.SetActive(false);
                }

                OnDisabled?.Invoke();
            });
        }


        public virtual void ProcessWanderingState()
        {
        }

        public virtual void ProcessFollowingState()
        {
        }

        protected bool OutOfAgroRange()
        {
            return Vector2.Distance(CurrentTarget.position, transform.position)
                > m_Model.CombatModel.GetMonsterStatData.AgroDistance;
        }

        protected bool InAttackRange()
        {
            return Vector2.Distance(CurrentTarget.position, transform.position) 
                <= m_Model.CombatModel.GetMonsterStatData.AttackRange;
        }

        protected void OnInit()
        {
            OnInitialized?.Invoke();
        }
    }
}