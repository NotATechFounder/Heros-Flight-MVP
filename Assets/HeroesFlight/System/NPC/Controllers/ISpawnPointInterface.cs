using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers
{
    public interface ISpawnPointInterface
    {
        EnemySpawmType TargetEnemySpawmType { get; }
        Vector2 GetSpawnPosition();
    }
}