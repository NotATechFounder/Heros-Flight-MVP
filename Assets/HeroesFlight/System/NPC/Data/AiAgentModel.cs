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
        [Header("BehaviourData")]
        [SerializeField] float wanderingDistance;
        [SerializeField] float agroDistance;
        [SerializeField] float agroDuration;
        [SerializeField] bool attacksInteruptable;
        [SerializeField] bool useKnockback;
        [SerializeField] float knockBackForce;
        [SerializeField] float knockBackDuration = 0.1f;

        [Header("StatsData")]
        [SerializeField] MonsterStatData aiData;
     
        public EnemyType EnemyType => enemyType;
        public SpawnType EnemySpawmType => enemySpawmType;
        public float WanderingDistance => wanderingDistance;
        public float AgroDistance => agroDistance;
        public float AgroDuration => agroDuration;
        public float KnockBackForce => knockBackForce;
        public float KnockBackDuration => knockBackDuration;
        public bool AttacksInteruptable => attacksInteruptable;
        public bool UseKnockBack => useKnockback;
        public MonsterStatData AiData => aiData;
     
    }
}