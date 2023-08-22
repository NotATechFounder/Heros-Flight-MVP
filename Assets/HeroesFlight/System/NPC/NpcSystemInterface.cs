using System;
using HeroesFlight.System.NPC.Model;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlight.System.NPC
{
    public interface NpcSystemInterface : SystemInterface
    {
        event Action<AiControllerBase> OnEnemySpawned;
        void SpawnRandomEnemies(SpawnModel model);
        AiControllerBase SpawnMiniBoss(SpawnModel currentLvlModel, Action onComplete = null);
        void InjectPlayer(Transform player);
    }
}