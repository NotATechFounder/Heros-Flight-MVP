using System;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.System.Character;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.NPC;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using StansAssets.Foundation.Async;
using StansAssets.Foundation.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using CameraControllerInterface = HeroesFlightProject.System.Gameplay.Controllers.CameraControllerInterface;

namespace HeroesFlight.System.Gameplay
{
    public class GamePlaySystem : GamePlaySystemInterface
    {
        public GamePlaySystem(CharacterSystemInterface characterSystem, NpcSystemInterface npcSystem)
        {
            this.npcSystem = npcSystem;
            this.characterSystem = characterSystem;
            npcSystem.OnEnemySpawned += HandleEnemySpawned;
            GameTimer = new CountDownTimer();
        }

        public CountDownTimer GameTimer { get; private set; }
        List<IHealthController> GetExistingEnemies() => enemyHealthControllers;
        List<IHealthController> enemyHealthControllers = new();
        IHealthController miniBoss;
        IHealthController characterHealthController;
        CharacterAttackController characterAttackController;
        CharacterSystemInterface characterSystem;
        CharacterControllerInterface characterController;
        CameraControllerInterface cameraController;
        NpcSystemInterface npcSystem;
        GameplayState currentState;
        float timeSinceLastStrike;
        float timeTiResetCombo = 3f;
        int characterComboNumber;

        public event Action<bool> OnMinibossSpawned;
        public event Action<float> OnMinibossHealthChange;
       
        public event Action<int> OnRemainingEnemiesLeft;
        public event Action<Transform, int> OnCharacterDamaged;
        public event Action<Transform, int> OnEnemyDamaged;
        public event Action<int> OnCharacterHealthChanged;
        public event Action<int> OnCharacterComboChanged;
        public event Action<GameplayState> OnGameStateChange;
        int enemiesToKill;
        int wavesAmount;
        Coroutine combotTimerRoutine;

        public void Init(Scene scene = default, Action OnComplete = null)
        {
            wavesAmount = 10;
            enemiesToKill = 50;
            OnRemainingEnemiesLeft?.Invoke(enemiesToKill);
            characterHealthController = scene.GetComponent<CharacterHealthController>();
            characterAttackController = scene.GetComponent<CharacterAttackController>();
            characterController = scene.GetComponent<CharacterSimpleController>();
            cameraController = scene.GetComponentInChildren<CameraControllerInterface>();
            characterController.SetActionState(true);
            characterAttackController.SetCallback(GetExistingEnemies);
            characterHealthController.OnDeath += HandleCharacterDeath;
            characterHealthController.OnBeingDamaged += HandleCharacterDamaged;
            characterHealthController.Init();
            OnCharacterComboChanged?.Invoke(characterComboNumber);
            OnGameStateChange?.Invoke(currentState);
            combotTimerRoutine = CoroutineUtility.Start(CheckTimeSinceLastStrike());
        }

        public void Reset()
        {
            enemyHealthControllers.Clear();
            enemiesToKill = 0;
            GameTimer.Stop();
            currentState = GameplayState.Ended;
            OnGameStateChange?.Invoke(currentState);
            OnMinibossSpawned?.Invoke(false);
            CoroutineUtility.Stop(combotTimerRoutine);
        }

        public void StartGameLoop()
        {
            currentState = GameplayState.Ongoing;
            OnGameStateChange?.Invoke(currentState);
            cameraController.SetCameraShakeState(true);
            GameTimer.Start(3, null,
                () =>
                {
                    characterController.SetActionState(false);
                    CreateMiniboss();
                    npcSystem.SpawnRandomEnemies(enemiesToKill, wavesAmount);
                    GameTimer.Start(180, null,
                        () =>
                        {
                            if (currentState != GameplayState.Ongoing)
                                return;

                            ChangeState(GameplayState.Lost);
                        }, characterAttackController);
                }, characterAttackController);
        }

        void CreateMiniboss()
        {
            var miniboss = npcSystem.SpawnMiniBoss();
            miniBoss = miniboss.GetComponent<IHealthController>();
            miniBoss.OnBeingDamaged += HandleEnemyDamaged;
            miniBoss.OnBeingDamaged += HandleMinibosshealthChange;
            miniBoss.OnDeath += HandleEnemyDeath;
            miniBoss.Init();
            enemyHealthControllers.Add(miniBoss);
            OnMinibossSpawned?.Invoke(true);
            cameraController.SetCameraShakeState(false);
        }

        void HandleMinibosshealthChange(Transform transform, int i)
        {
           OnMinibossHealthChange?.Invoke(miniBoss.CurrentHealthProportion);
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
                ChangeState(GameplayState.Won);
            }
        }

        void HandleCharacterDamaged(Transform characterTransform, int damageReceived)
        {
            if (currentState != GameplayState.Ongoing)
                return;

            OnCharacterDamaged?.Invoke(characterTransform, damageReceived);
        }

        void HandleCharacterDeath(IHealthController obj)
        {
            if (currentState != GameplayState.Ongoing)
                return;

            ChangeState(GameplayState.Lost);
        }

        void HandleEnemyDamaged(Transform transform, int i)
        {
            UpdateCharacterCombo();
            OnEnemyDamaged?.Invoke(transform, i);
        }

        void UpdateCharacterCombo()
        {
            timeSinceLastStrike = timeTiResetCombo;
            characterComboNumber++;
            OnCharacterComboChanged?.Invoke(characterComboNumber);
        }

        IEnumerator CheckTimeSinceLastStrike()
        {
            while (true)
            {
                timeSinceLastStrike -= Time.deltaTime;
                if (timeSinceLastStrike <= 0)
                {
                    characterComboNumber = 0;
                    OnCharacterComboChanged?.Invoke(characterComboNumber);
                }

                yield return null;
            }
        }

        void ChangeState(GameplayState newState)
        {
            if (currentState == newState)
                return;
            currentState =newState;
            OnGameStateChange?.Invoke(currentState);
        }
    }
}