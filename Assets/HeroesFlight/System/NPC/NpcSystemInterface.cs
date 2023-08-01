using System;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlight.System.NPC
{
    public interface NpcSystemInterface : ISystemInterface
    {
        event Action<AiControllerBase> OnEnemySpawned;
        void SpawnRandomEnemies(int enemiesToKill, int waves);
        AiControllerBase SpawnMiniBoss(Action onComplete=null);
        void InjectPlayer(Transform player);
    }
}