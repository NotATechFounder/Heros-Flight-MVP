using HeroesFlight.Common;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Data
{
    [CreateAssetMenu(fileName = "AiModel", menuName = "Model/AI", order = 0)]
    public class AiAgentModel : ScriptableObject
    {
        [SerializeField] EnemyType m_EnemyType;
        [SerializeField] float wanderingDistance;
        [SerializeField] CombatModel m_CombatModel;

        public EnemyType EnemyType => m_EnemyType;
        public float WanderingDistance => wanderingDistance;
        public CombatModel CombatModel => m_CombatModel;
    }
}