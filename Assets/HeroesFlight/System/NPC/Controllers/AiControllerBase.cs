using System;
using HeroesFlightProject.System.NPC.Data;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public abstract class AiControllerBase : MonoBehaviour, AiControllerInterface
    {
        [SerializeField] protected AiAgentModel m_Model;
        [SerializeField] protected float wanderDistance = 10f;


        public event Action OnInitialized;
        public event Action OnDisabled;
        public AiAgentModel AgentModel => m_Model;
        public Transform CurrentTarget => currentTarget;

        protected Transform currentTarget;
        bool canAttack;
        Vector2 wanderPosition;
       


        protected virtual void Awake()
        {
            Init(GameObject.FindWithTag("Player").transform);
            Enable();
        }

        public virtual void Init(Transform player)
        {
           
            currentTarget = player;
            OnInitialized?.Invoke();

        }

        void Update()
        {
            if (OutOfAgroRange() || !canAttack)
            {
               ProcessWanderingState();
            }
            else
            {
                ProcessFollowingState();
            }
        }

        public void SetAttackState(bool attackState)
        {
            canAttack = attackState;
        }

        public virtual void Enable()
        {
            gameObject.SetActive(true);
        }

        public virtual void Disable()
        {
            gameObject.SetActive(false);
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
                > m_Model.CombatModel.AgroDistance;
        }

        protected bool InAttackRange()
        {
            return Vector2.Distance(CurrentTarget.position, transform.position) 
                <= m_Model.CombatModel.AttackRange;
        }

      
    }
}