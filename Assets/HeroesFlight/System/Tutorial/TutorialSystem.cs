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
using HeroesFlight.System.Stats;
using HeroesFlight.System.Stats.Handlers;
using HeroesFlight.System.Stats.Traits.Enum;
using HeroesFlight.System.Tutorial;
using HeroesFlight.System.UI;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Enum;
using Pelumi.ObjectPool;
using Plugins.Audio_System;
using StansAssets.Foundation.Async;
using StansAssets.Foundation.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialSystem : ITutorialInterface
{
    public event Action OnFullTutorialCompleted;
    public event Action<TutorialState> OnTutorialStateChanged;

    private TutorialHandler tutorialHandler;

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
    InventorySystemInterface inventorySystem;
    IHealthController characterHealthController;

    BaseCharacterAttackController characterAttackController;

    CharacterStatController characterStatController;

    CharacterVFXController characterVFXController;

    CameraControllerInterface cameraController;

    HitEffectPlayerInterface hitEffectsPlayer;

    GameState currentState;

    int enemiesToKill;
    int CurrentLvlIndex => tutorialHandler.CurrentLvlIndex;

    public Vector2 GetPlayerSpawnPosition => currentLevelEnvironment
        .GetSpawnpoint(SpawnType.Player).GetSpawnPosition();

    float countDownDelay;

    LevelEnvironment currentLevelEnvironment;

    Level currentLevel;

    private bool revivedByFeatThisRun = false;
    private int goldModifier;

    private TutorialRuntime currentTutorialRuntime;
    private Dictionary<TutorialMode, TutorialRuntime> tutorialDictionary = new Dictionary<TutorialMode, TutorialRuntime>();

    public TutorialSystem(DataSystemInterface dataSystem, CharacterSystemInterface characterSystem, NpcSystemInterface npcSystem, EnvironmentSystemInterface environmentSystem, 
        CombatSystemInterface combatSystem, IUISystem uiSystem, ProgressionSystemInterface progressionSystem, TraitSystemInterface traitSystem, InventorySystemInterface inventorySystem)
    {
        this.dataSystem = dataSystem;
        this.characterSystem = characterSystem;
        this.npcSystem = npcSystem;
        this.environmentSystem = environmentSystem;
        this.combatSystem = combatSystem;
        this.uiSystem = uiSystem;
        this.progressionSystem = progressionSystem;
        this.traitSystem = traitSystem;
        this.inventorySystem = inventorySystem;
    }

    public void GameplayTutorialCompleted()
    {
        OnTutorialStateChanged?.Invoke(TutorialState.MainMenu);
    }

    public void MainMenuTutorialCompleted()
    {
        OnFullTutorialCompleted?.Invoke();
    }

    public void Init(Scene scene = default, Action OnComplete = null)
    {
        this.npcSystem.OnEnemySpawned += HandleEnemySpawned;
        this.combatSystem.OnEntityReceivedDamage += HandleEntityReceivedDamage;
        this.combatSystem.OnEntityDied += HandleEntityDied;
        this.uiSystem.OnSpecialButtonClicked += UseCharacterSpecial;
        this.uiSystem.OnReviveCharacterRequest += () => { ReviveCharacter(100f); };

        cameraController = scene.GetComponentInChildren<CameraControllerInterface>();

        shrine = scene.GetComponentInChildren<Shrine>();
        activeAbilityManager = scene.GetComponentInChildren<ActiveAbilityManager>();
        godsBenevolence = scene.GetComponentInChildren<GodsBenevolence>();

        tutorialHandler = scene.GetComponentInChildren<TutorialHandler>();
        hitEffectsPlayer = tutorialHandler.GetComponent<StackableSoundPlayer>();

        //progressionSystem.BoosterManager.OnBoosterActivated += HandleBoosterActivated;
        //progressionSystem.BoosterManager.OnBoosterContainerCreated += HandleBoosterWithDurationActivated;

        GameEffectController = scene.GetComponentInChildren<GameEffectController>();
    
        tutorialHandler.Init();
        tutorialHandler.OnPlayerEnteredPortal += HandlePlayerTriggerPortal;

        npcSystem.NpcContainer.SetMobDifficultyHolder(tutorialHandler.GetTutorialModel.MobDifficulty);

        environmentSystem.CurrencySpawner.OnCollected = HandleCurrencyCollected;

        environmentSystem.BoosterSpawner.ActivateBooster = progressionSystem.BoosterManager.ActivateBooster;

        shrine.GetAngelEffectManager.OnPermanetCard +=
            uiSystem.UiEventHandler.AngelPermanetCardMenu.AcivateCardPermanetEffect;
        uiSystem.UiEventHandler.AngelGambitMenu.CardExit += shrine.GetAngelEffectManager.Exists;
        uiSystem.UiEventHandler.AngelGambitMenu.OnCardSelected += shrine.GetAngelEffectManager.AddAngelCardSO;
        uiSystem.UiEventHandler.AngelGambitMenu.OnMenuClosed += EnableMovement;

        uiSystem.UiEventHandler.GodsBenevolencePuzzleMenu.GetRandomBenevolenceVisualSO = godsBenevolence.GetRandomGodsBenevolenceVisualSO;
        uiSystem.UiEventHandler.GodsBenevolencePuzzleMenu.OnPuzzleSolved += godsBenevolence.ActivateGodsBenevolence;

        //uiSystem.UiEventHandler.SummaryMenu.OnMenuOpened += StoreRunReward;
        uiSystem.UiEventHandler.GameMenu.OnSingleLevelUpComplete += HandleSingleLevelUp;
        uiSystem.UiEventHandler.GameMenu.GetPassiveAbilityLevel += activeAbilityManager.GetPassiveAbilityLevel;
      //  uiSystem.OnRestartLvlRequest += HandleLvlRestart;
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
        uiSystem.OnPassiveAbilityButtonClicked += UiSystem_OnPassiveAbilityButtonClicked;

        activeAbilityManager.OnActiveAbilityEquipped += uiSystem.UiEventHandler.GameMenu.ActiveAbilityEqquiped;
        activeAbilityManager.OnPassiveAbilityEquipped += uiSystem.UiEventHandler.GameMenu.VisualisePassiveAbility;
        activeAbilityManager.OnPassiveAbilityRemoved += uiSystem.UiEventHandler.GameMenu.RemovePassiveAbility;
        activeAbilityManager.OnRegularActiveAbilitySwapped += uiSystem.UiEventHandler.GameMenu.SwapActiveAbility;
        activeAbilityManager.OnRegularActiveAbilityUpgraded += uiSystem.UiEventHandler.GameMenu.UpgradeActiveAbility;

        uiSystem.UiEventHandler.AbilitySelectMenu.OnRegularAbilitySelected += activeAbilityManager.EquippedAbility;
        uiSystem.UiEventHandler.AbilitySelectMenu.OnPassiveAbilitySelected += activeAbilityManager.AddPassiveAbility;
        uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomActiveAbility += activeAbilityManager.GetRandomActiveAbility;
        uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomActiveAbilityVisualData += activeAbilityManager.GetActiveAbilityVisualData;
        uiSystem.UiEventHandler.AbilitySelectMenu.GetActiveAbilityLevel += activeAbilityManager.GetActiveAbilityLevel;
        uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomPassiveAbility += activeAbilityManager.GetRandomPassiveAbility;
        uiSystem.UiEventHandler.AbilitySelectMenu.GetRandomPassiveAbilityVisualData += activeAbilityManager.GetPassiveAbilityVisualData;
        uiSystem.UiEventHandler.AbilitySelectMenu.GetPassiveAbilityLevel += activeAbilityManager.GetPassiveAbilityLevel;
        uiSystem.UiEventHandler.AbilitySelectMenu.OnMenuClosed += HeroProgressionCompleted;

        uiSystem.UiEventHandler.GameMenu.OnLevelUpComplete += GameMenu_OnLevelUpComplete;

        uiSystem.UiEventHandler.TutorialMenu.OnShowClicked += DisableMovement;
        uiSystem.UiEventHandler.TutorialMenu.OnHideClicked += EnableMovement;

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

        InitialseTutorialRuntimeStates();

        StartGameSession();

        OnComplete?.Invoke();
    }

    private void UiSystem_OnPassiveAbilityButtonClicked(int obj)
    {
        activeAbilityManager.UseCharacterAbility(obj);
        tutorialDictionary[TutorialMode.ActiveAbility].IsCompleted = true;
    }

    private void GameMenu_OnLevelUpComplete(int obj)
    {
        StartTutorialState(TutorialMode.PasiveAbility);

       // uiSystem.UiEventHandler.AbilitySelectMenu.Open();
    }

    public void StartGameSession()
    {
        uiSystem.UiEventHandler.GameMenu.Open();

        uiSystem.UiEventHandler.GameMenu.ShowTransition(() => // level transition
        {
            uiSystem.UiEventHandler.LoadingMenu.Close();
            PreloadLvl();
            SetupCharacter();
        } ,
        ()=>
        {
            StartTutorialState(TutorialMode.Fly);
        });
    }

    public void Reset()
    {
        ResetLogic();
        ResetConnections();
        tutorialHandler.SetStartingIndex(0);
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
            default: break;
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
            default: break;
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

        uiSystem.UiEventHandler.GodsBenevolencePuzzleMenu.OnPuzzleSolved -= godsBenevolence.ActivateGodsBenevolence;
        //uiSystem.UiEventHandler.SummaryMenu.OnMenuOpened -= StoreRunReward;
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
        activeAbilityManager.OnRegularActiveAbilityUpgraded -= uiSystem.UiEventHandler.GameMenu.UpgradeActiveAbility;

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

        enemiesToKill = 0;
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
    /// Used to disable character controller movement
    /// </summary>
    void DisableMovement()
    {
        characterSystem.SetCharacterControllerState(false);
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

        tutorialDictionary[TutorialMode.UltimateAbility].IsCompleted = true;
    }

    void ReviveCharacter(float healthPercentage)
    {
        environmentSystem.ParticleManager.Spawn("CharacterRevival", characterSystem.CurrentCharacter.CharacterTransform.position);

        combatSystem.RevivePlayer(healthPercentage);

        ChangeState(GameState.Ongoing);
    }

    /// <summary>
    /// Used to load next lvl model
    /// </summary>
    /// <param name="currentLevel"> regerenced model</param>
    /// <returns>true if next lvl model exists</returns>
    bool CheckLevel(ref Level currentLevel)
    {
        currentLevel = tutorialHandler.GetLevel();
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

        CombatEffectsController effectsController = characterController.CharacterTransform.GetComponent<CombatEffectsController>();
        combatSystem.RegisterEntity(new CombatEntityModel(characterHealthController, characterAttackController,
            effectsController,
            CombatEntityType.Player));
        combatSystem.InitCharacterUltimate(characterController.CharacterSO.CharacterAnimations.UltAnimationsData,
            characterController.CharacterSO.UltimateData.Charges);

        effectsController.AddCombatEffect(inventorySystem.GetEquippedItemsCombatEffects());

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
        GameEffectController.StopTime(0.1f, tutorialHandler.GetTutorialModel.TimeStopRestoreSpeed,
            tutorialHandler.GetTutorialModel.TimeStopDuration);
    }

    void HandleEnemySpawned(AiControllerBase obj)
    {
        var healthController = obj.GetComponent<IHealthController>();

        obj.OnDisabled += HandleEnemyDisabled;
        var attackController = obj.GetComponent<IAttackControllerInterface>();
        var effectsController = obj.GetComponent<CombatEffectsController>();
        combatSystem.RegisterEntity(new CombatEntityModel(healthController, attackController, effectsController,
            CombatEntityType.Mob));
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

        environmentSystem.CurrencySpawner.SpawnAtPosition(CurrencyKeys.Gold, 10 + goldModifier, position);

        environmentSystem.CurrencySpawner.SpawnAtPosition(CurrencyKeys.RunExperience, 0, position);

        uiSystem.UpdateEnemiesCounter(enemiesToKill);

        if (currentState != GameState.Ongoing) return;

        if (enemiesToKill <= 0)
        {
            Debug.Log("Player Won");
            HandleAllEnemiesKilled();
        }
    }

    void HandleAllEnemiesKilled()
    {
        GameEffectController.ForceStop(() =>
        {
           // dataSystem.WorldManger.SetMaxLevelReached(dataSystem.WorldManger.SelectedWorld, tutorialHandler.CurrentLvlIndex);
            //if (tutorialHandler.FinishedLoop)
            //{
            //    characterAttackController.ToggleControllerState(false);
            //    CoroutineUtility.WaitForSeconds(6f, () => { ChangeState(GameState.Won); });
            //    return;
            //}
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
        ReviveCharacter(100);
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
        MoveNextLvl();
    }

    public void StartGameLoop()
    {
        switch (currentLevel.LevelType)
        {
            case LevelType.NormalCombat:

                enemiesToKill = currentLevel.MiniHasBoss
                    ? currentLevel.TotalMobsToSpawn + 1
                    : currentLevel.TotalMobsToSpawn;
                uiSystem.UpdateEnemiesCounter(enemiesToKill);
                combatSystem.StartCharacterComboCheck();

                SpawnEnemies(currentLevel);

                ChangeState(GameState.Ongoing);

                EnableMovement();

                break;
            case LevelType.Shrine:
                EnableMovement();
                break;
            default: break;
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

        cameraController.SetConfiner(currentLevelEnvironment.BoundsCollider);
        npcSystem.NpcContainer.SetSpawnPoints(currentLevelEnvironment.SpawnPointsCache);
        switch (currentLevel.LevelType)
        {
            case LevelType.NormalCombat:
                tutorialHandler.DisablePortal();
                break;
            case LevelType.WorldBoss:
                tutorialHandler.DisablePortal();
                break;

            case LevelType.Shrine:

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

                tutorialHandler.EnablePortal(currentLevelEnvironment.GetSpawnpoint(SpawnType.Portal).GetSpawnPosition());
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
        uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetActiveAbilityLevel += activeAbilityManager.GetActiveAbilityLevel;
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
        uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetPassiveAbilityLevel += activeAbilityManager.GetPassiveAbilityLevel;
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
        uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetEqquipedActiveAbilityTypes -= activeAbilityManager.GetEqqipedActiveAbilities;
        uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetRandomActiveAbilityTypes -= activeAbilityManager.GetRandomActiveAbilityFromAll;
        uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetActiveAbilityVisualData -= activeAbilityManager.GetActiveAbilityVisualData;
        uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.GetActiveAbilityLevel -= activeAbilityManager.GetActiveAbilityLevel;
        uiSystem.UiEventHandler.ActiveAbilityRerollerNPCMenu.OnActiveAbilitySwapped -= activeAbilityManager.SwapActiveAbility;

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
        uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.GetPassiveAbilityLevel -= activeAbilityManager.GetPassiveAbilityLevel;
        uiSystem.UiEventHandler.PassiveAbilityRerollerNPCMenu.OnPassiveAbilitySwapped -= activeAbilityManager.SwapPassiveAbility;
    }

    void HandleHeroProgression()
    {
        godsBenevolence.DeactivateGodsBenevolence();
        environmentSystem.CurrencySpawner.ActivateExpItems(() =>
        {
            progressionSystem.AddCurrency(CurrencyKeys.RunExperience, (int)currentLevel.ExpReward);
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

    void AddCurrency(string key, int amount)
    {
        progressionSystem.AddCurrency(key, amount);
    }

    void HandleGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Won:
                OnTutorialStateChanged?.Invoke(TutorialState.MainMenu);
                HandleGameLoopFinish();
                break;
            case GameState.Died:
                uiSystem.UiEventHandler.ReviveMenu.Open();
                break;
            case GameState.WaitingPortal:
                CoroutineUtility.Start(WaitingPortalRoutine());
                break;
            default:   break;
        }
    }

    IEnumerator WaitingPortalRoutine()
    {
        uiSystem.UiEventHandler.GameMenu.DisplayInfoMessage(UISystem.GameMenu.InfoMessageType.Complete);

        yield return new WaitForSeconds(2f);

        uiSystem.UiEventHandler.GameMenu.OnUpdateXpBarCompleted += ContinueWaitForPortalRoutine;
        HandleHeroProgression();
    }

    void ContinueWaitForPortalRoutine()
    {
        uiSystem.UiEventHandler.GameMenu.OnUpdateXpBarCompleted -= ContinueWaitForPortalRoutine;
        CoroutineUtility.Start(ContinueAfterXpBarUpdate());
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

        //ShowLevelPortal();
    }

    void ShowLevelPortal()
    {
        CoroutineUtility.WaitForSeconds(1f,
            () =>
            {
                tutorialHandler.EnablePortal(currentLevelEnvironment.GetSpawnpoint(SpawnType.Portal).GetSpawnPosition());
            });
    }

    void HandleGameLoopFinish()
    {
        //uiSystem.UiEventHandler.SummaryMenu.Open();
        // TODO: Add exp to player
        dataSystem.AccountLevelManager.AddExp(100);
    }

    void MoveNextLvl()
    {
        characterSystem.SetCharacterControllerState(false);

        if (currentLevel.LevelType == LevelType.Shrine)
        {
            uiSystem.UiEventHandler.GameMenu.ShowTransition(() => // level to level transition
            {
                ResetLogic();
                ChangeState(GameState.Won);
            },
            () =>
            {

            });
            return;
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
                ContinueGameLoop();
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

    public void InitialseTutorialRuntimeStates()
    {
        TutorialRuntime flyTutorial = new TutorialRuntime(TutorialMode.Fly);
        flyTutorial.AssignEvents(() =>
        {
            tutorialHandler.GetTutorialTrigger.Activate(new Vector2(-6.0999999f, -0.300000012f), () =>
            {
                flyTutorial.IsCompleted = true;
            });
        }, () =>
        {
            Debug.Log("fly Tutorial ended");
            StartTutorialState (TutorialMode.AutoAttack);
        });

        tutorialDictionary.Add(TutorialMode.Fly, flyTutorial);

        TutorialRuntime autoAttackTutorial = new TutorialRuntime(TutorialMode.AutoAttack);
        autoAttackTutorial.AssignEvents(() =>
        {
            StartGameLoop();
            autoAttackTutorial.IsCompleted = true;
        }, () =>
        {
            Debug.Log("auto Attack Tutorial ended");
            // StartTutorialState (TutorialMode.AutoAttack);
        });

        tutorialDictionary.Add(TutorialMode.AutoAttack, autoAttackTutorial);

        TutorialRuntime passiveAbilityTutorial = new TutorialRuntime(TutorialMode.PasiveAbility);
        passiveAbilityTutorial.AssignEvents(() =>
        {
            activeAbilityManager.AddPassiveAbility(PassiveAbilityType.FrostStrike);

            CoroutineUtility.WaitForSeconds(2,()=> 
            {
                passiveAbilityTutorial.IsCompleted = true;
            });
        }, () =>
        {
             StartTutorialState (TutorialMode.ActiveAbility);
        });

        tutorialDictionary.Add(TutorialMode.PasiveAbility, passiveAbilityTutorial);

        TutorialRuntime activeAbilityTutorial = new TutorialRuntime(TutorialMode.ActiveAbility);
        activeAbilityTutorial.AssignEvents(() =>
        {
            activeAbilityManager.EquippedAbility(ActiveAbilityType.LightningArrow);
        }, () =>
        {
            CoroutineUtility.WaitForSeconds(2, () =>
            {
                StartTutorialState(TutorialMode.UltimateAbility);
            });   
        });

        tutorialDictionary.Add(TutorialMode.ActiveAbility, activeAbilityTutorial);

        TutorialRuntime UltimateAbilityTutorial = new TutorialRuntime(TutorialMode.UltimateAbility);
        UltimateAbilityTutorial.AssignEvents(() =>
        {
            combatSystem.SetSpecialBarValue(1);
        }, () =>
        {
            CoroutineUtility.WaitForSeconds(2, () =>
            {
                ShowLevelPortal();
            });
        });

        tutorialDictionary.Add(TutorialMode.UltimateAbility, UltimateAbilityTutorial);
    }

    public void StartTutorialState(TutorialMode tutorialMode)
    {
        DisplayTutorialStartUI(tutorialMode, () =>
        {
            currentTutorialRuntime = tutorialDictionary[tutorialMode];
            tutorialHandler.StartTutorialState(currentTutorialRuntime);
        });
    }

    public void DisplayTutorialStartUI(TutorialMode tutorialMode, Action OnStartUIClosed)
    {
        TutorialSO tutorialSO = tutorialHandler.GetTutorialSO(tutorialMode);
        DisableMovement();
        uiSystem.UiEventHandler.TutorialMenu.Display(tutorialSO.GetTutorialVisualData, ()=>
        {
            EnableMovement();
            OnStartUIClosed?.Invoke();
        });
    }
}
