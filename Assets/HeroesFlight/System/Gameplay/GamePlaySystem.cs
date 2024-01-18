using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Character;
using HeroesFlight.System.Combat;
using HeroesFlight.System.Combat.Effects.Effects;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Combat.Model;
using HeroesFlight.System.Environment;
using HeroesFlight.System.Gameplay.Container;
using HeroesFlight.System.Gameplay.Controllers.Sound;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.Inventory;
using HeroesFlight.System.NPC;
using HeroesFlight.System.NPC.Controllers.Ability;
using HeroesFlight.System.NPC.Controllers.Control;
using HeroesFlight.System.NPC.Model;
using HeroesFlight.System.ShrineSystem;
using HeroesFlight.System.Stats;
using HeroesFlight.System.Stats.Handlers;
using HeroesFlight.System.Stats.Traits.Enum;
using HeroesFlight.System.UI;
using HeroesFlight.System.UI.Enum;
using HeroesFlight.System.UI.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Enum;
using Pelumi.ObjectPool;
using Plugins.Audio_System;
using StansAssets.Foundation.Async;
using StansAssets.Foundation.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.Gameplay
{
    public class GamePlaySystem : GamePlaySystemInterface
    {
        public event Action OnLevelComplected;
        public event Action OnLevelFailed;
        public event Action OnWorldCompleted;

        public GamePlaySystem(DataSystemInterface dataSystem, CharacterSystemInterface characterSystem,
            NpcSystemInterface npcSystem, EnvironmentSystemInterface environmentSystem,
            CombatSystemInterface combatSystem,
            IUISystem uiSystem, ProgressionSystemInterface progressionSystem, TraitSystemInterface traitSystemInterface,
            InventorySystemInterface inventorySystemInterface, IAchievementSystemInterface achievementSystemInterface,
            ShrineSystemInterface shrineSystem)
        {
            this.dataSystem = dataSystem;
            this.npcSystem = npcSystem;
            this.characterSystem = characterSystem;
            this.environmentSystem = environmentSystem;
            this.combatSystem = combatSystem;
            this.uiSystem = uiSystem;
            this.progressionSystem = progressionSystem;
            traitSystem = traitSystemInterface;
            InventorySystem = inventorySystemInterface;
            achievementSystem = achievementSystemInterface;
            this.shrineSystem = shrineSystem;
        }

        CountDownTimer GameTimer;

        GameEffectController GameEffectController;

        GodsBenevolence godsBenevolence;


        ActiveAbilityManager activeAbilityManager;

        DataSystemInterface dataSystem;

        EnvironmentSystemInterface environmentSystem;

        NpcSystemInterface npcSystem;

        CombatSystemInterface combatSystem;
        IAchievementSystemInterface achievementSystem;
        private ShrineSystemInterface shrineSystem;
        IUISystem uiSystem;

        ProgressionSystemInterface progressionSystem;

        CharacterSystemInterface characterSystem;

        TraitSystemInterface traitSystem;

        InventorySystemInterface InventorySystem;

        GameplayContainer container;

        IHealthController characterHealthController;

        BaseCharacterAttackController characterAttackController;

        CharacterStatController characterStatController;

        CharacterVFXController characterVFXController;

        CameraControllerInterface cameraController;

        HitEffectPlayerInterface hitEffectsPlayer;

        BossControllerBase boss;

        GameState currentState;

        int enemiesToKill;

        int CurrentLvlIndex => container.CurrentLvlIndex;

        int MaxLvlIndex => container.MaxLvlIndex;

        public Vector2 GetPlayerSpawnPosition => currentLevelEnvironment
            .GetSpawnpoint(SpawnType.Player).GetSpawnPosition();

        float countDownDelay;

        LevelEnvironment currentLevelEnvironment;

        Level currentLevel;

        List<Environment.Objects.Crystal> crystals = new();

        private bool revivedByFeatThisRun = false;

        private int goldReceiveModifier;

        private int reviveAmount;

        public void Init(Scene scene = default, Action OnComplete = null)
        {
            dataSystem.CurrencyManager.SetCurencyAmount(CurrencyKeys.RuneShard, 0);
            npcSystem.OnEnemySpawned += HandleEnemySpawned;
            combatSystem.OnEntityReceivedDamage += HandleEntityReceivedDamage;
            combatSystem.OnEntityDied += HandleEntityDied;
            uiSystem.OnSpecialButtonClicked += UseCharacterSpecial;
            uiSystem.OnReviveCharacterRequest += ReviveCharacterWithFullHp;
            cameraController = scene.GetComponentInChildren<CameraControllerInterface>();

            activeAbilityManager = scene.GetComponentInChildren<ActiveAbilityManager>();
            godsBenevolence = scene.GetComponentInChildren<GodsBenevolence>();

            container = scene.GetComponentInChildren<GameplayContainer>();
            hitEffectsPlayer = container.GetComponent<StackableSoundPlayer>();

            progressionSystem.BoosterManager.OnBoosterActivated += HandleBoosterActivated;
            progressionSystem.BoosterManager.OnBoosterContainerCreated += HandleBoosterWithDurationActivated;

            GameEffectController = scene.GetComponentInChildren<GameEffectController>();

            container.Init(dataSystem.WorldManger.SelectedWorld);
            container.OnPlayerEnteredPortal += HandlePlayerTriggerPortal;
            container.SetStartingIndex(0);

            npcSystem.NpcContainer.SetMobDifficultyHolder(container.CurrentModel.MobDifficulty);

            GameTimer = new CountDownTimer(container);

            environmentSystem.CurrencySpawner.OnCollected = HandleCurrencyCollected;

            environmentSystem.BoosterSpawner.ActivateBooster = progressionSystem.BoosterManager.ActivateBooster;

            uiSystem.AssignGameEvents();

            shrineSystem.Shrine.GetAngelEffectManager.OnPermanetCard +=
                uiSystem.UiEventHandler.AngelPermanetCardMenu.AcivateCardPermanetEffect;
            uiSystem.UiEventHandler.AngelGambitMenu.CardExit += shrineSystem.Shrine.GetAngelEffectManager.Exists;
            uiSystem.UiEventHandler.AngelGambitMenu.OnCardSelected +=
                shrineSystem.Shrine.GetAngelEffectManager.AddAngelCardSO;
            uiSystem.UiEventHandler.AngelGambitMenu.OnMenuClosed += EnableMovement;

            uiSystem.UiEventHandler.GodsBenevolencePuzzleMenu.GetRandomBenevolenceVisualSO =
                godsBenevolence.GetRandomGodsBenevolenceVisualSO;
            uiSystem.UiEventHandler.GodsBenevolencePuzzleMenu.OnPuzzleSolved += godsBenevolence.ActivateGodsBenevolence;

            uiSystem.UiEventHandler.SummaryMenu.OnMenuOpened += StoreRunReward;
            uiSystem.UiEventHandler.GameMenu.OnSingleLevelUpComplete += HandleSingleLevelUp;
            uiSystem.UiEventHandler.GameMenu.GetPassiveAbilityLevel += activeAbilityManager.GetPassiveAbilityLevel;
            uiSystem.OnRestartLvlRequest += HandleLvlRestart;
            uiSystem.UiEventHandler.GodsBenevolencePuzzleMenu.OnMenuClosed += ContinueGameLoop;
            uiSystem.UiEventHandler.ReviveMenu.OnCloseButtonClicked += HandleGameLoopFinish;
            uiSystem.UiEventHandler.ReviveMenu.OnCountDownCompleted += HandleGameLoopFinish;

            activeAbilityManager.OnEXPAdded += uiSystem.UiEventHandler.GameMenu.UpdateExpBar;

            activeAbilityManager.TimedAbilityControllers[0].OnRuntimeActive += uiSystem.UiEventHandler.GameMenu
                .ActiveAbilityTriggerButtons[0].UpdateSkillOneFill;
            activeAbilityManager.TimedAbilityControllers[0].OnCoolDownActive += uiSystem.UiEventHandler.GameMenu
                .ActiveAbilityTriggerButtons[0].UpdateSkillOneFillCoolDown;
            activeAbilityManager.TimedAbilityControllers[1].OnRuntimeActive += uiSystem.UiEventHandler.GameMenu
                .ActiveAbilityTriggerButtons[1].UpdateSkillOneFill;
            activeAbilityManager.TimedAbilityControllers[1].OnCoolDownActive += uiSystem.UiEventHandler.GameMenu
                .ActiveAbilityTriggerButtons[1].UpdateSkillOneFillCoolDown;
            activeAbilityManager.TimedAbilityControllers[2].OnRuntimeActive += uiSystem.UiEventHandler.GameMenu
                .ActiveAbilityTriggerButtons[2].UpdateSkillOneFill;
            activeAbilityManager.TimedAbilityControllers[2].OnCoolDownActive += uiSystem.UiEventHandler.GameMenu
                .ActiveAbilityTriggerButtons[2].UpdateSkillOneFillCoolDown;
            uiSystem.OnPassiveAbilityButtonClicked += activeAbilityManager.UseCharacterAbility;

            activeAbilityManager.OnActiveAbilityEquipped += uiSystem.UiEventHandler.GameMenu.ActiveAbilityEqquiped;
            activeAbilityManager.OnPassiveAbilityEquipped += uiSystem.UiEventHandler.GameMenu.VisualisePassiveAbility;
            activeAbilityManager.OnPassiveAbilityRemoved += uiSystem.UiEventHandler.GameMenu.RemovePassiveAbility;
            activeAbilityManager.OnRegularActiveAbilitySwapped += uiSystem.UiEventHandler.GameMenu.SwapActiveAbility;
            activeAbilityManager.OnRegularActiveAbilityUpgraded +=
                uiSystem.UiEventHandler.GameMenu.UpgradeActiveAbility;

            uiSystem.UiEventHandler.AbilitySelectMenu.OnRegularAbilitySelected += activeAbilityManager.EquippedAbility;
            uiSystem.UiEventHandler.AbilitySelectMenu.OnPassiveAbilitySelected +=
                activeAbilityManager.AddPassiveAbility;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomActiveAbility +=
                activeAbilityManager.GetRandomActiveAbility;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomActiveAbilityVisualData +=
                activeAbilityManager.GetActiveAbilityVisualData;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetActiveAbilityLevel +=
                activeAbilityManager.GetActiveAbilityLevel;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomPassiveAbility +=
                activeAbilityManager.GetRandomPassiveAbility;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomPassiveAbilityVisualData +=
                activeAbilityManager.GetPassiveAbilityVisualData;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetPassiveAbilityLevel +=
                activeAbilityManager.GetPassiveAbilityLevel;
            uiSystem.UiEventHandler.AbilitySelectMenu.OnMenuClosed += HeroProgressionCompleted;

            CalculateRuneshardBoostModifier();

            uiSystem.UiEventHandler.AbilitySelectMenu.OnGemReRoll += AbilitySelectMenu_OnGemReRoll;
            uiSystem.UiEventHandler.AbilitySelectMenu.OnAdsReRoll += AbilitySelectMenu_OnAdsReRoll;

            RegisterShrineNPCUIEvents();

            OnComplete?.Invoke();
        }

        private void CalculateRuneshardBoostModifier()
        {
            goldReceiveModifier = 0;
            if (traitSystem.HasTraitOfType(TraitType.CurrencyBoost, out var traits))
            {
                foreach (var data in traits)
                {
                    var traitValue = traitSystem.GetTraitEffect(data.TargetTrait.Id);
                    goldReceiveModifier += traitValue.Value + data.Value.Value;
                }
            }
        }

        private void AbilitySelectMenu_OnGemReRoll()
        {
            if (dataSystem.CurrencyManager.GetCurrencyAmount(CurrencyKeys.Gem) >= 10)
            {
                dataSystem.CurrencyManager.ReduceCurency(CurrencyKeys.Gem, 10);
                uiSystem.UiEventHandler.AbilitySelectMenu.ReRoll();
            }
        }

        private void AbilitySelectMenu_OnAdsReRoll()
        {
            dataSystem.AdManager.ShowRewardedAd(() => { uiSystem.UiEventHandler.AbilitySelectMenu.AdsReRoll(); });
        }

        public void Reset()
        {
            Debug.Log("reseting gameplay");
            dataSystem.CurrencyManager.SetCurencyAmount(CurrencyKeys.RuneShard, 0);
            npcSystem.OnEnemySpawned -= HandleEnemySpawned;
            combatSystem.OnEntityReceivedDamage -= HandleEntityReceivedDamage;
            combatSystem.OnEntityDied -= HandleEntityDied;
            ResetLogic();
            ResetConnections();
            container.SetStartingIndex(0);
            revivedByFeatThisRun = false;
            goldReceiveModifier = 0;
            reviveAmount = 0;
            shrineSystem.Shrine.GetAngelEffectManager.ResetAngelEffects();
        }

        private void AbilitySelectMenu_OnMenuClosed()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Used to start game from first lvl
        /// </summary>
        public void StartGameSession()
        {
            uiSystem.UiEventHandler.GameMenu.Open();
            CoroutineUtility.WaitForSeconds(1f, () => // Run the first time the game is loaded
            {
                uiSystem.UiEventHandler.GameMenu.ShowTransition(() => // level transition
                    {
                        uiSystem.UiEventHandler.LoadingMenu.Close();
                        ResetLogic();
                        PreloadLvl();
                        SetupCharacter();
                    }
                    , ContinueGameLoop);
            });
        }

        /// <summary>
        /// Used when any entity in game received died
        /// </summary>
        /// <param name="deathModel"></param>
        void HandleEntityDied(EntityDeathModel deathModel)
        {
            switch (deathModel.EntityType)
            {
                case CombatEntityType.Player:
                    HandleCharacterDeath();
                    break;
                case CombatEntityType.Mob:
                    HandleEnemyDeath(deathModel.Position);
                    break;
                case CombatEntityType.MiniBoss:
                    HandleEnemyDeath(deathModel.Position);
                    SpawnLootFromMiniboss(deathModel.Position);
                    break;
                case CombatEntityType.Boss:
                    break;
            }
        }

        /// <summary>
        /// Used when any entity in game received damage
        /// </summary>
        /// <param name="damageModel"></param>
        void HandleEntityReceivedDamage(EntityDamageReceivedModel damageModel)
        {
            switch (damageModel.EntityType)
            {
                case CombatEntityType.Player:
                    HandleCharacterDamaged(damageModel.DamageIntentModel);
                    break;
                case CombatEntityType.Mob:
                    HandleEnemyDamaged(damageModel.DamageIntentModel);
                    break;
                case CombatEntityType.MiniBoss:
                    HandleEnemyDamaged(damageModel.DamageIntentModel);
                    HandleSpecialEnemyHealthChange(damageModel.HealthPercentagePercentageLeft);
                    break;
                case CombatEntityType.Boss:
                    HandleEnemyDamaged(damageModel.DamageIntentModel);
                    break;
                case CombatEntityType.TempMob:
                    HandleEnemyDamaged(damageModel.DamageIntentModel);
                    break;
                case CombatEntityType.BossCrystal:
                    HandleEnemyDamaged(damageModel.DamageIntentModel);
                    break;
            }
        }

        /// <summary>
        /// Resets session subscriptions
        /// </summary>
        void ResetConnections()
        {
            this.npcSystem.OnEnemySpawned -= HandleEnemySpawned;
            this.combatSystem.OnEntityReceivedDamage -= HandleEntityReceivedDamage;
            this.combatSystem.OnEntityDied -= HandleEntityDied;
            this.uiSystem.OnSpecialButtonClicked -= UseCharacterSpecial;
            this.uiSystem.OnReviveCharacterRequest -= ReviveCharacterWithFullHp;

            shrineSystem.Shrine.GetAngelEffectManager.OnPermanetCard -=
                uiSystem.UiEventHandler.AngelPermanetCardMenu.AcivateCardPermanetEffect;
            uiSystem.UiEventHandler.AngelGambitMenu.CardExit -= shrineSystem.Shrine.GetAngelEffectManager.Exists;
            uiSystem.UiEventHandler.AngelGambitMenu.OnCardSelected -=
                shrineSystem.Shrine.GetAngelEffectManager.AddAngelCardSO;
            uiSystem.UiEventHandler.AngelGambitMenu.OnMenuClosed -= EnableMovement;

            uiSystem.UiEventHandler.GodsBenevolencePuzzleMenu.OnPuzzleSolved -= godsBenevolence.ActivateGodsBenevolence;
            uiSystem.UiEventHandler.SummaryMenu.OnMenuOpened -= StoreRunReward;
            uiSystem.UiEventHandler.GameMenu.OnSingleLevelUpComplete -= HandleSingleLevelUp;
            uiSystem.UiEventHandler.GameMenu.GetPassiveAbilityLevel -= activeAbilityManager.GetPassiveAbilityLevel;
            uiSystem.UiEventHandler.GodsBenevolencePuzzleMenu.OnMenuClosed -= ContinueGameLoop;
            uiSystem.UiEventHandler.ReviveMenu.OnCloseButtonClicked -= HandleGameLoopFinish;
            uiSystem.UiEventHandler.ReviveMenu.OnCountDownCompleted -= HandleGameLoopFinish;

            characterAttackController.OnHitTarget -= OnEnemyHitSuccess;
            characterHealthController.OnDodged -= HandleCharacterDodged;
            characterAttackController = null;
            characterHealthController = null;

            activeAbilityManager.TimedAbilityControllers[0].OnRuntimeActive -= uiSystem.UiEventHandler.GameMenu
                .ActiveAbilityTriggerButtons[0].UpdateSkillOneFill;
            activeAbilityManager.TimedAbilityControllers[0].OnCoolDownActive -= uiSystem.UiEventHandler.GameMenu
                .ActiveAbilityTriggerButtons[0].UpdateSkillOneFillCoolDown;
            activeAbilityManager.TimedAbilityControllers[1].OnRuntimeActive -= uiSystem.UiEventHandler.GameMenu
                .ActiveAbilityTriggerButtons[1].UpdateSkillOneFill;
            activeAbilityManager.TimedAbilityControllers[1].OnCoolDownActive -= uiSystem.UiEventHandler.GameMenu
                .ActiveAbilityTriggerButtons[1].UpdateSkillOneFillCoolDown;
            activeAbilityManager.TimedAbilityControllers[2].OnRuntimeActive -= uiSystem.UiEventHandler.GameMenu
                .ActiveAbilityTriggerButtons[2].UpdateSkillOneFill;
            activeAbilityManager.TimedAbilityControllers[2].OnCoolDownActive -= uiSystem.UiEventHandler.GameMenu
                .ActiveAbilityTriggerButtons[2].UpdateSkillOneFillCoolDown;
            uiSystem.OnPassiveAbilityButtonClicked -= activeAbilityManager.UseCharacterAbility;

            activeAbilityManager.OnActiveAbilityEquipped -= uiSystem.UiEventHandler.GameMenu.ActiveAbilityEqquiped;
            activeAbilityManager.OnPassiveAbilityEquipped -= uiSystem.UiEventHandler.GameMenu.VisualisePassiveAbility;
            activeAbilityManager.OnPassiveAbilityRemoved -= uiSystem.UiEventHandler.GameMenu.RemovePassiveAbility;
            activeAbilityManager.OnRegularActiveAbilitySwapped -= uiSystem.UiEventHandler.GameMenu.SwapActiveAbility;
            activeAbilityManager.OnRegularActiveAbilityUpgraded -=
                uiSystem.UiEventHandler.GameMenu.UpgradeActiveAbility;

            uiSystem.UiEventHandler.AbilitySelectMenu.OnRegularAbilitySelected -= activeAbilityManager.EquippedAbility;
            uiSystem.UiEventHandler.AbilitySelectMenu.OnPassiveAbilitySelected -=
                activeAbilityManager.AddPassiveAbility;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomActiveAbility -=
                activeAbilityManager.GetRandomActiveAbility;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomActiveAbilityVisualData -=
                activeAbilityManager.GetActiveAbilityVisualData;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetActiveAbilityLevel -=
                activeAbilityManager.GetActiveAbilityLevel;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomPassiveAbility -=
                activeAbilityManager.GetRandomPassiveAbility;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomPassiveAbilityVisualData -=
                activeAbilityManager.GetPassiveAbilityVisualData;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetPassiveAbilityLevel -=
                activeAbilityManager.GetPassiveAbilityLevel;
            uiSystem.UiEventHandler.AbilitySelectMenu.OnMenuClosed -= HeroProgressionCompleted;

            uiSystem.UiEventHandler.AbilitySelectMenu.OnGemReRoll -= AbilitySelectMenu_OnGemReRoll;
            uiSystem.UiEventHandler.AbilitySelectMenu.OnAdsReRoll -= AbilitySelectMenu_OnAdsReRoll;

            UnRegisterShrineNPCUIEvents();
        }

        /// <summary>
        /// Used to reset cached logic
        /// </summary>
        void ResetLogic()
        {
            environmentSystem.CurrencySpawner.ResetItems();
            foreach (var crystal in crystals)
            {
                IHealthController healthController = crystal.GetComponent<IHealthController>();
                healthController.OnBeingHitDamaged -= HandleCrystalHit;
                ObjectPoolManager.ReleaseObject(crystal);
            }

            crystals.Clear();

            environmentSystem.BoosterSpawner.ClearAllBoosters();

            activeAbilityManager.ResetAbility();

            // EffectManager.ResetAngelEffects();
            enemiesToKill = 0;
            GameTimer.Stop();
            ChangeState(GameState.Ended);
            uiSystem.ToggleSpecialEnemyHealthBar(false);
        }

        /// <summary>
        /// Used to enable character controller movement
        /// </summary>
        void EnableMovement()
        {
            characterSystem.SetCharacterControllerState(true);
        }

        /// <summary>
        /// Used to trigger character's ultimate ability
        /// </summary>
        void UseCharacterSpecial()
        {
            if (characterHealthController.IsDead())
                return;

            combatSystem.UseCharacterUltimate(() =>
            {
                cameraController.SetCameraState(GameCameraType.Skill);
                TogglePlayerCombatState(false);
                TogglePlayerMovementState(false);
            }, () =>
            {
                cameraController.SetCameraState(GameCameraType.Character);
                TogglePlayerCombatState(true);
                TogglePlayerMovementState(true);
            });
        }

        void ReviveCharacter(float healthPercentage)
        {
            environmentSystem.ParticleManager.Spawn("CharacterRevival",
                characterSystem.CurrentCharacter.CharacterTransform.position);
            combatSystem.RevivePlayer(healthPercentage);
            GameTimer.Resume();
            ChangeState(GameState.Ongoing);
            uiSystem.UiEventHandler.ReviveMenu.Close();
        }

        /// <summary>
        /// Used to load next lvl model
        /// </summary>
        /// <param name="currentLevel"> regerenced model</param>
        /// <returns>true if next lvl model exists</returns>
        bool CheckLevel(ref Level currentLevel)
        {
            currentLevel = container.GetLevel();
            if (currentLevel == null)
            {
                ChangeState(GameState.Won);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Used to spawn enemies from provided Level
        /// </summary>
        /// <param name="currentLvl">target Level to get enemy data from</param>
        void SpawnEnemies(Level currentLvl)
        {
            npcSystem.SpawnEnemies(currentLvl, CurrentLvlIndex);
        }

        /// <summary>
        /// Used to create  player character and proper init it
        /// </summary>
        void SetupCharacter()
        {
            CharacterControllerInterface characterController = characterSystem.CreateCharacter(GetPlayerSpawnPosition);
            characterHealthController =
                characterController.CharacterTransform.GetComponent<CharacterHealthController>();
            characterAttackController =
                characterController.CharacterTransform.GetComponent<BaseCharacterAttackController>();

            characterStatController = characterController.CharacterTransform.GetComponent<CharacterStatController>();
            characterStatController.Initialize(dataSystem.StatManager.GetStatModel());

            CombatEffectsController effectsController =
                characterController.CharacterTransform.GetComponent<CombatEffectsController>();
            combatSystem.RegisterEntity(new CombatEntityModel(characterHealthController, characterAttackController,
                effectsController,
                CombatEntityType.Player));
            combatSystem.InitCharacterUltimate(characterController.CharacterSO.CharacterAnimations.UltAnimationsData,
                characterController.CharacterSO.UltimateData.Charges);

            effectsController.AddCombatEffect(InventorySystem.GetEquippedItemsCombatEffects());

            characterAttackController.OnHitTarget += OnEnemyHitSuccess;

            characterVFXController = characterController.CharacterTransform.GetComponent<CharacterVFXController>();
            characterVFXController.InjectShaker(cameraController.CameraShaker);
            characterHealthController.OnDodged += HandleCharacterDodged;

            characterSystem.SetCharacterControllerState(false);
            cameraController.SetTarget(characterController.CharacterTransform);
            npcSystem.InjectPlayer(characterController.CharacterTransform);

            progressionSystem.BoosterManager.Initialize(characterStatController);

            environmentSystem.CurrencySpawner.SetPlayer(characterController.CharacterTransform);

            shrineSystem.Shrine.Initialize(dataSystem.CurrencyManager, characterStatController, dataSystem.AdManager);
            godsBenevolence.Initialize(characterStatController);

            activeAbilityManager.Initialize(characterStatController);
        }

        void OnEnemyHitSuccess()
        {
            GameEffectController.StopTime(0.1f, container.TimeStopRestoreSpeed,container.TimeStopDuration);
        }

        void HandleEnemySpawned(AiControllerBase obj)
        {
            var healthController = obj.GetComponent<IHealthController>();

            if (obj.EnemyType == EnemyType.MiniBoss)
            {
                HandleMinibossSpawned(obj, healthController);
            }
            else
            {
                HandleMobSpawned(obj, healthController);
            }
        }

        private void HandleMobSpawned(AiControllerBase obj, IHealthController healthController)
        {
            obj.OnDisabled += HandleEnemyDisabled;
            var attackController = obj.GetComponent<IAttackControllerInterface>();
            var effectsController = obj.GetComponent<CombatEffectsController>();
            combatSystem.RegisterEntity(new CombatEntityModel(healthController, attackController, effectsController,
                CombatEntityType.Mob));
        }

        private void HandleMinibossSpawned(AiControllerBase obj, IHealthController healthController)
        {
            var attackController = obj.GetComponent<IAttackControllerInterface>();
            var effectsController = obj.GetComponent<CombatEffectsController>();
            combatSystem.RegisterEntity(new CombatEntityModel(healthController, attackController, effectsController,
                CombatEntityType.MiniBoss));
            HandleSpecialEnemyHealthChange(1f);
            uiSystem.ToggleSpecialEnemyHealthBar(true);
            var mobSpawnAbility = obj.transform.GetComponentInChildren<SpawnEnemyAbility>();
            if (mobSpawnAbility != null)
            {
                mobSpawnAbility.OnEnemySpawned += HandleMobSpawnedByAbility;
            }
        }

        void SpawnLootFromMiniboss(Vector3 position)
        {
            environmentSystem.BoosterSpawner.SpawnBoostLoot(container.CurrentModel.BossDrop,
                position);
        }

        void HandleEnemyDisabled(AiControllerInterface obj)
        {
            obj.OnDisabled -= HandleEnemyDisabled;
            var baseComponent = obj as AiControllerBase;
            environmentSystem.ParticleManager.Spawn("Enemy_Death", baseComponent.transform.position);
        }

        void HandleEnemyDeath(Vector3 position)
        {
            enemiesToKill--;
            environmentSystem.ParticleManager.Spawn("Loot_Spawn", position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            environmentSystem.CurrencySpawner.SpawnAtPosition(CurrencyKeys.RuneShard,
               0, position);

            environmentSystem.CurrencySpawner.SpawnAtPosition(CurrencyKeys.RunExperience, 0, position);

            uiSystem.UpdateEnemiesCounter(enemiesToKill);

            if (currentState != GameState.Ongoing)
                return;

            if (enemiesToKill <= 0)
            {
                GameTimer.Stop();

                Debug.Log(CurrentLvlIndex);
                achievementSystem.AddQuestProgress(
                    new QuestEntry<LevelComplectionQuest>(
                        new LevelComplectionQuest(dataSystem.WorldManger.SelectedWorld)));
                achievementSystem.AddProgressionProgress(new QuestEntry<LevelComplectionQuest>(
                    new LevelComplectionQuest(dataSystem.WorldManger.SelectedWorld, CurrentLvlIndex)));
                HandlePlayerWon();
            }
        }

        void HandlePlayerWon()
        {
            TogglePlayerCombatState(false);
            GameEffectController.ForceStop(() =>
            {
                dataSystem.WorldManger.SetMaxLevelReached(dataSystem.WorldManger.SelectedWorld,
                    container.CurrentLvlIndex);

                if (container.FinishedLoop)
                {
                    TogglePlayerMovementState(false);
                    CoroutineUtility.WaitForSeconds(6f, () => { ChangeState(GameState.Won); });

                    return;
                }

                if (traitSystem.HasTraitOfType(TraitType.HealthRestore, out var traitId))
                {
                    var traitValue = traitSystem.GetTraitEffect(traitId[0].TargetTrait.Id);
                    characterSystem.CurrentCharacter.CharacterTransform.GetComponent<IHealthController>().TryDealDamage(
                        new HealthModificationIntentModel(traitValue.Value, DamageCritType.NoneCritical,
                            AttackType.Healing,
                            CalculationType.Percentage, null));
                }

                ChangeState(GameState.WaitingPortal);
            });
        }

        void HandleCharacterDamaged(HealthModificationIntentModel healthModificationIntentModel)
        {
            if (currentState != GameState.Ongoing)
                return;
        }


        void HandleCharacterDodged()
        {
            characterVFXController.TriggerMissEffect();
        }

        void HandleCharacterDeath()
        {
            Debug.LogError($"character died and game state is {currentState}");
            // if (currentState != GameState.Ongoing)
            //     return;
            characterAttackController.GetComponent<CharacterAnimationController>().StopUltSequence();


            if (!revivedByFeatThisRun && traitSystem.HasTraitOfType(TraitType.Revival, out var traitId))
            {
                revivedByFeatThisRun = true;
                var traitValue = traitSystem.GetTraitEffect(traitId[0].TargetTrait.Id);
                CoroutineUtility.WaitForSeconds(2f, () => { ReviveCharacter(traitValue.Value); });

                return;
            }

            OnLevelFailed?.Invoke();

            dataSystem.WorldManger.SetMaxLevelReached(dataSystem.WorldManger.SelectedWorld, container.CurrentLvlIndex);

            //freezes engine?  
            // GameTimer.Pause();
            CoroutineUtility.WaitForSeconds(1f, () => { ChangeState(GameState.Died); });
        }

        void HandleEnemyDamaged(HealthModificationIntentModel healthModificationIntentModel)
        {
            Particle vfxReference = null;
            var isCritical = healthModificationIntentModel.DamageCritType == DamageCritType.Critical;
            switch (healthModificationIntentModel.AttackType)
            {
                case AttackType.Regular:
                    vfxReference = isCritical
                        ? characterSystem.CurrentCharacter.CharacterSO.VFXData.AutoattackCrit
                        : characterSystem.CurrentCharacter.CharacterSO.VFXData.AutoattackNormal;

                    break;
                case AttackType.Ultimate:
                    vfxReference = isCritical
                        ? characterSystem.CurrentCharacter.CharacterSO.VFXData.UltCrit
                        : characterSystem.CurrentCharacter.CharacterSO.VFXData.UltNormal;
                    break;
            }

            if (healthModificationIntentModel.DamageCritType == DamageCritType.Critical)
            {
                cameraController.CameraShaker.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Explosion, 0.1f,
                    0.20f);
            }
            else
            {
                cameraController.CameraShaker.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Bump, 0.1f, 0.1f);
            }

            if (vfxReference != null)
                environmentSystem.ParticleManager.Spawn(vfxReference,
                    healthModificationIntentModel.DefenderTransform.position);


            //  GameEffectController.StopFrame(0.1f);

            // OnEnemyDamaged?.Invoke(damageModel);
            hitEffectsPlayer.PlayHitEffect("Hit", true);
        }


        void ChangeState(GameState newState)
        {
            if (currentState == newState)
                return;
            currentState = newState;
            Debug.Log(currentState);
            HandleGameStateChanged(newState);
        }

        void HandlePlayerTriggerPortal()
        {
            AudioManager.PlaySoundEffect("EnterPortal", SoundEffectCategory.UI);
            MoveToNextLvl();
        }

        public void StartGameLoop()
        {
            uiSystem.DisplayStartInfoMessage(2f);
            switch (currentLevel.LevelType)
            {
                case LevelType.NormalCombat:

                    enemiesToKill = currentLevel.MiniHasBoss
                        ? currentLevel.TotalMobsToSpawn + 1
                        : currentLevel.TotalMobsToSpawn;
                    uiSystem.UpdateEnemiesCounter(enemiesToKill);
                    combatSystem.StartCharacterComboCheck();
                    TogglePlayerMovementState(true);
                    TogglePlayerCombatState(false);
                    ChangeState(GameState.Ongoing);
                    if (currentLevel.MiniHasBoss)
                    {
                        cameraController.CameraShaker.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Rumble,
                            3f, 3f);
                        uiSystem.ShowSpecialEnemyWarning(EncounterType.Miniboss);
                        TogglePlayerCombatState(true);
                        SpawnEnemies(currentLevel);
                    }
                    else
                    {
                        countDownDelay = .5f;
                        CoroutineUtility.WaitForSeconds(countDownDelay, () =>
                        {
                            GameTimer.Start(2, null,
                                () =>
                                {
                                    TogglePlayerCombatState(true);
                                    SpawnEnemies(currentLevel);
                                    GameTimer.Start(currentLevel.LevelDuration, uiSystem.UpdateGameTimeUI,
                                        () =>
                                        {
                                            if (currentState != GameState.Ongoing)
                                                return;

                                            ChangeState(GameState.TimeEnded);
                                        });
                                }, null);
                        });
                    }


                    uiSystem.UiEventHandler.GameMenu.SetProgressionFill(CurrentLvlIndex / (float)MaxLvlIndex);

                    break;
                case LevelType.Shrine:
                    uiSystem.UiEventHandler.GameMenu.ToggleActionButtonsVisibility(false);
                    characterSystem.SetCharacterControllerState(true);

                    break;
                case LevelType.WorldBoss:
                    enemiesToKill = 2;
                    HandleWorldBoss();
                    break;
            }
        }


        void TogglePlayerMovementState(bool state)
        {
            characterSystem.SetCharacterControllerState(state);
        }

        public Level PreloadLvl()
        {
            if (!CheckLevel(ref currentLevel))
            {
                Debug.LogError("Current lvl loop model has 0 lvls");
                return null;
            }

            AudioManager.PlaySoundEffect("ExitPortal", SoundEffectCategory.Environment);

            SetUpLevelEnvironment();
            return currentLevel;
        }

        void SetUpLevelEnvironment()
        {
            if (currentLevelEnvironment != null)
            {
                GameObject.DestroyImmediate(currentLevelEnvironment.gameObject);
            }

            currentLevelEnvironment = GameObject.Instantiate(currentLevel.LevelPrefab).GetComponent<LevelEnvironment>();

            foreach (var spawnPoint in currentLevelEnvironment.GetSpawnpoints(SpawnType.Crystal))
            {
                var crystal = ObjectPoolManager.SpawnObject(container.CurrentModel.CrystalPrefab,
                    spawnPoint.GetSpawnPosition(), Quaternion.identity);
                IHealthController healthController = crystal.GetComponent<IHealthController>();
                crystal.SpawnLoot = () =>
                {
                    environmentSystem.BoosterSpawner.SpawnBoostLoot(crystal.BoosterDropSO, crystal.transform.position);
                    for (int i = 0; i < crystal.GoldInBatch; i++)
                    {
                        environmentSystem.CurrencySpawner.SpawnAtPosition(CurrencyKeys.RuneShard, crystal.GoldAmount,
                            crystal.transform.position);
                    }
                };
                crystal.OnDestroyed = OnCrystalDestroyed;
                healthController.OnBeingHitDamaged += HandleCrystalHit;
                crystals.Add(crystal);
            }

            cameraController.SetConfiner(currentLevelEnvironment.BoundsCollider);
            npcSystem.NpcContainer.SetSpawnPoints(currentLevelEnvironment.SpawnPointsCache);
            switch (currentLevel.LevelType)
            {
                case LevelType.NormalCombat:
                    container.DisablePortal();
                    break;
                case LevelType.WorldBoss:
                    container.DisablePortal();
                    break;

                case LevelType.Shrine:

                    // When player gets in contact with the NPC

                    ShrineNPCHolder shrineNPCHolder = currentLevelEnvironment.GetComponent<ShrineNPCHolder>();

                    shrineNPCHolder.shrineNPCsCache[ShrineNPCType.AngelsGambit].Initialize(
                        shrineSystem.Shrine.ShrineNPCFeeCache[ShrineNPCType.AngelsGambit],
                        () =>
                        {
                            uiSystem.UiEventHandler.AngelGambitMenu.Open();
                            TogglePlayerMovementState(false);
                        });

                    shrineNPCHolder.shrineNPCsCache[ShrineNPCType.ActiveAbilityReRoller].Initialize(
                        shrineSystem.Shrine.ShrineNPCFeeCache[ShrineNPCType.ActiveAbilityReRoller],
                        () =>
                        {
                            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.Open();
                            TogglePlayerMovementState(false);
                        });

                    shrineNPCHolder.shrineNPCsCache[ShrineNPCType.PassiveAbilityReRoller].Initialize(
                        shrineSystem.Shrine.ShrineNPCFeeCache[ShrineNPCType.PassiveAbilityReRoller],
                        () =>
                        {
                            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.Open();
                            TogglePlayerMovementState(false);
                        });

                    shrineNPCHolder.shrineNPCsCache[ShrineNPCType.HealingMagicRune].Initialize(
                        shrineSystem.Shrine.ShrineNPCFeeCache[ShrineNPCType.HealingMagicRune],
                        () =>
                        {
                            uiSystem.UiEventHandler.HealingNPCMenu.Open(
                                dataSystem.CurrencyManager.GetCurrencyAmount(CurrencyKeys.RuneShard),
                                dataSystem.CurrencyManager.GetCurrencyAmount(CurrencyKeys.Gem));
                            TogglePlayerMovementState(false);
                        });

                    shrineNPCHolder.shrineNPCsCache[ShrineNPCType.Blacksmith].Initialize(
                        shrineSystem.Shrine.ShrineNPCFeeCache[ShrineNPCType.Blacksmith],
                        () => { });

                    container.EnablePortal(currentLevelEnvironment.GetSpawnpoint(SpawnType.Portal).GetSpawnPosition());
                    break;
            }
        }

        void RegisterShrineNPCUIEvents()
        {
            uiSystem.UiEventHandler.AngelGambitMenu.OnMenuClosed += EnableCharacterMovement;
            uiSystem.UiEventHandler.AngelGambitMenu.OnCardSelected += (angelCard) =>
            {
                shrineSystem.Shrine.Purchase(ShrineNPCType.AngelsGambit);
            };


            uiSystem.UiEventHandler.HealingNPCMenu.OnMenuClosed += EnableCharacterMovement;
            uiSystem.UiEventHandler.HealingNPCMenu.GetCurrencyPrice +=
                shrineSystem.Shrine.ShrineNPCFeeCache[ShrineNPCType.HealingMagicRune].GetPrice;
            uiSystem.UiEventHandler.HealingNPCMenu.OnPurchaseRequested += HandleShrineHealerNpcPurchaseRequest;
            // uiSystem.UiEventHandler.HealingNPCMenu.OnPurchaseCompleted += () => { shrine.GetHealer.Heal(); };

            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.OnMenuClosed += EnableCharacterMovement;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetCurrencyPrice +=
                shrineSystem.Shrine.ShrineNPCFeeCache[ShrineNPCType.ActiveAbilityReRoller].GetPrice;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.OnPurchaseRequested += (currencyType) =>
            {
                return shrineSystem.Shrine.Purchase(ShrineNPCType.ActiveAbilityReRoller, currencyType);
            };
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetEqquipedActiveAbilityTypes +=
                activeAbilityManager.GetEqqipedActiveAbilities;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetRandomActiveAbilityTypes +=
                activeAbilityManager.GetRandomActiveAbilityFromAll;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetActiveAbilityVisualData +=
                activeAbilityManager.GetActiveAbilityVisualData;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetActiveAbilityLevel +=
                activeAbilityManager.GetActiveAbilityLevel;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.OnActiveAbilitySwapped +=
                activeAbilityManager.SwapActiveAbility;

            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.OnMenuClosed += () => TogglePlayerMovementState(true);
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetCurrencyPrice +=
                shrineSystem.Shrine.ShrineNPCFeeCache[ShrineNPCType.PassiveAbilityReRoller].GetPrice;
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.OnPurchaseRequested += (currencyType) =>
            {
                return shrineSystem.Shrine.Purchase(ShrineNPCType.PassiveAbilityReRoller, currencyType);
            };
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetEqquipedPassiveAbilityTypes +=
                activeAbilityManager.GetEqquipedPassiveAbilities;
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetRandomPassiveAbilityTypes +=
                activeAbilityManager.GetRandomPassiveAbilityFromAll;
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetPassiveAbilityVisualData +=
                activeAbilityManager.GetPassiveAbilityVisualData;
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetPassiveAbilityLevel +=
                activeAbilityManager.GetPassiveAbilityLevel;
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.OnPassiveAbilitySwapped +=
                activeAbilityManager.SwapPassiveAbility;
        }

        bool HandleShrineHealerNpcPurchaseRequest(ShrineNPCCurrencyType currencyType)
        {
            if (shrineSystem.Shrine.Purchase(ShrineNPCType.HealingMagicRune, currencyType))
            {
                var currentAmount = progressionSystem.GetCurrency(CurrencyKeys.RuneShard);
                int realCurrency = (int)dataSystem.CurrencyManager.GetCurrencyAmount(CurrencyKeys.RuneShard);
                int delta = realCurrency - currentAmount;
                progressionSystem.AddCurrency(CurrencyKeys.RuneShard, delta);
                dataSystem.CurrencyManager.SetCurencyAmount(CurrencyKeys.RuneShard,
                    progressionSystem.GetCurrency(CurrencyKeys.RuneShard));
                uiSystem.UpdateRuinShardUi(progressionSystem.GetCurrency(CurrencyKeys.RuneShard));
                uiSystem.UiEventHandler.PauseMenu.UpdateCurrencyUi(
                    progressionSystem.GetCurrency(CurrencyKeys.RuneShard), 0);
                shrineSystem.Shrine.GetHealer.Heal();
                return true;
            }

            return false;
        }

        void UnRegisterShrineNPCUIEvents()
        {
            uiSystem.UiEventHandler.AngelGambitMenu.OnMenuClosed -= EnableCharacterMovement;
            uiSystem.UiEventHandler.AngelGambitMenu.OnCardSelected -= (angelCard) =>
            {
                shrineSystem.Shrine.Purchase(ShrineNPCType.AngelsGambit);
            };

            uiSystem.UiEventHandler.HealingNPCMenu.OnMenuClosed -= EnableCharacterMovement;
            uiSystem.UiEventHandler.HealingNPCMenu.GetCurrencyPrice -=
                shrineSystem.Shrine.ShrineNPCFeeCache[ShrineNPCType.HealingMagicRune].GetPrice;
            uiSystem.UiEventHandler.HealingNPCMenu.OnPurchaseRequested -= HandleShrineHealerNpcPurchaseRequest;
            // uiSystem.UiEventHandler.HealingNPCMenu.OnPurchaseCompleted -= () => { shrine.GetHealer.Heal(); };

            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.OnMenuClosed -= EnableCharacterMovement;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetCurrencyPrice -=
                shrineSystem.Shrine.ShrineNPCFeeCache[ShrineNPCType.ActiveAbilityReRoller].GetPrice;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.OnPurchaseRequested -= (currencyType) =>
            {
                return shrineSystem.Shrine.Purchase(ShrineNPCType.ActiveAbilityReRoller, currencyType);
            };
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetEqquipedActiveAbilityTypes -=
                activeAbilityManager.GetEqqipedActiveAbilities;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetRandomActiveAbilityTypes -=
                activeAbilityManager.GetRandomActiveAbilityFromAll;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetActiveAbilityVisualData -=
                activeAbilityManager.GetActiveAbilityVisualData;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetActiveAbilityLevel -=
                activeAbilityManager.GetActiveAbilityLevel;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.OnActiveAbilitySwapped -=
                activeAbilityManager.SwapActiveAbility;

            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.OnMenuClosed += () => TogglePlayerMovementState(true);
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetCurrencyPrice +=
                shrineSystem.Shrine.ShrineNPCFeeCache[ShrineNPCType.PassiveAbilityReRoller].GetPrice;
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.OnPurchaseRequested += (currencyType) =>
            {
                return shrineSystem.Shrine.Purchase(ShrineNPCType.PassiveAbilityReRoller, currencyType);
            };
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetEqquipedPassiveAbilityTypes -=
                activeAbilityManager.GetEqquipedPassiveAbilities;
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetRandomPassiveAbilityTypes -=
                activeAbilityManager.GetRandomPassiveAbilityFromAll;
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetPassiveAbilityVisualData -=
                activeAbilityManager.GetPassiveAbilityVisualData;
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetPassiveAbilityLevel -=
                activeAbilityManager.GetPassiveAbilityLevel;
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.OnPassiveAbilitySwapped -=
                activeAbilityManager.SwapPassiveAbility;
        }

        void HandleCrystalHit(Transform transform)
        {
            var crystal = transform.GetComponent<Environment.Objects.Crystal>();
            IHealthController healthController = crystal.GetComponent<IHealthController>();
            crystal.OnHit(healthController.CurrentHit);
        }

        void OnCrystalDestroyed(Environment.Objects.Crystal crystal)
        {
            IHealthController healthController = crystal.GetComponent<IHealthController>();
            healthController.OnBeingHitDamaged -= HandleCrystalHit;
            crystals.Remove(crystal);
            ObjectPoolManager.ReleaseObject(crystal);
        }

        void HandleBoosterActivated(BoosterSO sO, float arg2, Transform transform)
        {
            uiSystem.ShowPopupAtPosition($"+{sO.Abreviation}",
                new Vector2(transform.position.x, transform.position.y + 2), sO.BoosterColor);
            characterVFXController.TriggerBoosterEffect(sO.BoosterEffectType);
        }


        void AddCurrency(string key, int amount)
        {
            progressionSystem.AddCurrency(key, amount);
        }


        public void StoreRunReward()
        {
            progressionSystem.SaveRunResults();
            //collectedHeroProgressionSp = 0;
        }

        void HandleBoosterWithDurationActivated(BoosterContainer container)
        {
            uiSystem.UiEventHandler.GameMenu.VisualiseBooster(container);
        }

        void HandleHeroProgression()
        {
            godsBenevolence.DeactivateGodsBenevolence();
            environmentSystem.CurrencySpawner.ActivateExpEffectItems(() =>
            {
                activeAbilityManager.AddExp(container.GetRunXpForLevel(currentLevel.LevelType));
                progressionSystem.CollectRunCurrency();
            });
        }

        void HandleSingleLevelUp()
        {
            characterVFXController.TriggerLevelUpEffect();
        }

        void HeroProgressionCompleted()
        {
            characterVFXController.TriggerLevelUpAfterEffect();
        }

        void HandleCurrencyCollected(string key, int amount)
        {
            AddCurrency(key, amount);

            switch (key)
            {
                case CurrencyKeys.RuneShard:
                    var currentAmount = progressionSystem.GetCurrency(CurrencyKeys.RuneShard);
                    characterVFXController.TriggerCurrencyEffect(CurrencyKeys.RuneShard);
                    uiSystem.UpdateRuinShardUi(currentAmount);
                    dataSystem.CurrencyManager.SetCurencyAmount(CurrencyKeys.RuneShard, currentAmount);
                    uiSystem.UiEventHandler.PauseMenu.UpdateCurrencyUi(currentAmount, 0);
                    break;
                case CurrencyKeys.Experience:
                    characterVFXController.TriggerCurrencyEffect(CurrencyKeys.Experience);
                    break;
            }
        }

        void HandleGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.Ongoing:
                    break;
                case GameState.Won:
                    HandleGameLoopFinish();
                    break;
                case GameState.Died:
                    //TODO: remove hardcoded values
                    uiSystem.UiEventHandler.ReviveMenu.OpenWithContext(reviveAmount < 2,
                        dataSystem.CurrencyManager.GetCurrencyAmount(CurrencyKeys.Gem) > 50);
                    break;
                case GameState.Ended:
                    break;
                case GameState.WaitingPortal:
                    CoroutineUtility.Start(WaitingPortalRoutine());
                    break;

                case GameState.TimeEnded:
                    uiSystem.UiEventHandler.GameMenu.DisplayInfoMessage(UISystem.GameMenu.InfoMessageType.TimeUp);
                    CoroutineUtility.WaitForSeconds(1f, HandleGameLoopFinish);
                    break;
            }
        }


        //Boss

        void HandleWorldBoss()
        {
            Debug.Log("WORLD BOSS LOGIC");
            if (currentLevel.MiniHasBoss)
            {
                cameraController.CameraShaker.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Rumble,
                    3f, 3f);
                uiSystem.ShowSpecialEnemyWarning(EncounterType.Miniboss);
                uiSystem.UpdateEnemiesCounter(0);
                combatSystem.StartCharacterComboCheck();
                var miniboss = npcSystem.SpawnEntity(currentLevel.GetMinibossPrefab);
                var healthController = miniboss.GetComponent<IHealthController>();


                HandleMinibossSpawned(miniboss, healthController);
                healthController.OnDeath += (health) => { InitWorldBossEncounter(); };
                TogglePlayerCombatState(true);
                TogglePlayerMovementState(true);
            }
            else
            {
                InitWorldBossEncounter();
            }
        }

        private void InitWorldBossEncounter()
        {
            //Init world boss
            AudioManager.PlayMusic(container.CurrentModel.WorldBossMusicKey);
            uiSystem.UpdateEnemiesCounter(0);
            combatSystem.StartCharacterComboCheck();
            ChangeState(GameState.Ongoing);
            cameraController.CameraShaker.ShakeCamera(container.BossProfile);

            uiSystem.ShowSpecialEnemyWarning(EncounterType.Boss);
            uiSystem.UpdateSpecialEnemyHealthBar(1f);
            uiSystem.ToggleSpecialEnemyHealthBar(true);

            environmentSystem.ParticleManager.Spawn("BossSpawn", new Vector2(2.2f, 8.6f));


            cameraController.InitLevelOverview(4.8f, () =>
            {
                TogglePlayerMovementState(true);
                TogglePlayerCombatState(true);
                cameraController.UpdateCharacterCameraFrustrum(1.5f, true);
                boss = npcSystem.NpcContainer.SpawnBoss(currentLevel);
                boss.OnBossStateChange += HandleBossStateChange;
                boss.OnBeingDamaged += HandleBossDamaged;

                boss.OnHealthPercentageChange += HandleSpecialEnemyHealthChange;
                boss.OnCrystalDestroyed += SpawnLootFromBoss;
                characterSystem.SetCharacterControllerState(true);
                boss.Init(npcSystem.NpcContainer.MobDifficulties.GetHealth(CurrentLvlIndex, EnemyType.Boss),
                    npcSystem.NpcContainer.MobDifficulties.GetDamage(CurrentLvlIndex, EnemyType.Boss));
                foreach (var health in boss.CrystalNodes)
                {
                    var effectsHandler = health.HealthTransform.GetComponent<CombatEffectsController>();
                    combatSystem.RegisterEntity(new CombatEntityModel(health, effectsHandler,
                        CombatEntityType.BossCrystal));
                }

                var mushroomTriggerAbility = boss.transform.GetComponentInChildren<BossMushroomSummonAbility>();
                if (mushroomTriggerAbility != null)
                {
                    mushroomTriggerAbility.OnEnemySpawned += HandleMobSpawnedByAbility;
                }

                var mobSpawnAbility = boss.transform.GetComponentInChildren<SpawnEnemyAbility>();
                if (mobSpawnAbility != null)
                {
                    mobSpawnAbility.OnEnemySpawned += HandleMobSpawnedByAbility;
                }
            });
        }

        private void HandleMobSpawnedByAbility(IHealthController healthController)
        {
            var aiController = healthController.HealthTransform.GetComponent<AiControllerBase>();
            aiController.Init(characterVFXController.transform,
                container.CurrentModel.MobDifficulty.GetHealth(container.CurrentLvlIndex,
                    aiController.AgentModel.EnemyType),
                container.CurrentModel.MobDifficulty.GetDamage(container.CurrentLvlIndex,
                    aiController.AgentModel.EnemyType),
                npcSystem.NpcContainer.MonsterStatController.GetMonsterStatModifier, null);
            var attackController =
                healthController.HealthTransform.GetComponent<IAttackControllerInterface>();
            var effectsController =
                healthController.HealthTransform.GetComponent<CombatEffectsController>();
            combatSystem.RegisterEntity(new CombatEntityModel(healthController, attackController,
                effectsController,
                CombatEntityType.TempMob));
        }

        void HandleBossStateChange(BossState obj)
        {
            if (obj == BossState.Dead)
            {
                boss.OnBossStateChange -= HandleBossStateChange;
                boss.OnBeingDamaged -= HandleBossDamaged;
                boss.OnHealthPercentageChange -= HandleSpecialEnemyHealthChange;
                boss.OnCrystalDestroyed -= SpawnLootFromBoss;
                combatSystem.ManuallyNotifyEntityDeath(new EntityDeathModel(Vector3.zero, CombatEntityType.Boss));
                HandlePlayerWon();
            }
        }

        void HandleBossDamaged(HealthModificationIntentModel damageIntentModel)
        {
        }

        void HandleSpecialEnemyHealthChange(float amount)
        {
            Debug.Log($"updating with {amount}");
            uiSystem.UpdateSpecialEnemyHealthBar(amount);
        }

        void SpawnLootFromBoss(Transform target)
        {
            Debug.Log("Crystal destroyed spawning loot");
            environmentSystem.BoosterSpawner.SpawnBoostLoot(container.CurrentModel.BossDrop,
                target.transform.position);
        }

        IEnumerator WaitingPortalRoutine()
        {
            uiSystem.UiEventHandler.GameMenu.DisplayInfoMessage(UISystem.GameMenu.InfoMessageType.Complete);

            yield return new WaitForSeconds(2f);

            void ContinueWaitForPortalRoutine()
            {
                uiSystem.UiEventHandler.GameMenu.OnUpdateXpBarCompleted -= ContinueWaitForPortalRoutine;
                CoroutineUtility.Start(ContinueAfterXpBarUpdate());
            }

            uiSystem.UiEventHandler.GameMenu.OnUpdateXpBarCompleted += ContinueWaitForPortalRoutine;
            HandleHeroProgression();
        }

        IEnumerator ContinueAfterXpBarUpdate()
        {
            yield return new WaitUntil(() =>
                uiSystem.UiEventHandler.AbilitySelectMenu.MenuStatus == UISystem.Menu.Status.Closed);
            yield return new WaitForSeconds(1f);
            if (shrineSystem.Shrine.GetAngelEffectManager.CompletedLevel())
            {
                yield return new WaitUntil(() =>
                    uiSystem.UiEventHandler.AngelPermanetCardMenu.MenuStatus ==
                    UISystem.Menu.Status.Closed);
            }

            ShowLevelPortal();
        }

        void ShowLevelPortal()
        {
            CoroutineUtility.WaitForSeconds(1f,
                () =>
                {
                    container.EnablePortal(currentLevelEnvironment.GetSpawnpoint(SpawnType.Portal).GetSpawnPosition());
                });
        }

        void ShowGodBenevolencePrompt()
        {
            if (characterStatController.GetHealthPercentage() <= 30)
            {
                uiSystem.UiEventHandler.ConfirmationMenu.Display(uiSystem.UiEventHandler.PuzzleConfirmation,
                    uiSystem.UiEventHandler.GodsBenevolencePuzzleMenu.Open,
                    ContinueGameLoop);
            }
            else
            {
                ContinueGameLoop();
            }
        }

        void HandleGameLoopFinish()
        {
            dataSystem.AccountLevelManager.AddExp(container.CurrentModel.PermanentXpPerRoom * CurrentLvlIndex);
            uiSystem.UiEventHandler.SummaryMenu.Open();
        }

        void HandleLvlRestart()
        {
            characterSystem.Reset();
            npcSystem.Reset();
            Reset();
            uiSystem.UiEventHandler.ReviveMenu.Close();
            uiSystem.UiEventHandler.AngelPermanetCardMenu.ResetMenu();
            SetupCharacter();
            StartGameLoop();
        }

        void MoveToNextLvl()
        {
            if (currentLevel.LevelType == LevelType.Shrine)
            {
                shrineSystem.Shrine.OnShrineExit();
            }

            CoroutineUtility.WaitForSeconds(0.5f, () =>
            {
                Level newLevel = null;
                uiSystem.UiEventHandler.GameMenu.ShowTransition(() => // level to level transition
                    {
                        ResetLogic();
                        npcSystem.Reset();

                        newLevel = PreloadLvl();
                        characterSystem.ResetCharacter(GetPlayerSpawnPosition);
                    },
                    () =>
                    {
                        if (newLevel.LevelType == LevelType.NormalCombat ||
                            newLevel.LevelType == LevelType.WorldBoss)
                        {
                            CoroutineUtility.Start(ContinueGameLoopRoutine());
                            uiSystem.UiEventHandler.GameMenu.ToggleActionButtonsVisibility(true);
                        }
                        else
                        {
                            TogglePlayerMovementState(true);
                        }
                    });
            });
        }

        void ContinueGameLoop()
        {
            CoroutineUtility.WaitForSeconds(0.5f, () =>
            {
                npcSystem.Reset();
                //TogglePlayerCombatState(false,false);
                StartGameLoop();
            });
        }

        IEnumerator ContinueGameLoopRoutine()
        {
            yield return new WaitForSeconds(0.5f);

            if (CurrentLvlIndex != MaxLvlIndex) // Open every second lvl
            {
                ShowGodBenevolencePrompt();
            }
            else
            {
                ContinueGameLoop();
            }
        }

        private void ReviveCharacterWithFullHp(ReviveRequestModel reviveRequestModel)
        {
            if (reviveAmount >= 2)
            {
                return;
            }

            switch (reviveRequestModel.ReviveType)
            {
                case UiReviveType.Gems:
                    if (dataSystem.CurrencyManager.GetCurrencyAmount(CurrencyKeys.Gem) >= reviveRequestModel.Cost)
                    {
                        dataSystem.CurrencyManager.ReduceCurency(CurrencyKeys.Gem, reviveRequestModel.Cost);
                        ReviveCharacter(100f);

                        reviveAmount++;
                    }

                    break;
                case UiReviveType.Ads:
                    dataSystem.AdManager.ShowRewardedAd(() =>
                    {
                        ReviveCharacter(100f);
                        reviveAmount++;
                    });
                    break;
            }
        }

        void TogglePlayerCombatState(bool canAttack)
        {
            characterAttackController.ToggleControllerState(canAttack);
            characterHealthController.SetInvulnerableState(!canAttack);
        }

        void EnableCharacterMovement()
        {
            TogglePlayerMovementState(true);
        }
    }
}