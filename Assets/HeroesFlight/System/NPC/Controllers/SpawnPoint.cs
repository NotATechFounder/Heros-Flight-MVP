
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeroesFlight.System.NPC.Controllers
{
    public class SpawnPoint : MonoBehaviour, ISpawnPointInterface
    {
        [SerializeField] SpawnType spawnType;

        [Header("To Hide")]
        [SerializeField] bool isOccupied;

        public SpawnType SpawnType => spawnType;

        public Vector2 GetSpawnPosition()
        {
            return transform.position;
        }

        void OnDrawGizmos()
        {
#if UNITY_EDITOR
            Color gizmosColor = Color.green;

            switch (spawnType)
            {
                case SpawnType.GroundMob:
                    gizmosColor = Color.red;
                    break;
                case SpawnType.FlyingMob:
                    gizmosColor = Color.yellow;
                    break;
                case SpawnType.Crystal: 
                    gizmosColor = Color.white;
                    break;
                case SpawnType.Player:
                    gizmosColor = Color.blue;
                    break;
                case SpawnType.Portal:
                    gizmosColor = Color.cyan;
                    break;
                default: break;
            }

            Gizmos.color=gizmosColor;
            Gizmos.DrawSphere(transform.position,0.2f);
#endif
        }
    }
}