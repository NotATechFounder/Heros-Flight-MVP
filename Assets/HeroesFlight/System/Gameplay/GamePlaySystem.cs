using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using HeroesFlight.Common.Enum;
using HeroesFlight.Common.Progression;
using HeroesFlight.System.Character;
using HeroesFlight.System.Combat;
using HeroesFlight.System.Combat.Effects.Effects;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Combat.Handlers;
using HeroesFlight.System.Combat.Model;
using HeroesFlight.System.Combat.StatusEffects;
using HeroesFlight.System.Environment;
using HeroesFlight.System.FileManager.Enum;
using HeroesFlight.System.FileManager.Model;
using HeroesFlight.System.Gameplay.Container;
using HeroesFlight.System.Gameplay.Controllers.Sound;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC;
using HeroesFlight.System.NPC.Controllers.Ability;
using HeroesFlight.System.NPC.Controllers.Control;
using HeroesFlight.System.NPC.Model;
using HeroesFlight.System.Stats;
using HeroesFlight.System.Stats.Handlers;
using HeroesFlight.System.Stats.Traits.Effects;
using HeroesFlight.System.Stats.Traits.Enum;
using HeroesFlight.System.UI;
using HeroesFlightProject.System.Combat.Controllers;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Enum;
using Pelumi.ObjectPool;
using Plugins.Audio_System;
using StansAssets.Foundation.Async;
using StansAssets.Foundation.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CurrencySO;

namespace HeroesFlight.System.Gameplay
{
    public class GamePlaySystem : GamePlaySystemInterface
    {
        public GamePlaySystem(DataSystemInterface dataSystem, CharacterSystemInterface characterSystem,
            NpcSystemInterface npcSystem, EnvironmentSystemInterface environmentSystem,
            CombatSystemInterface combatSystem,
            IUISystem uiSystem, ProgressionSystemInterface progressionSystem, TraitSystemInterface traitSystemInterface)
        {
            this.dataSystem = dataSystem;
            this.npcSystem = npcSystem;
            this.characterSystem = characterSystem;
            this.environmentSystem = environmentSystem;
            this.combatSystem = combatSystem;
            this.uiSystem = uiSystem;
            this.progressionSystem = progressionSystem;
            traitSystem = traitSystemInterface;
            this.npcSystem.OnEnemySpawned += HandleEnemySpawned;
            this.combatSystem.OnEntityReceivedDamage += HandleEntityReceivedDamage;
            this.combatSystem.OnEntityDied += HandleEntityDied;
            this.uiSystem.OnSpecialButtonClicked += UseCharacterSpecial;
            this.uiSystem.OnReviveCharacterRequest += () => { ReviveCharacter(100f); };
        }

        CountDownTimer GameTimer;
        GameEffectController GameEffectController;

        GodsBenevolence godsBenevolence;
        Shrine shrine;
        ActiveAbilityManager activeAbilityManager;

        DataSystemInterface dataSystem;
        EnvironmentSystemInterface environmentSystem;
        NpcSystemInterface npcSystem;
        CombatSystemInterface combatSystem;
        IUISystem uiSystem;
        ProgressionSystemInterface progressionSystem;
        CharacterSystemInterface characterSystem;
        TraitSystemInterface traitSystem;

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
        private int goldModifier;

        public void Init(Scene scene = default, Action OnComplete = null)
        {
            cameraController = scene.GetComponentInChildren<CameraControllerInterface>();

            shrine = scene.GetComponentInChildren<Shrine>();
            activeAbilityManager = scene.GetComponentInChildren<ActiveAbilityManager>();
            godsBenevolence = scene.GetComponentInChildren<GodsBenevolence>();

            container = scene.GetComponentInChildren<GameplayContainer>();
            hitEffectsPlayer = container.GetComponent<StackableSoundPlayer>();

            progressionSystem.BoosterManager.OnBoosterActivated += HandleBoosterActivated;
            progressionSystem.BoosterManager.OnBoosterContainerCreated += HandleBoosterWithDurationActivated;

            GameEffectController = scene.GetComponentInChildren<GameEffectController>();

            container.Init();
            container.OnPlayerEnteredPortal += HandlePlayerTriggerPortal;
            container.SetStartingIndex(0);

            npcSystem.NpcContainer.SetMobDifficultyHolder(container.CurrentModel.MobDifficulty);

            GameTimer = new CountDownTimer(container);

            environmentSystem.CurrencySpawner.OnCollected = HandleCurrencyCollected;

            environmentSystem.BoosterSpawner.ActivateBooster = progressionSystem.BoosterManager.ActivateBooster;

            shrine.GetAngelEffectManager.OnPermanetCard +=
                uiSystem.UiEventHandler.AngelPermanetCardMenu.AcivateCardPermanetEffect;
            uiSystem.UiEventHandler.AngelGambitMenu.CardExit += shrine.GetAngelEffectManager.Exists;
            uiSystem.UiEventHandler.AngelGambitMenu.OnCardSelected += shrine.GetAngelEffectManager.AddAngelCardSO;
            uiSystem.UiEventHandler.AngelGambitMenu.OnMenuClosed += EnableMovement;

            uiSystem.UiEventHandler.GodsBenevolencePuzzleMenu.GetRandomBenevolenceVisualSO =  godsBenevolence.GetRandomGodsBenevolenceVisualSO;
            uiSystem.UiEventHandler.GodsBenevolencePuzzleMenu.OnPuzzleSolved +=    godsBenevolence.ActivateGodsBenevolence;

            uiSystem.UiEventHandler.SummaryMenu.OnMenuOpened += StoreRunReward;
            uiSystem.UiEventHandler.GameMenu.OnSingleLevelUpComplete += HandleSingleLevelUp;
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

            uiSystem.UiEventHandler.AbilitySelectMenu.OnRegularAbilitySelected += activeAbilityManager.EquippedAbility;
            uiSystem.UiEventHandler.AbilitySelectMenu.OnPassiveAbilitySelected += activeAbilityManager.AddPassiveAbility;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomActiveAbility += activeAbilityManager.GetRandomActiveAbility;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomActiveAbilityVisualData += activeAbilityManager.GetActiveAbilityVisualData;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetActiveAbilityLevel += activeAbilityManager.GetActiveAbilityLevel;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomPassiveAbility += activeAbilityManager.GetRandomPassiveAbility;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomPassiveAbilityVisualData += activeAbilityManager.GetPassiveAbilityVisualData;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetPassiveAbilityLevel += activeAbilityManager.GetPassiveAbilityLevel;
            uiSystem.UiEventHandler.AbilitySelectMenu.OnMenuClosed += HeroProgressionCompleted;

            goldModifier = 0;
            if (traitSystem.HasTraitOfType(TraitType.CurrencyBoost, out var traits))
            {
                foreach (var data in traits)
                {
                    var traitValue = traitSystem.GetTraitEffect(data.TargetTrait.Id);
                    goldModifier += traitValue.Value + data.Value.Value;
                }
            }

            RegisterShrineNPCUIEvents();

            OnComplete?.Invoke();
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

        public void Reset()
        {
            ResetLogic();
            ResetConnections();
            container.SetStartingIndex(0);
            revivedByFeatThisRun = false;
            goldModifier = 0;
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
                 //   HandleMinibossHealthChange(damageModel.HealthPercentagePercentageLeft);
                    break;
                case CombatEntityType.Boss:
                    HandleEnemyDamaged(damageModel.DamageIntentModel);
                    break;
                case CombatEntityType.TempMob:
                    HandleEnemyDamaged(damageModel.DamageIntentModel);
                    break;
            }
        }

        /// <summary>
        /// Resets session subscriptions
        /// </summary>
        void ResetConnections()
        {
            shrine.GetAngelEffectManager.OnPermanetCard -=
                uiSystem.UiEventHandler.AngelPermanetCardMenu.AcivateCardPermanetEffect;
            uiSystem.UiEventHandler.AngelGambitMenu.CardExit -= shrine.GetAngelEffectManager.Exists;
            uiSystem.UiEventHandler.AngelGambitMenu.OnCardSelected -= shrine.GetAngelEffectManager.AddAngelCardSO;
            uiSystem.UiEventHandler.AngelGambitMenu.OnMenuClosed -= EnableMovement;

            uiSystem.UiEventHandler.GodsBenevolencePuzzleMenu.OnPuzzleSolved -=  godsBenevolence.ActivateGodsBenevolence;
            uiSystem.UiEventHandler.SummaryMenu.OnMenuOpened -= StoreRunReward;
            uiSystem.UiEventHandler.GameMenu.OnSingleLevelUpComplete -= HandleSingleLevelUp;
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

            uiSystem.UiEventHandler.AbilitySelectMenu.OnRegularAbilitySelected -= activeAbilityManager.EquippedAbility;
            uiSystem.UiEventHandler.AbilitySelectMenu.OnPassiveAbilitySelected -= activeAbilityManager.AddPassiveAbility;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomActiveAbility -= activeAbilityManager.GetRandomActiveAbility;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomActiveAbilityVisualData -= activeAbilityManager.GetActiveAbilityVisualData;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetActiveAbilityLevel -= activeAbilityManager.GetActiveAbilityLevel;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomPassiveAbility -= activeAbilityManager.GetRandomPassiveAbility;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomPassiveAbilityVisualData -= activeAbilityManager.GetPassiveAbilityVisualData;
            uiSystem.UiEventHandler.AbilitySelectMenu.GetPassiveAbilityLevel -= activeAbilityManager.GetPassiveAbilityLevel;
            uiSystem.UiEventHandler.AbilitySelectMenu.OnMenuClosed -= HeroProgressionCompleted;

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
                characterHealthController.SetInvulnerableState(true);
                characterSystem.SetCharacterControllerState(false);
                characterAttackController.ToggleControllerState(false);
            }, () =>
            {
                cameraController.SetCameraState(GameCameraType.Character);
                characterSystem.SetCharacterControllerState(true);
                characterAttackController.ToggleControllerState(true);
                characterHealthController.SetInvulnerableState(false);
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
           // progressionSystem.HeroProgression.Initialise(characterStatController);
            UpdateStatModifiers();


            var effectsController = characterController.CharacterTransform.GetComponent<CombatEffectsController>();
            combatSystem.RegisterEntity(new CombatEntityModel(characterHealthController, characterAttackController,
                effectsController,
                CombatEntityType.Player));
            combatSystem.InitCharacterUltimate(characterController.CharacterSO.CharacterAnimations.UltAnimationsData,
                characterController.CharacterSO.UltimateData.Charges);

            characterAttackController.OnHitTarget += OnEnemyHitSuccess;

            characterVFXController = characterController.CharacterTransform.GetComponent<CharacterVFXController>();
            characterVFXController.InjectShaker(cameraController.CameraShaker);
            characterHealthController.OnDodged += HandleCharacterDodged;

            characterSystem.SetCharacterControllerState(false);
            cameraController.SetTarget(characterController.CharacterTransform);
            npcSystem.InjectPlayer(characterController.CharacterTransform);

            progressionSystem.BoosterManager.Initialize(characterStatController);

            environmentSystem.CurrencySpawner.SetPlayer(characterController.CharacterTransform);

            shrine.Initialize(dataSystem.CurrencyManager, characterStatController);
            godsBenevolence.Initialize(characterStatController);

            activeAbilityManager.Initialize(characterStatController);
        }

        void OnEnemyHitSuccess()
        {
            GameEffectController.StopTime(0.1f, container.CurrentModel.TimeStopRestoreSpeed,
                container.CurrentModel.TimeStopDuration);
        }

        void HandleMinibossHealthChange(float healthProportion)
        {
            uiSystem.UpdateSpecialEnemyHealthBar(healthProportion);
        }

        void HandleEnemySpawned(AiControllerBase obj)
        {
            var healthController = obj.GetComponent<IHealthController>();

            if (obj.EnemyType == EnemyType.MiniBoss)
            {
                var attackController = obj.GetComponent<IAttackControllerInterface>();
                var effectsController = obj.GetComponent<CombatEffectsController>();
                combatSystem.RegisterEntity(new CombatEntityModel(healthController, attackController, effectsController,
                    CombatEntityType.MiniBoss));
                uiSystem.ToggleSpecialEnemyHealthBar(true);
                var spawnAbility = obj.transform.GetComponentInChildren<SpawnEnemyAbility>();
                if (spawnAbility != null)
                {
                    spawnAbility.OnEnemySpawned += (healthController) =>
                    {
                        var attackController =
                            healthController.HealthTransform.GetComponent<IAttackControllerInterface>();
                        var effectsController =
                            healthController.HealthTransform.GetComponent<CombatEffectsController>();
                        combatSystem.RegisterEntity(new CombatEntityModel(healthController, attackController,
                            effectsController,
                            CombatEntityType.TempMob));
                    };
                }
            }
            else
            {
                obj.OnDisabled += HandleEnemyDisabled;
                var attackController = obj.GetComponent<IAttackControllerInterface>();
                var effectsController = obj.GetComponent<CombatEffectsController>();
                combatSystem.RegisterEntity(new CombatEntityModel(healthController, attackController, effectsController,
                    CombatEntityType.Mob));
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
            environmentSystem.ParticleManager.Spawn("Loot_Spawn", position,
                Quaternion.Euler(new Vector3(-90, 0, 0)));

            environmentSystem.CurrencySpawner.SpawnAtPosition(CurrencyKeys.Gold, 10 + goldModifier,
                position);
            environmentSystem.CurrencySpawner.SpawnAtPosition(CurrencyKeys.RunExperience, 10,
                position);
            progressionSystem.AddCurrency(CurrencyKeys.RunExperience, (int)container.HeroProgressionExpEarnedPerKill);

            uiSystem.UpdateEnemiesCounter(enemiesToKill);
            if (currentState != GameState.Ongoing)
                return;

            if (enemiesToKill <= 0)
            {
                GameTimer.Stop();
                HandlePlayerWon();
            }
        }

        void HandlePlayerWon()
        {
            GameEffectController.ForceStop(() =>
            {
                if (container.FinishedLoop)
                {
                    characterAttackController.ToggleControllerState(false);

                    //Temp rewarding player with unlock here
                    dataSystem.RewardHandler.GrantReward(new HeroRewardModel(RewardType.Hero,
                        CharacterType.Lancer));

                    CoroutineUtility.WaitForSeconds(6f, () => { ChangeState(GameState.Won); });

                    return;
                }

                if (traitSystem.HasTraitOfType(TraitType.HealthRestore, out var traitId))
                {
                    var traitValue = traitSystem.GetTraitEffect(traitId[0].TargetTrait.Id);
                    characterSystem.CurrentCharacter.CharacterTransform.GetComponent<IHealthController>().TryDealDamage(
                        new HealthModificationIntentModel(traitValue.Value, DamageCritType.NoneCritical, AttackType.Healing,
                            CalculationType.Percentage,null));
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
            if (currentState != GameState.Ongoing)
                return;
            characterAttackController.GetComponent<CharacterAnimationController>().StopUltSequence();


            if (!revivedByFeatThisRun && traitSystem.HasTraitOfType(TraitType.Revival, out var traitId))
            {
                revivedByFeatThisRun = true;
                var traitValue = traitSystem.GetTraitEffect(traitId[0].TargetTrait.Id);
                CoroutineUtility.WaitForSeconds(2f, () => { ReviveCharacter(traitValue.Value); });

                return;
            }

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
                    healthModificationIntentModel.TargetTransform.position);


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

                    ChangeState(GameState.Ongoing);
                    if (currentLevel.MiniHasBoss)
                    {
                        cameraController.CameraShaker.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Rumble,
                            3f, 3f);
                        uiSystem.ShowSpecialEnemyWarning(EncounterType.Miniboss);
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

                    characterSystem.SetCharacterControllerState(true);

                    break;
                case LevelType.Shrine:
                    characterSystem.SetCharacterControllerState(true);
                    break;
                case LevelType.WorldBoss:
                    HandleWorldBoss();
                    break;
            }
        }


        void TogglePlayerMovement(bool state)
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
                        environmentSystem.CurrencySpawner.SpawnAtPosition(CurrencyKeys.Gold, crystal.GoldAmount,
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
                        shrine.ShrineNPCFeeCache[ShrineNPCType.AngelsGambit],
                        () =>
                        {
                            uiSystem.UiEventHandler.AngelGambitMenu.Open();
                            TogglePlayerMovement(false);
                        });

                    shrineNPCHolder.shrineNPCsCache[ShrineNPCType.ActiveAbilityReRoller].Initialize(
                        shrine.ShrineNPCFeeCache[ShrineNPCType.ActiveAbilityReRoller],
                        () =>
                        {
                            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.Open();
                            TogglePlayerMovement(false);
                        });

                    shrineNPCHolder.shrineNPCsCache[ShrineNPCType.PassiveAbilityReRoller].Initialize(
                        shrine.ShrineNPCFeeCache[ShrineNPCType.PassiveAbilityReRoller],
                        () =>
                        {
                            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.Open();
                            TogglePlayerMovement(false);
                        });

                    shrineNPCHolder.shrineNPCsCache[ShrineNPCType.HealingMagicRune].Initialize(
                        shrine.ShrineNPCFeeCache[ShrineNPCType.HealingMagicRune],
                        () =>
                        {
                            uiSystem.UiEventHandler.HealingNPCMenu.Open();
                            TogglePlayerMovement(false);
                        });

                    shrineNPCHolder.shrineNPCsCache[ShrineNPCType.Blacksmith].Initialize(
                        shrine.ShrineNPCFeeCache[ShrineNPCType.Blacksmith],
                        () => { });

                    container.EnablePortal(currentLevelEnvironment.GetSpawnpoint(SpawnType.Portal).GetSpawnPosition());
                    break;
            }
        }

        void RegisterShrineNPCUIEvents()
        {
            uiSystem.UiEventHandler.AngelGambitMenu.OnMenuClosed += () => TogglePlayerMovement(true);
            uiSystem.UiEventHandler.AngelGambitMenu.OnCardSelected += (angelCard) =>
            {
                shrine.Purchase(ShrineNPCType.AngelsGambit);
            };


            uiSystem.UiEventHandler.HealingNPCMenu.OnMenuClosed += () => TogglePlayerMovement(true);
            uiSystem.UiEventHandler.HealingNPCMenu.GetCurrencyPrice +=
                shrine.ShrineNPCFeeCache[ShrineNPCType.HealingMagicRune].GetPrice;
            uiSystem.UiEventHandler.HealingNPCMenu.OnPurchaseRequested += (currencyType) =>
            {
                return shrine.Purchase(ShrineNPCType.HealingMagicRune, currencyType);
            };
            uiSystem.UiEventHandler.HealingNPCMenu.OnPurchaseCompleted += () => { shrine.GetHealer.Heal(); };

            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.OnMenuClosed += () => TogglePlayerMovement(true);
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetCurrencyPrice +=
                shrine.ShrineNPCFeeCache[ShrineNPCType.ActiveAbilityReRoller].GetPrice;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.OnPurchaseRequested += (currencyType) =>
            {
                return shrine.Purchase(ShrineNPCType.ActiveAbilityReRoller, currencyType);
            };
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetEqquipedActiveAbilityTypes += activeAbilityManager.GetEqqipedActiveAbilities;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetRandomActiveAbilityTypes += activeAbilityManager.GetRandomActiveAbilityFromAll;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetActiveAbilityVisualData += activeAbilityManager.GetActiveAbilityVisualData;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.OnActiveAbilitySwapped += activeAbilityManager.SwapActiveAbility;

            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.OnMenuClosed += () => TogglePlayerMovement(true);
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetCurrencyPrice +=
                shrine.ShrineNPCFeeCache[ShrineNPCType.PassiveAbilityReRoller].GetPrice;
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.OnPurchaseRequested += (currencyType) =>
            {
                return shrine.Purchase(ShrineNPCType.PassiveAbilityReRoller, currencyType);
            };
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetEqquipedPassiveAbilityTypes += activeAbilityManager.GetEqquipedPassiveAbilities;
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetRandomPassiveAbilityTypes += activeAbilityManager.GetRandomPassiveAbilityFromAll;
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetPassiveAbilityVisualData += activeAbilityManager.GetPassiveAbilityVisualData;
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.OnPassiveAbilitySwapped += activeAbilityManager.SwapPassiveAbility;
        }

        void UnRegisterShrineNPCUIEvents()
        {
            uiSystem.UiEventHandler.AngelGambitMenu.OnMenuClosed -= () => TogglePlayerMovement(true);
            uiSystem.UiEventHandler.AngelGambitMenu.OnCardSelected -= (angelCard) =>
            {
                shrine.Purchase(ShrineNPCType.AngelsGambit);
            };

            uiSystem.UiEventHandler.HealingNPCMenu.OnMenuClosed -= () => TogglePlayerMovement(true);
            uiSystem.UiEventHandler.HealingNPCMenu.GetCurrencyPrice -=
                shrine.ShrineNPCFeeCache[ShrineNPCType.HealingMagicRune].GetPrice;
            uiSystem.UiEventHandler.HealingNPCMenu.OnPurchaseRequested -= (currencyType) =>
            {
                return shrine.Purchase(ShrineNPCType.HealingMagicRune, currencyType);
            };
            uiSystem.UiEventHandler.HealingNPCMenu.OnPurchaseCompleted -= () => { shrine.GetHealer.Heal(); };

            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.OnMenuClosed += () => TogglePlayerMovement(true);
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetCurrencyPrice +=
                shrine.ShrineNPCFeeCache[ShrineNPCType.ActiveAbilityReRoller].GetPrice;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.OnPurchaseRequested += (currencyType) =>
            {
                return shrine.Purchase(ShrineNPCType.ActiveAbilityReRoller, currencyType);
            };
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetEqquipedActiveAbilityTypes += activeAbilityManager.GetEqqipedActiveAbilities;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetRandomActiveAbilityTypes += activeAbilityManager.GetRandomActiveAbilityFromAll;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetActiveAbilityVisualData += activeAbilityManager.GetActiveAbilityVisualData;
            uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.OnActiveAbilitySwapped += activeAbilityManager.SwapActiveAbility;

            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.OnMenuClosed += () => TogglePlayerMovement(true);
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetCurrencyPrice +=
                shrine.ShrineNPCFeeCache[ShrineNPCType.PassiveAbilityReRoller].GetPrice;
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.OnPurchaseRequested += (currencyType) =>
            {
                return shrine.Purchase(ShrineNPCType.PassiveAbilityReRoller, currencyType);
            };
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetEqquipedPassiveAbilityTypes -= activeAbilityManager.GetEqquipedPassiveAbilities;
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetRandomPassiveAbilityTypes -= activeAbilityManager.GetRandomPassiveAbilityFromAll;
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetPassiveAbilityVisualData -= activeAbilityManager.GetPassiveAbilityVisualData;
            uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.OnPassiveAbilitySwapped -= activeAbilityManager.SwapPassiveAbility;
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
            environmentSystem.CurrencySpawner.ActivateExpItems(() =>
            {
                Debug.Log("EXP ITEMS ACTIVATED");
                activeAbilityManager.AddExp(progressionSystem.GetCurrency(CurrencyKeys.RunExperience));
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
                case CurrencyKeys.Gold:
                    characterVFXController.TriggerCurrencyEffect(CurrencyKeys.Gold);
                    uiSystem.UpdateCoinsUi(progressionSystem.GetCurrency(CurrencyKeys.Gold));
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

                    dataSystem.RewardHandler.GrantReward(new HeroRewardModel(RewardType.Hero,
                        CharacterType.Storm));
                    dataSystem.CharacterManager.UnlockCharacter(CharacterType.Storm);
                    Debug.Log("Granting STORM");

                    HandleGameLoopFinish();
                    break;
                case GameState.Died:
                    uiSystem.UiEventHandler.ReviveMenu.Open();
                    break;
                case GameState.Ended:
                    break;
                case GameState.WaitingPortal:

                    Debug.Log(CurrentLvlIndex);
                    if (CurrentLvlIndex == 3)
                    {
                        dataSystem.CharacterManager.UnlockCharacter(CharacterType.Lancer);
                        Debug.Log("Granting LANCER");
                    }

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

            //Init world boss
            AudioManager.PlayMusic(container.CurrentModel.WorldBossMusicKey);
            uiSystem.UpdateEnemiesCounter(0);
            combatSystem.StartCharacterComboCheck();
            ChangeState(GameState.Ongoing);
            cameraController.CameraShaker.ShakeCamera(container.BossProfile);

            uiSystem.ShowSpecialEnemyWarning(EncounterType.Boss);
            uiSystem.UpdateSpecialEnemyHealthBar(1f);
            uiSystem.ToggleSpecialEnemyHealthBar(true);
            environmentSystem.ParticleManager.Spawn("BossSpawn", new Vector2(-4, 0));


            cameraController.InitLevelOverview(6f, () =>
            {
                cameraController.UpdateCharacterCameraFrustrum(1.5f, true);
                boss = npcSystem.NpcContainer.SpawnBoss(currentLevel);
                boss.OnBossStateChange += HandleBossStateChange;
                boss.OnBeingDamaged += HandleBossDamaged;

                boss.OnHealthPercentageChange += HandleBossHealthChange;
                boss.OnCrystalDestroyed += SpawnLootFromBoss;
                characterSystem.SetCharacterControllerState(true);
                boss.Init();
                foreach (var health in boss.CrystalNodes)
                {
                    combatSystem.RegisterEntity(new CombatEntityModel(health, null, CombatEntityType.Boss));
                }

                var spawnAbility = boss.transform.GetComponentInChildren<BossMushroomSummonAbility>();
                if (spawnAbility != null)
                {
                    spawnAbility.OnEnemySpawned += (healthController) =>
                    {
                        var attackController =
                            healthController.HealthTransform.GetComponent<IAttackControllerInterface>();
                        var effectsController =
                            healthController.HealthTransform.GetComponent<CombatEffectsController>();
                        combatSystem.RegisterEntity(new CombatEntityModel(healthController, attackController,
                            effectsController,
                            CombatEntityType.TempMob));
                    };
                }
            });
        }

        void HandleBossStateChange(BossState obj)
        {
            if (obj == BossState.Dead)
            {
                boss.OnBossStateChange -= HandleBossStateChange;
                boss.OnBeingDamaged -= HandleBossDamaged;
                boss.OnHealthPercentageChange -= HandleBossHealthChange;
                boss.OnCrystalDestroyed -= SpawnLootFromBoss;
                HandlePlayerWon();
            }
        }

        void HandleBossDamaged(HealthModificationIntentModel damageIntentModel)
        {
        }

        void HandleBossHealthChange(float amount)
        {
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
            if (shrine.GetAngelEffectManager.CompletedLevel())
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
            //if (characterStatController.GetHealthPercentage() <= 30)

            if (true)
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
            if (dataSystem.RewardHandler.RewardPending)
            {
                var pendingRewards = dataSystem.RewardHandler.GetPendingRewards();
                var rewardsToConsume = new List<RewardModel>();

                if (pendingRewards.TryGetValue(RewardType.Hero, out var rewards))
                {
                    foreach (var reward in rewards)
                    {
                        if (reward.RewardType == RewardType.Hero)
                        {
                            var heroReward = reward as HeroRewardModel;
                            rewardsToConsume.Add(reward);
                            uiSystem.UiEventHandler.SummaryMenu.AddRewardEntry(
                                $"Unlocked new Hero - {heroReward.HeroType}");
                        }
                    }
                }


                foreach (var reward in rewardsToConsume)
                {
                    dataSystem.RewardHandler.ConsumeReward(reward);
                }
            }

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
                shrine.OnShrineExit();
            }

            characterSystem.SetCharacterControllerState(false);

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
                        }
                        else
                        {
                            characterSystem.SetCharacterControllerState(true);
                        }
                    });
            });
        }

        void ContinueGameLoop()
        {
            CoroutineUtility.WaitForSeconds(0.5f, () =>
            {
                npcSystem.Reset();
                characterSystem.ResetCharacter(GetPlayerSpawnPosition);
                characterSystem.SetCharacterControllerState(false);
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

        private void UpdateStatModifiers()
        {
            var unlockedTraits = traitSystem.GetUnlockedEffects();
            var modifiedStatsMap = new Dictionary<StatAttributeType, int>();
            foreach (var trait in unlockedTraits)
            {
                if (trait.TargetTrait.Effect.TraitType == TraitType.StatBoost)
                {
                    var effect = trait.TargetTrait.Effect as StatBoostEffect;
                    var modificationValue = effect.Value + trait.Value.Value;
                    Debug.Log($"Gona update {effect.TargetStat} with value {effect.Value + trait.Value.Value}");
                    if (modifiedStatsMap.TryGetValue(effect.TargetStat, out var currentValue))
                    {
                        modifiedStatsMap[effect.TargetStat] += modificationValue;
                    }
                    else
                    {
                        modifiedStatsMap.Add(effect.TargetStat, modificationValue);
                    }
                }
            }

            //progressionSystem.StatManager.ProcessTraitsStatsModifiers(modifiedStatsMap);
        }
    }
}