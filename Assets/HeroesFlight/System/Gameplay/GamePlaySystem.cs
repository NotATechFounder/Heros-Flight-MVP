using System;
using System.Collections.Generic;
using HeroesFlight.System.Character;
using HeroesFlight.System.Gameplay.Enum;
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
        GameplayState currentState;


        public event Action<int> OnRemainingEnemiesLeft;
        public event Action<Transform, int> OnCharacterDamaged;
        public event Action<Transform, int> OnEnemyDamaged;
        public event Action<int> OnCharacterHealthChanged;
        public event Action<GameplayState> OnGameStateChange;
        int enemiesToKill;
        int wavesAmount;


        public void Init(Scene scene = default, Action OnComplete = null)
        {
            wavesAmount = 3;
            enemiesToKill = 6;
            OnRemainingEnemiesLeft?.Invoke(enemiesToKill);
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
            currentState = GameplayState.Ongoing;
            npcSystem.SpawnRandomEnemies(enemiesToKill,wavesAmount);
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
            if (currentState != GameplayState.Ongoing)
                return;
            
            iHealthController.OnDeath -= HandleEnemyDeath;
            enemyHealthControllers.Remove(iHealthController);
            enemiesToKill--;
            OnRemainingEnemiesLeft?.Invoke(enemiesToKill);
            if (enemiesToKill <= 0)
            {
                currentState = GameplayState.Won;
                OnGameStateChange?.Invoke(currentState);
            }

            
        }

        void HandleCharacterDamaged(Transform characterTransform, int damageReceived)
        {
            if (currentState != GameplayState.Ongoing)
                return;
            
            OnCharacterDamaged?.Invoke(characterTransform,damageReceived);
        }

        void HandleCharacterDeath(IHealthController obj)
        {
            if (currentState != GameplayState.Ongoing)
                return;
            
            currentState = GameplayState.Lost;
            OnGameStateChange?.Invoke(currentState);
        }

        void HandleEnemyDamaged(Transform transform, int i)
        {
           OnEnemyDamaged?.Invoke(transform,i);
        }
    }
}