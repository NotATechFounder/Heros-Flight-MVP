using System;
using HeroesFlightProject.System.NPC.Data;
using HeroesFlightProject.System.NPC.Enum;
using HeroesFlightProject.System.NPC.Utilities;
using NodeCanvas.Framework;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public class AiController : MonoBehaviour,AiControllerInterface
    {
        [SerializeField] AiAgentModel m_Model;
        [SerializeField] Blackboard m_BlackBoard;
        [SerializeField] GraphOwner m_GraphOwner;
        [SerializeField] float wanderDistance = 10f;
        [SerializeField] bool canFly = false;

        public event Action OnInitialized;
        public AiAgentModel AgentMode => m_Model;
        public Transform CurrentTarget { get; private set; }

        Vector2 wanderPosition;

        void Awake()
        {
            CurrentTarget = GameObject.FindWithTag("Player").transform;
            Init(CurrentTarget);
            Enable();
        }

        public void Init(Transform player)
        {
           
            m_BlackBoard = GetComponent<Blackboard>();
            m_GraphOwner = GetComponent<GraphOwner>();
            m_BlackBoard.SetVariableValue(BlackboardVariableNames.MovementSpeed, m_Model.Speed);
            m_BlackBoard.SetVariableValue(BlackboardVariableNames.AttackRange, m_Model.AttackRange);
            m_BlackBoard.SetVariableValue(BlackboardVariableNames.TimeBetweenAttacks, m_Model.Speed);
            m_BlackBoard.SetVariableValue(BlackboardVariableNames.Target, player);
            m_BlackBoard.SetVariableValue(BlackboardVariableNames.AgroDistance, m_Model.AgroDistance);


        }

        
       
        public void Enable()
        {
            gameObject.SetActive(true);
            m_GraphOwner.StartBehaviour();
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

       
    }
}