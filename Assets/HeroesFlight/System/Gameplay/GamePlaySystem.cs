using System;
using System.Collections.Generic;
using HeroesFlight.System.Character;
using HeroesFlight.System.NPC;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using StansAssets.Foundation.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.Gameplay
{
    public class GamePlaySystem : GamePlaySystemInterface
    {
        public GamePlaySystem(CharacterSystemInterface characterSystem,NpcSystemInterface npcSystem)
        {
            this.npcSystem = npcSystem;
            this.characterSystem = characterSystem;
            npcSystem.OnEnemySpawned += HandleEnemySpawned;
           
        }

        List<IHealthController> enemyHealthControllers = new();
        IHealthController characterHealthController;

        CharacterSystemInterface characterSystem;

        NpcSystemInterface npcSystem;

      

        public event Action OnPlayerDeath;
        public event Action OnPlayerWin;
        public event Action<Transform, int> OnEnemyDamaged;
        public event Action<int> OnCharacterHealthChanged;
        int enemiesToKill;


        public void Init(Scene scene = default, Action OnComplete = null)
        {
            enemiesToKill = 20;
            characterHealthController = scene.GetComponent<CharacterHealthController>();
            characterHealthController.OnDeath += HandleCharacterDeath;
            characterHealthController.OnBeingDamaged += HandleCharacterDamaged;
            characterHealthController.Init();
        }

        public void Reset()
        {
            enemiesToKill = 0;
        }

        public void StartGameLoop()
        {
            npcSystem.SpawnRandomEnemies(enemiesToKill);
        }

        void HandleEnemySpawned(AiControllerBase obj)
        {
            var healthController = obj.GetComponent<AiHealthController>();
            healthController.OnBeingDamaged += HandleEnemyDamaged;
            healthController.OnDeath += HandleEnemyDeath;
            healthController.Init();
            enemyHealthControllers.Add(healthController);
        }

        void HandleEnemyDeath(IHealthController iHealthController)
        {
            iHealthController.OnDeath -= HandleEnemyDeath;
            enemyHealthControllers.Remove(iHealthController);
            enemiesToKill--;
            if (enemiesToKill <= 0)
            {
                OnPlayerWin?.Invoke();
            }
        }

        void HandleCharacterDamaged(Transform arg1, int arg2) { }

        void HandleCharacterDeath(IHealthController obj)
        {
            OnPlayerDeath?.Invoke();
        }

        void HandleEnemyDamaged(Transform transform, int i)
        {
           OnEnemyDamaged?.Invoke(transform,i);
        }
    }
}