using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeroesFlightProject.System.NPC.Data
{
    [CreateAssetMenu(fileName = "AiModel", menuName = "Model/AI", order = 0)]
    public class AiAgentModel : ScriptableObject
    {
        [SerializeField] EnemyType enemyType;
        [FormerlySerializedAs("enemySpawmType")][SerializeField] EnemySpawmType m_EnemySpawnType;
        [SerializeField] float wanderingDistance;
        [SerializeField] AiAgentCombatModel m_CombatModel;

        public EnemyType EnemyType => enemyType;
        public EnemySpawmType EnemySpawnType => m_EnemySpawnType;
        public float WanderingDistance => wanderingDistance;
        public AiAgentCombatModel CombatModel => m_CombatModel;
    }
}