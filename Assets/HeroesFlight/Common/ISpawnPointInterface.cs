using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers
{
    public interface ISpawnPointInterface
    {
        SpawnType SpawnType { get; }
        Vector2 GetSpawnPosition();
    }
}