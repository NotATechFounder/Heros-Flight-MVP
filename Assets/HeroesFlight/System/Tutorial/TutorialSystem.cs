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
using Pelumi.Juicer;
using Plugins.Audio_System;
using StansAssets.Foundation.Async;
using StansAssets.Foundation.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UISystem;
using System.ComponentModel;
using HeroesFlight.System.ShrineSystem;
using HeroesFlight.System.UI.Model;

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
    RewardSystemInterface rewardSystem;

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

    private Dictionary<GameButtonType, JuicerRuntime> tutorialButtonEffects = new Dictionary<GameButtonType,JuicerRuntime>();

    private ButtonCheck buttonCheck;

    public TutorialSystem(DataSystemInterface dataSystem, CharacterSystemInterface characterSystem, NpcSystemInterface npcSystem, EnvironmentSystemInterface environmentSystem, 
        CombatSystemInterface combatSystem, IUISystem uiSystem, 
        ProgressionSystemInterface progressionSystem, TraitSystemInterface traitSystem, InventorySystemInterface inventorySystem, RewardSystemInterface rewardSystemInterface)
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
        this.rewardSystem = rewardSystemInterface;
    }

    public void GameplayTutorialCompleted()
    {
        OnTutorialStateChanged?.Invoke(TutorialState.MainMenu);
    }

    public void MainMenuTutorialCompleted()
    {
        Debug.Log("MainMenuTutorialCompleted");
        OnFullTutorialCompleted?.Invoke();
        dataSystem.TutorialDataHolder.TutorialCompleted();
        dataSystem.TutorialDataHolder.GetTutorialHand.HideHand();
    }

    public void Init(Scene scene = default, Action OnComplete = null)
    {
        this.npcSystem.OnEnemySpawned += HandleEnemySpawned;
        this.combatSystem.OnEntityReceivedDamage += HandleEntityReceivedDamage;
        this.combatSystem.OnEntityDied += HandleEntityDied;
        this.uiSystem.OnSpecialButtonClicked += UseCharacterSpecial;
        this.uiSystem.OnReviveCharacterRequest +=ReviveCharacterWithFullHP;

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
       // uiSystem.UiEventHandler.ReviveMenu.OnCloseButtonClicked += HandleGameLoopFinish;
       // uiSystem.UiEventHandler.ReviveMenu.OnCountDownCompleted += HandleGameLoopFinish;

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

        goldModifier = 0;
        if (traitSystem.HasTraitOfType(TraitType.CurrencyBoost, out var traits))
        {
            foreach (var data in traits)
            {
                var traitValue = traitSystem.GetTraitEffect(data.TargetTrait.Id);
                goldModifier += traitValue.Value + data.Value.Value;
            }
        }

        inventorySystem.InventoryHandler.OnItemEquipped += InventoryHandler_OnItemEquipped;
        AdvanceButton.OnAnyButtonPointerDown += AnyButtonPointerDown;

        buttonCheck = new ButtonCheck();

        RegisterShrineNPCUIEvents();

        InitialseGameplayTutorialRuntimeStates();
        InitialseUITutorialRuntimeStates();

        StartGameSession();

        OnComplete?.Invoke();
    }

    private void UiSystem_OnPassiveAbilityButtonClicked(int obj)
    {
        activeAbilityManager.UseCharacterAbility(obj);
        //tutorialDictionary[TutorialMode.ActiveAbility].IsCompleted = true;
    }

    private void GameMenu_OnLevelUpComplete(int obj)
    {
        Debug.Log("GameMenu_OnLevelUpComplete");
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
            uiSystem.UiEventHandler.TutorialMenu.Open();
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
        this.npcSystem.OnEnemySpawned -= HandleEnemySpawned;
        this.combatSystem.OnEntityReceivedDamage -= HandleEntityReceivedDamage;
        this.combatSystem.OnEntityDied -= HandleEntityDied;
        this.uiSystem.OnSpecialButtonClicked -= UseCharacterSpecial;
        this.uiSystem.OnReviveCharacterRequest -= ReviveCharacterWithFullHP;

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
       // uiSystem.UiEventHandler.ReviveMenu.OnCloseButtonClicked -= HandleGameLoopFinish;
        //uiSystem.UiEventHandler.ReviveMenu.OnCountDownCompleted -= HandleGameLoopFinish;

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

        uiSystem.UiEventHandler.GameMenu.OnLevelUpComplete -= GameMenu_OnLevelUpComplete;

        UnRegisterShrineNPCUIEvents();

        inventorySystem.InventoryHandler.OnItemEquipped -= InventoryHandler_OnItemEquipped;
        AdvanceButton.OnAnyButtonPointerDown -= AnyButtonPointerDown;
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

        shrine.Initialize(dataSystem.CurrencyManager, characterStatController,dataSystem.AdManager);
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
                StartTutorialState(TutorialMode.Shrine);
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
        environmentSystem.CurrencySpawner.ActivateExpEffectItems(() =>
        {
            activeAbilityManager.AddExp(100);
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
                uiSystem.UpdateRuinShardUi(progressionSystem.GetCurrency(CurrencyKeys.Gold));
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

                //uiSystem.UiEventHandler.TutorialMenu.Close();

                OnTutorialStateChanged?.Invoke(TutorialState.MainMenu);

                StartTutorialState(TutorialMode.FirstRewardEquip);

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

    private void InventoryHandler_OnItemEquipped()
    {
      //  tutorialDictionary[TutorialMode.Equip].IsCompleted = true;
    }

    public void InitialseGameplayTutorialRuntimeStates()
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
            CoroutineUtility.WaitForSeconds(2, () =>
            {
                StartTutorialState(TutorialMode.ActiveAbility);
            });

        });

        tutorialDictionary.Add(TutorialMode.PasiveAbility, passiveAbilityTutorial);

        TutorialRuntime activeAbilityTutorial = new TutorialRuntime(TutorialMode.ActiveAbility);
        activeAbilityTutorial.AssignEvents(() =>
        {
            activeAbilityManager.EquippedAbility(ActiveAbilityType.LightningArrow);

            AdvanceButton advanceButton = uiSystem.UiEventHandler.GameMenu.GetButton(GameButtonType.GameMenu_Ability1);
            dataSystem.TutorialDataHolder.GetTutorialHand.ShowHand((RectTransform)advanceButton.transform);

            ButtonCheck buttonCheck = new ButtonCheck();
            buttonCheck.SetButton(advanceButton);
            WaitForButtonPressed waitForButtonPressed = new WaitForButtonPressed(buttonCheck);
            Juicer.WaitForCustomYieldInstruction(waitForButtonPressed, new JuicerCallBack(() =>
            {
                activeAbilityTutorial.IsCompleted = true;
                dataSystem.TutorialDataHolder.GetTutorialHand.HideHand();
            }));

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

            AdvanceButton advanceButton = uiSystem.UiEventHandler.GameMenu.GetButton(GameButtonType.GameMenu_SpecialAttack);
            dataSystem.TutorialDataHolder.GetTutorialHand.ShowHand((RectTransform)advanceButton.transform);

            ButtonCheck buttonCheck = new ButtonCheck();
            buttonCheck.SetButton(advanceButton);
            WaitForButtonPressed waitForButtonPressed = new WaitForButtonPressed(buttonCheck);
            Juicer.WaitForCustomYieldInstruction(waitForButtonPressed, new JuicerCallBack(() =>
            {
                UltimateAbilityTutorial.IsCompleted = true;
                dataSystem.TutorialDataHolder.GetTutorialHand.HideHand();
            }));

        }, () =>
        {
            CoroutineUtility.WaitForSeconds(2, () =>
            {
                ShowLevelPortal();
                uiSystem.UiEventHandler.TutorialMenu.DisplayMessage("Use Portal is transverse levels");
            });
        });

        tutorialDictionary.Add(TutorialMode.UltimateAbility, UltimateAbilityTutorial);

        TutorialRuntime shrineTutorial = new TutorialRuntime(TutorialMode.Shrine);
        shrineTutorial.AssignEvents(() =>
        {
            shrineTutorial.IsCompleted = true;
        }, () =>
        {
            Debug.Log("shrine Tutorial ended");
        });

        tutorialDictionary.Add(TutorialMode.Shrine, shrineTutorial);
    }

    public void InitialseUITutorialRuntimeStates()
    {
        TutorialRuntime equipItemTutorial = new TutorialRuntime(TutorialMode.FirstRewardEquip);
        equipItemTutorial.AssignEvents(() =>
        {
            CoroutineUtility.Start (EquipTutorialRoutine());
        }, () =>
        {
            StartTutorialState(TutorialMode.StatPoint);
        });

        tutorialDictionary.Add(TutorialMode.FirstRewardEquip, equipItemTutorial);

        TutorialRuntime unlockStatPointTutorial = new TutorialRuntime(TutorialMode.StatPoint);
        unlockStatPointTutorial.AssignEvents(() =>
        {
            CoroutineUtility.Start (StatPointTutorialRoutine());

        }, () =>
        {
            // MainMenuTutorialCompleted();
            StartTutorialState(TutorialMode.OpenShop);
        });

        tutorialDictionary.Add(TutorialMode.StatPoint, unlockStatPointTutorial);

        TutorialRuntime itemShopTutorial = new TutorialRuntime(TutorialMode.OpenShop);
        itemShopTutorial.AssignEvents(() =>
        {
            CoroutineUtility.Start(ShopTutorialRoutine());
        }, () =>
        {
            MainMenuTutorialCompleted();
        });

        tutorialDictionary.Add(TutorialMode.OpenShop, itemShopTutorial);
    }

    private void AnyButtonPointerDown(GameButtonType type)
    {
        if (tutorialButtonEffects.ContainsKey(type))
        {
            tutorialButtonEffects[type].Stop();
        }
    }

    public void StartTutorialState(TutorialMode tutorialMode)
    {
        DisplayTutorialStartUI(tutorialMode, () =>
        {
            currentTutorialRuntime = tutorialDictionary[tutorialMode];
            CoroutineUtility.Start(TutorialStateRoutine(currentTutorialRuntime));
        });
    }

    public void DisplayTutorialStartUI(TutorialMode tutorialMode, Action OnStartUIClosed)
    {
        TutorialSO tutorialSO = dataSystem.TutorialDataHolder.GetTutorialSO(tutorialMode);
        DisableMovement();
        uiSystem.UiEventHandler.TutorialMenu.SetTutorialDataToDisplay(tutorialSO.GetTutorialVisualData,()=>
        {
            EnableMovement();
            OnStartUIClosed?.Invoke();
        });
    }

    public IEnumerator TutorialStateRoutine(TutorialRuntime tutorialRuntime)
    {
        tutorialRuntime.OnBegin?.Invoke();
        while (!tutorialRuntime.IsCompleted)
        {
            yield return null;
        }
        tutorialRuntime.OnEnd?.Invoke();
    }

    public void StopTutorialButtonEffects()
    {
        foreach (var item in tutorialButtonEffects)
        {
            item.Value.Stop();
        }
    }

    public IEnumerator EquipTutorialRoutine()
    {
        // /First phase
        uiSystem.UiEventHandler.MainMenu.Open();

        inventorySystem.InventoryHandler.RemoveAllItems();
        rewardSystem.ProcessRewards(new List<Reward>() { tutorialHandler.FirstItemReward });
        uiSystem.UiEventHandler.RewardMenu.DisplayRewardsVisual(rewardSystem.GetRewardVisual(tutorialHandler.FirstItemReward));

        yield return WaitForGameButtonPressed(uiSystem.UiEventHandler.MainMenu, GameButtonType.Mainmenu_Inventory);

        // Second phase
        uiSystem.UiEventHandler.MainMenu.SetSingleButtonVibility(GameButtonType.Mainmenu_Inventory, GameButtonVisiblity.Hidden);

        uiSystem.UiEventHandler.TutorialMenu.NextVisualStep();

        uiSystem.UiEventHandler.InventoryMenu.SetAllButtonVibility(GameButtonVisiblity.Hidden);

        yield return WaitForButtonPressed(uiSystem.UiEventHandler.InventoryMenu.FirstItemUIButton);

        yield return WaitForGameButtonPressed(uiSystem.UiEventHandler.InventoryMenu, GameButtonType.Inventory_Equip);

        yield return new WaitForSeconds(2f);

        tutorialDictionary[TutorialMode.FirstRewardEquip].IsCompleted = true;
    }

    public IEnumerator StatPointTutorialRoutine()
    {
        uiSystem.UiEventHandler.MainMenu.SetAllButtonVibility(GameButtonVisiblity.Hidden);

        yield return WaitForGameButtonPressed(uiSystem.UiEventHandler.InventoryMenu, GameButtonType.Inventory_StatPoints);

        uiSystem.UiEventHandler.StatePointsMenu.SetAllButtonVibility(GameButtonVisiblity.Hidden);

        uiSystem.UiEventHandler.StatePointsMenu.ToggleAllStatButtonVisibility(StatPointButtonType.All, GameButtonVisiblity.Hidden);

        dataSystem.AccountLevelManager.SetXp(100);

        uiSystem.UiEventHandler.TutorialMenu.NextVisualStep();

        uiSystem.UiEventHandler.StatePointsMenu.ToggleFirstStatButtonVisibility(StatPointButtonType.Up, GameButtonVisiblity.Visible);
        yield return WaitForButtonPressed(uiSystem.UiEventHandler.StatePointsMenu.GetStatPointUIs[0].UpButton);

        uiSystem.UiEventHandler.TutorialMenu.NextVisualStep();

        uiSystem.UiEventHandler.StatePointsMenu.ToggleFirstStatButtonVisibility(StatPointButtonType.Down, GameButtonVisiblity.Visible);
        yield return WaitForButtonPressed(uiSystem.UiEventHandler.StatePointsMenu.GetStatPointUIs[0].DownButton);

        uiSystem.UiEventHandler.StatePointsMenu.ToggleFirstStatButtonVisibility(StatPointButtonType.Dice, GameButtonVisiblity.Visible);

        uiSystem.UiEventHandler.TutorialMenu.NextVisualStep();

        yield return WaitForButtonPressed(uiSystem.UiEventHandler.StatePointsMenu.GetStatPointUIs[0].DiceButton);

        yield return WaitForGameButtonPressed(uiSystem.UiEventHandler.DiceMenu, GameButtonType.DiceMenu_Info);

        yield return WaitForGameButtonPressed(uiSystem.UiEventHandler.DiceMenu, GameButtonType.DiceMenu_InfoClose);

        yield return WaitForGameButtonPressed(uiSystem.UiEventHandler.DiceMenu, GameButtonType.DiceMenu_Roll);

        yield return WaitForGameButtonPressed(uiSystem.UiEventHandler.DiceMenu, GameButtonType.DiceMenu_Close);

        yield return WaitForGameButtonPressed(uiSystem.UiEventHandler.StatePointsMenu, GameButtonType.StatPoints_Close);

        uiSystem.UiEventHandler.StatePointsMenu.ToggleAllStatButtonVisibility(StatPointButtonType.All, GameButtonVisiblity.Visible);

        uiSystem.UiEventHandler.InventoryMenu.Close();

        tutorialDictionary[TutorialMode.StatPoint].IsCompleted = true;
    }

    public IEnumerator ShopTutorialRoutine()
    {
        dataSystem.CurrencyManager.SetCurencyAmount(CurrencyKeys.Gem, 50);

        yield return WaitForGameButtonPressed(uiSystem.UiEventHandler.MainMenu, GameButtonType.Mainmenu_Shop);

        uiSystem.UiEventHandler.TutorialMenu.NextVisualStep();

        yield return WaitForGameButtonPressed(uiSystem.UiEventHandler.ShopMenu, GameButtonType.ShopMenu_RareChest);

        yield return WaitForGameButtonPressed(uiSystem.UiEventHandler.ConfirmationMenu, GameButtonType.ConfirmationMenu_Yes);

        uiSystem.UiEventHandler.TutorialMenu.DisplayMessage("Tutorial completed");

        yield return WaitForGameButtonPressed(uiSystem.UiEventHandler.MainMenu, GameButtonType.Mainmenu_World);

        uiSystem.UiEventHandler.TutorialMenu.Close();

        uiSystem.UiEventHandler.ConfirmationMenu.SetAllButtonVibility(GameButtonVisiblity.Visible);
        uiSystem.UiEventHandler.MainMenu.SetAllButtonVibility(GameButtonVisiblity.Visible);
        uiSystem.UiEventHandler.StatePointsMenu.SetAllButtonVibility(GameButtonVisiblity.Visible);
        uiSystem.UiEventHandler.ShopMenu.SetAllButtonVibility(GameButtonVisiblity.Visible);
        uiSystem.UiEventHandler.InventoryMenu.SetAllButtonVibility(GameButtonVisiblity.Visible);
        uiSystem.UiEventHandler.DiceMenu.SetAllButtonVibility(GameButtonVisiblity.Visible);

        tutorialDictionary[TutorialMode.OpenShop].IsCompleted = true;
    }

    public IEnumerator WaitForGameButtonPressed<T>(BaseMenu<T> theMenu, GameButtonType gameButtonType) where T : BaseMenu<T>
    {
        AdvanceButton advanceButton = theMenu.GetButton(gameButtonType);
        theMenu.SetButtonVibilityOnly(gameButtonType, GameButtonVisiblity.Visible);
        TriggerTutorialButtonEffects(theMenu, gameButtonType);

       yield return WaitForButtonPressed (advanceButton);
    }

    public IEnumerator WaitForButtonPressed (AdvanceButton advanceButton)
    {
        dataSystem.TutorialDataHolder.GetTutorialHand.ShowHand((RectTransform)advanceButton.transform);
        buttonCheck.SetButton(advanceButton);
        yield return new WaitUntil(() => buttonCheck.IsPressed);
    }

    public void TriggerTutorialButtonEffects<T>(BaseMenu<T> theMenu, GameButtonType gameButtonType) where T : BaseMenu<T>
    {
        AdvanceButton inventoryButton = theMenu.GetButton(gameButtonType);
        if (inventoryButton == null) return;
        JuicerRuntime juicerRuntime = inventoryButton.transform.JuicyScale(1.1f, 0.25f).SetLoop(-1).Start();
        tutorialButtonEffects.Add(gameButtonType, juicerRuntime);
    }
    
    private void ReviveCharacterWithFullHP(ReviveRequestModel reviveRequestModel)
    {
        ReviveCharacter(100f);
    }
}

public class ButtonCheck
{
    public bool IsPressed { get; private set; }
    private AdvanceButton advanceButton;

    public void SetButton(AdvanceButton advanceButton)
    {
        this.advanceButton = advanceButton;
        advanceButton.onClick.AddListener(OnPressed);
        IsPressed = false;
    }

    public void OnPressed()
    {
        IsPressed = true;
        advanceButton.onClick.RemoveListener(OnPressed);
    }   
}

public class WaitForButtonPressed : CustomYieldInstruction
{
    public override bool keepWaiting
    {
        get
        {
            return !buttonCheck.IsPressed;
        }
    }

    private ButtonCheck buttonCheck;

    public WaitForButtonPressed(ButtonCheck buttonCheck)
    {
        this.buttonCheck = buttonCheck;
    }
  
}