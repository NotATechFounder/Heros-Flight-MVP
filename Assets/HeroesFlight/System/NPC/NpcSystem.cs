using System;
using HeroesFlight.System.NPC.Container;
using HeroesFlightProject.System.NPC.Controllers;
using StansAssets.Foundation.Extensions;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.NPC
{
    public class NpcSystem : NpcSystemInterface
    {
       
        NpcContainer container;
        public void Init(Scene scene = default, Action onComplete = null)
        {
            container = scene.GetComponent<NpcContainer>();
            container.Init();
        }

        public void Reset() { }

        public event Action<AiControllerBase> OnEnemySpawned;
      
        public void SpawnRandomEnemies(int amount)
        {
            container.SpawnEnemies(amount,OnEnemySpawned);
        }
    }
}