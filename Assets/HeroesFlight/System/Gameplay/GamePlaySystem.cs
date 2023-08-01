using System;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.System.Character;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using StansAssets.Foundation.Async;
using StansAssets.Foundation.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        CameraControllerInterface cameraController;
        NpcSystemInterface npcSystem;
        GameplayState currentState;
        float timeSinceLastStrike;
        float timeTiResetCombo = 3f;
        int characterComboNumber;

        public event Action<bool> OnMinibossSpawned;
        public event Action<float> OnMinibossHealthChange;

        public event Action<int> OnRemainingEnemiesLeft;
        public event Action<DamageModel> OnCharacterDamaged;
        public event Action<DamageModel> OnEnemyDamaged;
        public event Action<int> OnCharacterHealthChanged;
        public event Action<int> OnCharacterComboChanged;
        public event Action<GameplayState> OnGameStateChange;
        int enemiesToKill;
        int wavesAmount;
        Coroutine combotTimerRoutine;

        public void Init(Scene scene = default, Action OnComplete = null)
        {
            cameraController = scene.GetComponentInChildren<CameraControllerInterface>();
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
            wavesAmount = 10;
            enemiesToKill = 50;
            OnRemainingEnemiesLeft?.Invoke(enemiesToKill);
            OnCharacterComboChanged?.Invoke(characterComboNumber);
            combotTimerRoutine = CoroutineUtility.Start(CheckTimeSinceLastStrike());
            SetupCharacter();
            currentState = GameplayState.Ongoing;
            OnGameStateChange?.Invoke(currentState);
            cameraController.SetCameraShakeState(true);
            GameTimer.Start(3, null,
                () =>
                {
                    characterSystem.SetCharacterControllerState(true);
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

        public void ReviveCharacter()
        {
            characterHealthController.Reset();
            GameTimer.Resume();
            currentState = GameplayState.Ongoing;
            OnGameStateChange?.Invoke(currentState);
        }

        void SetupCharacter()
        {
           var characterController = characterSystem.CreateCharacter();
            characterHealthController =
                characterController.CharacterTransform.GetComponent<CharacterHealthController>();
            characterAttackController =
                characterController.CharacterTransform.GetComponent<CharacterAttackController>();
            characterAttackController.SetCallback(GetExistingEnemies);
            characterHealthController.OnDeath += HandleCharacterDeath;
            characterHealthController.OnBeingDamaged += HandleCharacterDamaged;
            characterHealthController.Init();
            characterSystem.SetCharacterControllerState(false);
            cameraController.SetTarget(characterController.CharacterTransform);
            npcSystem.InjectPlayer(characterController.CharacterTransform);
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

        void HandleMinibosshealthChange(DamageModel damageModel)
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

        void HandleCharacterDamaged(DamageModel damageModel)
        {
            if (currentState != GameplayState.Ongoing)
                return;

            OnCharacterDamaged?.Invoke(damageModel);
        }

        void HandleCharacterDeath(IHealthController obj)
        {
            if (currentState != GameplayState.Ongoing)
                return;
            //freezes engine?  
           // GameTimer.Pause();
            CoroutineUtility.WaitForSeconds(2f, () =>
            {
                ChangeState(GameplayState.Lost);       
            });
         
        }

        void HandleEnemyDamaged(DamageModel damageModel)
        {
            UpdateCharacterCombo();
            OnEnemyDamaged?.Invoke(damageModel);
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
            currentState = newState;
            OnGameStateChange?.Invoke(currentState);
        }
    }
}