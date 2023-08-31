using System;
using HeroesFlight.System.NPC.Container;
using HeroesFlight.System.NPC.Model;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlight.System.NPC
{
    public interface NpcSystemInterface : SystemInterface
    {
        public NpcContainer NpcContainer { get;}

        event Action<AiControllerBase> OnEnemySpawned;
        void SetSpawnModel(Level level);
        void InjectPlayer(Transform player);
    }
}