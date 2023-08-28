using System;
using HeroesFlight.System.NPC.Container;
using HeroesFlight.System.NPC.Model;
using HeroesFlightProject.System.NPC.Controllers;
using StansAssets.Foundation.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.NPC
{
    public class NpcSystem : NpcSystemInterface
    {
       
        NpcContainer container;
        public void Init(Scene scene = default, Action onComplete = null)
        {
            container = scene.GetComponentInChildren<NpcContainer>();
            container.Init();
        }

        public void Reset()
        {
            container.Reset();
        }

        public event Action<AiControllerBase> OnEnemySpawned;
      
        public void SetSpawnModel(Level level)
        {
            container.SpawnEnemies(level,OnEnemySpawned);
        }

        public AiControllerBase SpawnMiniBoss(Level level, Action onComplete = null)
        {
             return container.SpawnMiniBoss(level);
        }

        public void InjectPlayer(Transform player)
        {
            container.InjectPlayer(player);
        }
    }
}