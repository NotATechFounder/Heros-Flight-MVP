using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeroesFlight.System.NPC.Controllers
{
    public class SpawnPoint : MonoBehaviour,ISpawnPointInterface
    {
        [FormerlySerializedAs("enemyType")][SerializeField] EnemySpawmType m_EnemySpawmType;
        public EnemySpawmType TargetEnemySpawmType => m_EnemySpawmType;

        public Vector2 GetSpawnPosition()
        {
            return transform.position;
        }
    }
}