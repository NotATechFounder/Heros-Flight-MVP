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
        [SerializeField] EnemySpawmType enemySpawmType;
        [SerializeField] float wanderingDistance;
        [SerializeField] CombatModel m_CombatModel;

        public EnemyType EnemyType => enemyType;
        public EnemySpawmType EnemySpawmType => enemySpawmType;
        public float WanderingDistance => wanderingDistance;
        public CombatModel CombatModel => m_CombatModel;
    }
}