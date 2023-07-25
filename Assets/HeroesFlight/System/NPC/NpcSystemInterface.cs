using System;
using HeroesFlightProject.System.NPC.Controllers;

namespace HeroesFlight.System.NPC
{
    public interface NpcSystemInterface : ISystemInterface
    {
        event Action<AiControllerBase> OnEnemySpawned;
        void SpawnRandomEnemies(int amount);
    }
}