using HeroesFlight.Common;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeroesFlightProject.System.NPC.Data
{
    [CreateAssetMenu(fileName = "AiModel", menuName = "Model/AI", order = 0)]
    public class AiAgentModel : ScriptableObject
    {
        [SerializeField] EnemyType enemyType;
        [SerializeField] SpawnType enemySpawmType;
        [SerializeField] float wanderingDistance;
        [SerializeField] AiAgentCombatModel m_CombatModel;

        public EnemyType EnemyType => enemyType;
        public SpawnType EnemySpawmType => enemySpawmType;
        public float WanderingDistance => wanderingDistance;
        public AiAgentCombatModel CombatModel => m_CombatModel;
    }
}