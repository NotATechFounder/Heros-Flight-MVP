using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Character;
using HeroesFlight.System.Character.Enum;
using HeroesFlight.System.Environment;
using HeroesFlight.System.FileManager.Enum;
using HeroesFlight.System.FileManager.Model;
using HeroesFlight.System.Gameplay.Container;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC;
using HeroesFlight.System.NPC.Model;
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
        public GamePlaySystem(DataSystemInterface dataSystemInterface, CharacterSystemInterface characterSystem,
            NpcSystemInterface npcSystem, EnvironmentSystemInterface environmentSystem)
        {
            this.dataSystemInterface = dataSystemInterface;
            this.npcSystem = npcSystem;
            this.characterSystem = characterSystem;
            this.environmentSystem = environmentSystem;
            npcSystem.OnEnemySpawned += HandleEnemySpawned;
        }

        public event Action OnNextLvlLoadRequest;
        public CountDownTimer GameTimer { get; private set; }
        public AngelEffectManager EffectManager { get; private set; }

        public BoosterManager BoosterManager { get; private set; }

        public BoosterSpawner BoosterSpawner { get; private set; }

        public CurrencySpawner CurrencySpawner { get; private set; }

        public HeroProgression HeroProgression { get; private set;}

        public GodsBenevolence GodsBenevolence { get; private set; }

        public int CurrentLvlIndex => container.CurrentLvlIndex;

        public int MaxLvlIndex => container.MaxLvlIndex;

        public Vector2 GetPlayerSpawnPosition => currentLevelEnvironment.GetSpawnpoint(HeroesFlightProject.System.NPC.Enum.SpawnType.Player).GetSpawnPosition();

        public event Action<float> OnUltimateChargesChange;
        public event Action<bool> OnMinibossSpawned;
        public event Action<float> OnMinibossHealthChange;

        public event Action<int> OnRemainingEnemiesLeft;
        public event Action<DamageModel> OnCharacterDamaged;
        public event Action<float, Transform> OnCharacterHeal;
        public event Action<DamageModel> OnEnemyDamaged;
        public event Action<int> OnCharacterHealthChanged;
        public event Action<int> OnCharacterComboChanged;
        public event Action<GameState> OnGameStateChange;
        public event Action<BoosterSO, float, Transform> OnBoosterActivated;
        public event Action<int> OnCoinsCollected;
        public event Action<BoosterContainer> OnBoosterContainerCreated;
        public event Action<int> OnCountDownTimerUpdate;
        public event Action<float> OnGameTimerUpdate;
        public event Action OnEnterMiniBossLvl;

        DataSystemInterface dataSystemInterface;
        List<IHealthController> GetExistingEnemies() => activeEnemyHealthControllers;
        List<IHealthController> activeEnemyHealthControllers = new();
        IHealthController miniBoss;
        IHealthController characterHealthController;
        BaseCharacterAttackController characterAttackController;
        CharacterStatController characterStatController;
        CharacterVFXController characterVFXController;
        CharacterAbilityInterface characterAbility;
        CharacterSystemInterface characterSystem;
        CameraControllerInterface cameraController;
        EnvironmentSystemInterface environmentSystem;
        NpcSystemInterface npcSystem;
        GameplayContainer container;
        GameState currentState;
        float timeSinceLastStrike;
        float timeToResetCombo = 3f;
        int characterComboNumber;
        int enemiesToKill;
        int wavesAmount;
        Coroutine combotTimerRoutine;
        int collectedGold;
        float collectedXp;
        float collectedHeroProgressionSp;
        float countDownDelay;
        LevelEnvironment currentLevelEnvironment;
        Level currentLevel;

        public void Init(Scene scene = default, Action OnComplete = null)
        {
            cameraController = scene.GetComponentInChildren<CameraControllerInterface>();
            EffectManager = scene.GetComponentInChildren<AngelEffectManager>();
            container = scene.GetComponentInChildren<GameplayContainer>();
            BoosterManager = scene.GetComponentInChildren<BoosterManager>();
            BoosterSpawner = scene.GetComponentInChildren<BoosterSpawner>();
            BoosterManager.OnBoosterActivated += HandleBoosterActivated;
            BoosterManager.OnBoosterContainerCreated += HandleBoosterWithDurationActivated;

            CurrencySpawner = scene.GetComponentInChildren<CurrencySpawner>();
            CurrencySpawner.Initialize(this);

            HeroProgression = scene.GetComponentInChildren<HeroProgression>();

            GodsBenevolence = scene.GetComponentInChildren<GodsBenevolence>();

            container.Init();
            container.OnPlayerEnteredPortal += HandlePlayerTriggerPortal;
            container.SetStartingIndex(0);

            npcSystem.NpcContainer.SetMobDifficultyHolder(container.CurrentModel.MobDifficulty);

            OnUltimateChargesChange?.Invoke(0);

            GameTimer = new CountDownTimer(container);

            OnComplete?.Invoke();
        }

        public void Reset()
        {
            ResetLogic();
            ResetPlayerSubscriptions();
            container.SetStartingIndex(0);
        }


        void ResetPlayerSubscriptions()
        {
           
            characterHealthController.OnDeath -= HandleCharacterDeath;
            characterHealthController.OnBeingDamaged -= HandleCharacterDamaged;
            characterHealthController.OnHeal -= HandleCharacterHeal;
            characterHealthController.OnDodged -= HandleCharacterDodged;
            characterAttackController = null;
            characterHealthController = null;
            characterAbility = null;
        }

        public void ResetLogic()
        {
            activeEnemyHealthControllers.Clear();
            BoosterSpawner.ClearAllBoosters(); 
            // EffectManager.ResetAngelEffects();
            enemiesToKill = 0;
            GameTimer.Stop();
            ChangeState(GameState.Ended);
            OnMinibossSpawned?.Invoke(false);
            if (combotTimerRoutine != null)
                CoroutineUtility.Stop(combotTimerRoutine);
        }

        public void EnablePortal()
        {
            container.EnablePortal(currentLevelEnvironment.GetSpawnpoint(HeroesFlightProject.System.NPC.Enum.SpawnType.Portal).GetSpawnPosition());
        }

        public void UseCharacterSpecial()
        {          
            cameraController.SetCameraState(GameCameraType.Skill);
            characterHealthController.SetInvulnerableState(true);
            characterSystem.SetCharacterControllerState(false);
            characterAttackController.ToggleControllerState(false);
            environmentSystem.ParticleManager.Spawn(characterSystem.CurrentCharacter.CharacterSO.VFXData.UltVfx,
                characterSystem.CurrentCharacter.CharacterTransform.position,Quaternion.Euler(new Vector3(-90,0,0)));
            characterAbility.UseAbility(characterStatController.CurrentPhysicalDamage * characterSystem.CurrentCharacter.CharacterSO.UltimateData.DamageMultiplier,
                null, () =>
            {
                cameraController.SetCameraState(GameCameraType.Character);
                characterSystem.SetCharacterControllerState(true);
                characterAttackController.ToggleControllerState(true);
                characterHealthController.SetInvulnerableState(false);
            });
        }

        public void CreateCharacter()
        {
            SetupCharacter();
        }

        public void ReviveCharacter()
        {
            characterHealthController.Revive();
            GameTimer.Resume();
            ChangeState(GameState.Ongoing);
        }

        bool CheckLevel(out Level currentLevel)
        {
            currentLevel = container.GetLevel();
            if (currentLevel == null)
            {
                ChangeState(GameState.Won);
                return false;
            }
            return true;
        }

        void CreateLvL(Level currentLvl)
        {
            npcSystem.SetSpawnModel(currentLvl, CurrentLvlIndex);
        }

        void SetupCharacter()
        {
            var characterController = characterSystem.CreateCharacter(GetPlayerSpawnPosition);
            characterHealthController =
                characterController.CharacterTransform.GetComponent<CharacterHealthController>();
            characterAttackController =
                characterController.CharacterTransform.GetComponent<BaseCharacterAttackController>();

            characterVFXController = characterController.CharacterTransform.GetComponent<CharacterVFXController>();
            characterVFXController.Initialize(cameraController.CameraShaker);

            characterAbility =characterController.CharacterTransform.GetComponent<AbilityBaseCharacter>();
            characterAbility.Init(characterController.CharacterSO.AnimationData.UltimateAnimations,
                characterController.CharacterSO.UltimateData.Charges);
            characterAttackController.Init();
          
            characterHealthController.OnDeath += HandleCharacterDeath;
            characterHealthController.OnBeingDamaged += HandleCharacterDamaged;
            characterHealthController.OnHeal += HandleCharacterHeal;
            characterHealthController.OnDodged += HandleCharacterDodged;
            characterHealthController.Init();
            characterSystem.SetCharacterControllerState(false);
            cameraController.SetTarget(characterController.CharacterTransform.GetComponentInChildren<CameraTargetController>().transform);
            npcSystem.InjectPlayer(characterController.CharacterTransform);

            characterStatController = characterController.CharacterTransform.GetComponent<CharacterStatController>();
            EffectManager.Initialize(characterStatController);
            BoosterManager.Initialize(characterStatController);
            CurrencySpawner.SetPlayer(characterController.CharacterTransform);
            HeroProgression.Initialise(characterStatController);
            GodsBenevolence.Initialize(characterStatController);
        }

        void HandleMinibossHealthChange(DamageModel damageModel)
        {
            OnMinibossHealthChange?.Invoke(miniBoss.CurrentHealthProportion);
            if (miniBoss.CurrentHealthProportion <= 0)
            {
                miniBoss.OnBeingDamaged -= HandleMinibossHealthChange;
            }
        }

        void HandleEnemySpawned(AiControllerBase obj)
        {
            var healthController = obj.GetComponent<AiHealthController>();

            if (obj.EnemyType == HeroesFlightProject.System.NPC.Enum.EnemyType.MiniBoss)
            {
                miniBoss = obj.GetComponent<IHealthController>();
                miniBoss.OnBeingDamaged += HandleEnemyDamaged;
                miniBoss.OnBeingDamaged += HandleMinibossHealthChange;
                miniBoss.OnDeath += HandleEnemyDeath;
                miniBoss.Init();
                activeEnemyHealthControllers.Add(miniBoss);
                OnMinibossSpawned?.Invoke(true);
            }
            else
            {
      
                obj.OnDisabled += HandleEnemyDisabled;
                healthController.OnBeingDamaged += HandleEnemyDamaged;
                healthController.OnDeath += HandleEnemyDeath;
                healthController.Init();
                activeEnemyHealthControllers.Add(healthController);
            }
        }

        void HandleEnemyDisabled(AiControllerInterface obj)
        {
            obj.OnDisabled -= HandleEnemyDisabled;
            var baseComponent = obj as AiControllerBase;
            environmentSystem.ParticleManager.Spawn("Enemy_Death",baseComponent.transform.position);
        }

        void HandleEnemyDeath(IHealthController iHealthController)
        {
            if (currentState != GameState.Ongoing)
                return;

            iHealthController.OnBeingDamaged -= HandleEnemyDamaged;
            iHealthController.OnDeath -= HandleEnemyDeath;
            activeEnemyHealthControllers.Remove(iHealthController);
            enemiesToKill--;
            environmentSystem.ParticleManager.Spawn("Loot_Spawn", iHealthController.HealthTransform.position,
                Quaternion.Euler(new Vector3(-90,0,0)));
            BoosterSpawner.SpawnBoostLoot(container.MobDrop, iHealthController.HealthTransform.position);
            CurrencySpawner.SpawnAtPosition(CurrencyKeys.Gold, 10, iHealthController.HealthTransform.position);
            CurrencySpawner.SpawnAtPosition(CurrencyKeys.Experience, 10, iHealthController.HealthTransform.position);
            collectedHeroProgressionSp += container.HeroProgressionExpEarnedPerKill;

            OnRemainingEnemiesLeft?.Invoke(enemiesToKill);

            if (enemiesToKill <= 0)
            {
                GameTimer.Stop();
                Time.timeScale = 0.2f;
                CoroutineUtility.WaitForSecondsRealtime(2f, () =>
                {
                    Time.timeScale =1f;
                });
               
                if (container.FinishedLoop)
                {
                    characterAttackController.ToggleControllerState(false);
                    characterSystem.SetCharacterControllerState(false);
                    //Temp rewarding player with unlock here
                    dataSystemInterface.RewardHandler.GrantReward(new HeroRewardModel(RewardType.Hero,CharacterType.Lancer));
                  
                    CoroutineUtility.WaitForSeconds(1f, () =>
                    {
                        ChangeState(GameState.Won);
                    });

                    return;
                }
                
                ChangeState(GameState.WaitingPortal);
            }
        }

        void HandleCharacterDamaged(DamageModel damageModel)
        {
            if (currentState != GameState.Ongoing)
                return;

            OnCharacterDamaged?.Invoke(damageModel);
        }

        void HandleCharacterHeal(float arg1, Transform transform)
        {
            OnCharacterHeal?.Invoke(arg1, transform);
        }

        private void HandleCharacterDodged()
        {
            characterVFXController.TriggerMissEffect();
        }

        void HandleCharacterDeath(IHealthController obj)
        {
            Debug.LogError($"character died and game state is {currentState}");
            if (currentState != GameState.Ongoing)
                return;

            //freezes engine?  
            // GameTimer.Pause();
            CoroutineUtility.WaitForSeconds(1f, () =>
            {
                ChangeState(GameState.Died);
            });
        }

        void HandleEnemyDamaged(DamageModel damageModel)
        {
            UpdateCharacterCombo();
            var vfxReference = string.Empty;
            var isCritical = damageModel.DamageType == DamageType.Critical;
            switch (damageModel.AttackType)
            {
                case AttackType.Regular:
                    vfxReference = isCritical
                        ? characterSystem.CurrentCharacter.CharacterSO.VFXData.AutoattackCrit
                        : characterSystem.CurrentCharacter.CharacterSO.VFXData.AutoattackNormal;
                    characterAbility.UpdateAbilityCharges(5);
                    OnUltimateChargesChange?.Invoke(characterAbility.CurrentCharge);
                    break;
                case AttackType.Ultimate:
                    vfxReference = isCritical
                        ? characterSystem.CurrentCharacter.CharacterSO.VFXData.UltCrit
                        : characterSystem.CurrentCharacter.CharacterSO.VFXData.UltNormal;
                    break;
               
            }
          
            if (damageModel.DamageType == DamageType.Critical)
            {
                cameraController.CameraShaker.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Explosion,0.1f,0.20f);
            }
            else
            {
                cameraController.CameraShaker.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Bump,0.1f,0.1f);
            }
            
            if(!vfxReference.Equals(string.Empty))
                environmentSystem.ParticleManager.Spawn(vfxReference, damageModel.Target.position);
            
           
            OnEnemyDamaged?.Invoke(damageModel);
        }

        void UpdateCharacterCombo()
        {
            timeSinceLastStrike = timeToResetCombo;
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

                    if (characterComboNumber != 0)
                    {
                        characterComboNumber = 0;
                        OnCharacterComboChanged?.Invoke(characterComboNumber);
                    }
                }

                yield return null;
            }
        }

        void ChangeState(GameState newState)
        {
            if (currentState == newState)
                return;
            currentState = newState;
            OnGameStateChange?.Invoke(currentState);
        }

        void HandlePlayerTriggerPortal()
        {
            OnNextLvlLoadRequest?.Invoke();
        }

        public void StartGameLoop()
        {
            //TODO  modify here for  angels gambit

            switch (currentLevel.LevelType)
            {
                case LevelType.Combat:

                    enemiesToKill = currentLevel.MiniHasBoss ? currentLevel.TotalMobsToSpawn + 1 : currentLevel.TotalMobsToSpawn;
                    OnRemainingEnemiesLeft?.Invoke(enemiesToKill);
                    OnCharacterComboChanged?.Invoke(characterComboNumber);
                    combotTimerRoutine = CoroutineUtility.Start(CheckTimeSinceLastStrike());

                    ChangeState(GameState.Ongoing);
                    if (currentLevel.MiniHasBoss)
                    {
                        cameraController.CameraShaker.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Rumble, 3f);
                        OnEnterMiniBossLvl?.Invoke();
                        countDownDelay = 2;
                    }
                    else
                    {
                        countDownDelay = .5f;
                    }

                    characterSystem.SetCharacterControllerState(true);

                    CoroutineUtility.WaitForSeconds(countDownDelay, () =>
                    {
                        GameTimer.Start(5, null,
                            () =>
                            {
                                CreateLvL(currentLevel);
                                GameTimer.Start(120, OnGameTimerUpdate,
                                    () =>
                                    {
                                        if (currentState != GameState.Ongoing)
                                            return;

                                        ChangeState(GameState.TimeEnded);
                                    });
                            }, OnCountDownTimerUpdate);
                    });

                    break;
                case LevelType.Intermission:
                    characterSystem.SetCharacterControllerState(true);
                    break;
            }

        }

        private void TriggerAngelsGambit()
        {
            EffectManager.TriggerAngelsGambit();
        }

        public Level PreloadLvl()
        {
            if (!CheckLevel(out Level currentLvl))
            {
                Debug.LogError("Current lvl loop model has 0 lvls");
                return null;
            }
            currentLevel = currentLvl;
            SetUpLevelEnvironment();
            return currentLvl;
        }

        public void SetUpLevelEnvironment()
        {
            if (currentLevelEnvironment != null) GameObject.DestroyImmediate(currentLevelEnvironment.gameObject);
            currentLevelEnvironment = GameObject.Instantiate(currentLevel.LevelPrefab).GetComponent<LevelEnvironment>();
            cameraController.SetConfiner(currentLevelEnvironment.BoundsCollider);
            npcSystem.NpcContainer.SetSpawnPoints(currentLevelEnvironment.SpawnPointsCache);

            switch (currentLevel.LevelType)
            {
                case LevelType.Combat:
                    container.DisablePortal();
                    break;
                case LevelType.Intermission:
                    currentLevelEnvironment.InteractiveNPC.OnInteract = TriggerAngelsGambit;
                    container.EnablePortal(currentLevelEnvironment.GetSpawnpoint(HeroesFlightProject.System.NPC.Enum.SpawnType.Portal).GetSpawnPosition());
                    break;
            }
        }

        private void HandleBoosterActivated(BoosterSO sO, float arg2, Transform transform)
        {
            OnBoosterActivated?.Invoke(sO, arg2, transform);
        }

        public void AddGold(int amount)
        {
            collectedGold += amount;
            OnCoinsCollected?.Invoke(collectedGold);
        }

        public void AddExperience(int amount)
        {
            collectedXp += amount;
        }

        public void StoreRunReward()
        {     
            dataSystemInterface.AddCurency(CurrencyKeys.Gold, collectedGold);
            dataSystemInterface.AddCurency(CurrencyKeys.Experience, collectedXp);
            collectedGold = 0;
            collectedXp = 0;
        }

        private void HandleBoosterWithDurationActivated(BoosterContainer container)
        {
            OnBoosterContainerCreated?.Invoke(container);
        }

        public void HandleHeroProgression()
        {
            GodsBenevolence.DeactivateGodsBenevolence();

            HeroProgression.AddExp(collectedHeroProgressionSp);
            collectedHeroProgressionSp = 0;
        }

        public void HandleSingleLevelUp()
        {
            characterVFXController.TriggerLevelUpEffect();
        }

        public void HeroProgressionCompleted()
        {
            characterVFXController.TriggerLevelUpAfterEffect();
        }
    }
}