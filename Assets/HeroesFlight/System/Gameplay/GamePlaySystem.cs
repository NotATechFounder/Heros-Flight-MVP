using System;
using System.Collections.Generic;
using HeroesFlight.System.Character;
using HeroesFlight.System.NPC;
using HeroesFlight.System.UI;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using StansAssets.Foundation.Extensions;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.Gameplay
{
    public class GamePlaySystem : GamePlaySystemInterface
    {
        public GamePlaySystem(IUISystem uiSystem,CharacterSystemInterface characterSystem,NpcSystemInterface npcSystem)
        {
            this.uiSystem = uiSystem;
            this.npcSystem = npcSystem;
            this.characterSystem = characterSystem;
            npcSystem.OnEnemySpawned += HandleEnemySpawned;
           
        }

        List<IHealthController> enemyHealthControllers = new();
        IHealthController characterHealthController;

        CharacterSystemInterface characterSystem;

        NpcSystemInterface npcSystem;

        IUISystem uiSystem;

        int enemiesToKill;

        public void Init(Scene scene = default, Action OnComplete = null)
        {
            enemiesToKill = 50;
            characterHealthController = scene.GetComponent<CharacterHealthController>();
            characterHealthController.OnDeath += HandleCharacterDeath;
            characterHealthController.Init();
            StartGameLoop();
            
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
            var healthController = obj.GetComponent<IHealthController>();
            healthController.OnBeingDamaged += HandleEnemyDamaged;
            healthController.OnDeath += HandleEnemyDeath;
            healthController.Init();
            enemyHealthControllers.Add(healthController);
        }

        void HandleEnemyDeath(IHealthController iHealthController)
        {
            iHealthController.OnBeingDamaged += HandleEnemyDamaged;
            iHealthController.OnDeath += HandleEnemyDeath;
            enemyHealthControllers.Remove(iHealthController);
            enemiesToKill--;
        }

        void HandleCharacterDeath(IHealthController obj)
        {
            throw new NotImplementedException();
        }

        void HandleEnemyDamaged(int obj)
        {
            //show dmg text
        }
    }
}