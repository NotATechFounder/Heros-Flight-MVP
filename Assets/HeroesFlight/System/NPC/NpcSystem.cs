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

        public NpcContainer NpcContainer => container;

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
      
        public void SetSpawnModel(Level level, int levelIndex)
        {
            container.SpawnEnemies(level, levelIndex,OnEnemySpawned);
        }

        public void InjectPlayer(Transform player)
        {
            container.InjectPlayer(player);
        }
    }
}