using HeroesFlightProject.System.NPC.Data;
using HeroesFlightProject.System.NPC.Utilities;
using NodeCanvas.Framework;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public class AiController : MonoBehaviour,AiControllerInterface
    {
        [SerializeField] AiAgentModel m_Model;
        [SerializeField] Blackboard m_BlackBoard;
        [SerializeField] GraphOwner m_GraphOwner;

        void Awake()
        {
            var player = GameObject.FindWithTag("Player");
            Init(player.transform);
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