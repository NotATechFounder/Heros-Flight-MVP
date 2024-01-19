using HeroesFlight.StateStack.State;
using HeroesFlight.StateStack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeroesFlight.Core.StateStack.Enum;
using StansAssets.Foundation.Patterns;
using JetBrains.Annotations;
using StansAssets.SceneManagement;
using HeroesFlight.System.Tutorial;
using System;
using HeroesFlight.System.Dice;
using HeroesFlight.System.Stats.Handlers;
using HeroesFlight.System.UI;
using HeroesFlight.Common.Enum;
using HeroesFlight.Common.Progression;
using HeroesFlight.System.Character;
using HeroesFlight.System.Combat;
using HeroesFlight.System.Gameplay;
using HeroesFlight.System.Input;
using HeroesFlight.System.NPC;
using HeroesFlight.System.Stats;
using UnityEngine.SceneManagement;

namespace HeroesFlight.StateStack.State
{
    [UsedImplicitly]
    public class TutorialState : BaseApplicationLoadSceneState, IAppState
    {
        public ApplicationState ApplicationState => ApplicationState.Tutorial;

        public void Init(ServiceLocator serviceLocator)
        {
            InitLocator(serviceLocator);
        }

        public override void ChangeState(StackChangeEvent<ApplicationState> evt, IProgressReporter progressReporter)
        {
            switch (evt.Action)
            {
                case StackAction.Added:
                    progressReporter.SetDone();

                    ITutorialInterface tutorialInterface = GetService<ITutorialInterface>();
                    tutorialInterface.OnTutorialStateChanged += TutorialInterface_OnTutorialStart;
                    tutorialInterface.OnFullTutorialCompleted += HandleTutorialComplete;

                    StartGameplayTutorial();

                    break;
                case StackAction.Paused:
                    break;
                case StackAction.Resumed:
                    break;
                case StackAction.Removed:
                    progressReporter.SetDone();
                    break;
                default:     break;
            }
        }

        private void TutorialInterface_OnTutorialStart(System.Tutorial.TutorialState obj)
        {
            switch (obj)
            {
                case System.Tutorial.TutorialState.Gameplay:

                    break;
                case System.Tutorial.TutorialState.MainMenu:
                    StartMainMenuTutorial();
                    break;
                default: break;
            }
        }

        private void StartGameplayTutorial()
        {
            var uiSystem = GetService<IUISystem>();
            var dataSystem = GetService<DataSystemInterface>();
            var traitSystem = GetService<TraitSystemInterface>();
            var diceSystem = GetService<DiceSystemInterface>();
            var characterSystem = GetService<CharacterSystemInterface>();
            var npcSystem = GetService<NpcSystemInterface>();
            var combatSystem = GetService<CombatSystemInterface>();
            var progressionSystem = GetService<ProgressionSystemInterface>();
            var inputSystem = GetService<InputSystemInterface>();

            var tutorialScene = $"{SceneType.TutorialScene}";
            m_SceneActionsQueue.AddAction(SceneActionType.Load, tutorialScene);
            m_SceneActionsQueue.Start(uiSystem.UiEventHandler.LoadingMenu.UpdateLoadingBar, () =>
            {
                uiSystem.UiEventHandler.GameMenu.Open();
                var loadedScene = m_SceneActionsQueue.GetLoadedScene(tutorialScene);
                SceneManager.SetActiveScene(loadedScene);
                characterSystem.Init(loadedScene);
                progressionSystem.Init(loadedScene);
                characterSystem.SetCurrentCharacterType(dataSystem.CharacterManager.SelectedCharacter
                    .CharacterType);
                npcSystem.Init(loadedScene);
                inputSystem.Init(loadedScene);
                combatSystem.Init(loadedScene);

                ITutorialInterface tutorialInterface = GetService<ITutorialInterface>();
                tutorialInterface.Init(loadedScene);
            });
        }

        void HandleReturnToMainMenu()
        {

        }

        private void StartMainMenuTutorial()
        {
            var uiSystem = GetService<IUISystem>();
            var dataSystem = GetService<DataSystemInterface>();
            var traitSystem = GetService<TraitSystemInterface>();
            var diceSystem = GetService<DiceSystemInterface>();
            var progressionSystem = GetService<ProgressionSystemInterface>();
            var combatSystem = GetService<CombatSystemInterface>();

            var tutorialScene = $"{SceneType.TutorialScene}";
            m_SceneActionsQueue.AddAction(SceneActionType.Unload, tutorialScene);

            progressionSystem.Reset();
            combatSystem.Reset();
            uiSystem.UiEventHandler.StatePointsMenu.ResetMenu();

            m_SceneActionsQueue.Start(uiSystem.UiEventHandler.LoadingMenu.UpdateLoadingBar, () =>
            {
                //uiSystem.UiEventHandler.MainMenu.Open();
                uiSystem.UiEventHandler.GameMenu.Close();
                uiSystem.UiEventHandler.LoadingMenu.Close();

                SubscribeMainMenuEvents();

                traitSystem.Init();
                dataSystem.CurrencyManager.TriggerAllCurrencyChange();
                dataSystem.StatManager.ProcessTraitsStatsModifiers(traitSystem.GetUnlockedEffects());
                uiSystem.UiEventHandler.MainMenu.LoadWorlds(dataSystem.WorldManger.Worlds);
                uiSystem.UiEventHandler.MainMenu.AccountLevelUp(dataSystem.AccountLevelManager.GetExpIncreaseResponse());

                uiSystem.UiEventHandler.LoadingMenu.Close();
                uiSystem.UiEventHandler.GameMenu.Close();
            });
        }

        private void HandleTutorialComplete()
        {
            ITutorialInterface tutorialInterface = GetService<ITutorialInterface>();
            tutorialInterface.Reset();
            UnSubscribeMainMenuEvents();
            AppStateStack.State.Set(ApplicationState.MainMenu);
            GetService<RewardSystemInterface>().SetCurrentState(GameStateType.MainMenu);
        }

        public void SubscribeMainMenuEvents()
        {
            var uiSystem = GetService<IUISystem>();
            var dataSystem = GetService<DataSystemInterface>();
            var traitSystem = GetService<TraitSystemInterface>();
            var diceSystem = GetService<DiceSystemInterface>();
            traitSystem.OnTraitsStateChange += HandleTraitStateChange;

            uiSystem.UiEventHandler.MainMenu.AddGem += () => dataSystem.CurrencyManager.AddCurrency(CurrencyKeys.Gem, 10000);
            uiSystem.UiEventHandler.MainMenu.AddGold += () => dataSystem.CurrencyManager.AddCurrency(CurrencyKeys.Gold, 10000);

            uiSystem.UiEventHandler.InventoryMenu.OnChangeHeroButtonClicked += uiSystem.UiEventHandler.CharacterSelectMenu.Open;
            uiSystem.UiEventHandler.InventoryMenu.OnStatPointButtonClicked += uiSystem.UiEventHandler.StatePointsMenu.Open;
            uiSystem.UiEventHandler.InventoryMenu.GetStatModel += dataSystem.StatManager.GetStatModel;

            uiSystem.UiEventHandler.CharacterSelectMenu.GetAllCharacterSO += dataSystem.CharacterManager.GetAllCharacterSO;
            uiSystem.UiEventHandler.CharacterSelectMenu.OnTryBuyCharacter += dataSystem.CharacterManager.TryBuyCharacter;

            uiSystem.UiEventHandler.InventoryMenu.GetSelectedCharacterSO += dataSystem.CharacterManager.GetSelectedCharacter;
            uiSystem.UiEventHandler.CharacterSelectMenu.OnCharacterSelected += dataSystem.CharacterManager.ToggleCharacterSelected;

            uiSystem.UiEventHandler.StatePointsMenu.GetCurrentSpLevel += dataSystem.StatPoints.GetSp;
            uiSystem.UiEventHandler.StatePointsMenu.OnAddSpClicked += dataSystem.StatPoints.TryAddSp;
            uiSystem.UiEventHandler.StatePointsMenu.OnRemoveSpClicked += dataSystem.StatPoints.TrytRemoveSp;
            uiSystem.UiEventHandler.StatePointsMenu.GetAvailabletSp += dataSystem.StatPoints.GetAvailableSp;
            uiSystem.UiEventHandler.StatePointsMenu.OnCompletePressed += dataSystem.StatPoints.Confirm;
            uiSystem.UiEventHandler.StatePointsMenu.GetDiceRollValue += dataSystem.StatPoints.GetDiceRollValue;
            uiSystem.UiEventHandler.StatePointsMenu.OnResetButtonPressed += dataSystem.StatPoints.ResetSp;

            uiSystem.UiEventHandler.StatePointsMenu.OnDiceClicked += OnDiceClicked;

            dataSystem.CharacterManager.OnCharacterChanged += uiSystem.UiEventHandler.InventoryMenu.UpdateCharacter;
            dataSystem.StatManager.OnValueChanged += uiSystem.UiEventHandler.InventoryMenu.OnStatValueChanged;

            uiSystem.UiEventHandler.MainMenu.OnPlayButtonPressed += HandleGameStartRequest;
            dataSystem.CurrencyManager.OnCurrencyChanged += uiSystem.UiEventHandler.MainMenu.CurrencyChanged;


            uiSystem.UiEventHandler.MainMenu.IsWorldUnlocked += dataSystem.WorldManger.IsWorldUnlocked;
            uiSystem.UiEventHandler.MainMenu.OnWorldChanged += dataSystem.WorldManger.SetSelectedWorld;
            uiSystem.UiEventHandler.MainMenu.GetMaxLevelReached += dataSystem.WorldManger.GetMaxLevelReached;

            dataSystem.EnergyManager.OnEnergyTimerUpdated += uiSystem.UiEventHandler.MainMenu.UpdateEnergyTime;

            dataSystem.AccountLevelManager.OnLevelUp += uiSystem.UiEventHandler.MainMenu.AccountLevelUp;
            dataSystem.AccountLevelManager.OnLevelUp += uiSystem.UiEventHandler.LevelUpMenu.AccountLevelUp;
            //uiSystem.UiEventHandler.MainMenu.GetCurrentAccountLevelXP += dataSystem.AccountLevelManager.GetExpIncreaseResponse;
        }
        public void UnSubscribeMainMenuEvents()
        {
            var uiSystem = GetService<IUISystem>();
            var dataSystem = GetService<DataSystemInterface>();
            var traitSystem = GetService<TraitSystemInterface>();
            var diceSystem = GetService<DiceSystemInterface>();

            traitSystem.OnTraitsStateChange -= HandleTraitStateChange;

            uiSystem.UiEventHandler.MainMenu.AddGem -= () => dataSystem.CurrencyManager.AddCurrency(CurrencyKeys.Gem, 10000);
            uiSystem.UiEventHandler.MainMenu.AddGold -= () => dataSystem.CurrencyManager.AddCurrency(CurrencyKeys.Gold, 10000);

            uiSystem.UiEventHandler.InventoryMenu.OnChangeHeroButtonClicked -= uiSystem.UiEventHandler.CharacterSelectMenu.Open;
            uiSystem.UiEventHandler.InventoryMenu.OnStatPointButtonClicked -= uiSystem.UiEventHandler.StatePointsMenu.Open;
            uiSystem.UiEventHandler.InventoryMenu.GetStatModel -= dataSystem.StatManager.GetStatModel;

            uiSystem.UiEventHandler.CharacterSelectMenu.GetAllCharacterSO -= dataSystem.CharacterManager.GetAllCharacterSO;
            uiSystem.UiEventHandler.CharacterSelectMenu.OnTryBuyCharacter -= dataSystem.CharacterManager.TryBuyCharacter;

            uiSystem.UiEventHandler.InventoryMenu.GetSelectedCharacterSO -= dataSystem.CharacterManager.GetSelectedCharacter;
            uiSystem.UiEventHandler.CharacterSelectMenu.OnCharacterSelected -= dataSystem.CharacterManager.ToggleCharacterSelected;

            uiSystem.UiEventHandler.StatePointsMenu.GetCurrentSpLevel -= dataSystem.StatPoints.GetSp;
            uiSystem.UiEventHandler.StatePointsMenu.OnAddSpClicked -= dataSystem.StatPoints.TryAddSp;
            uiSystem.UiEventHandler.StatePointsMenu.OnRemoveSpClicked -= dataSystem.StatPoints.TrytRemoveSp;
            uiSystem.UiEventHandler.StatePointsMenu.GetAvailabletSp -= dataSystem.StatPoints.GetAvailableSp;
            uiSystem.UiEventHandler.StatePointsMenu.OnCompletePressed -= dataSystem.StatPoints.Confirm;
            uiSystem.UiEventHandler.StatePointsMenu.GetDiceRollValue -= dataSystem.StatPoints.GetDiceRollValue;
            uiSystem.UiEventHandler.StatePointsMenu.OnResetButtonPressed -= dataSystem.StatPoints.ResetSp;

            uiSystem.UiEventHandler.StatePointsMenu.OnDiceClicked -= OnDiceClicked;

            dataSystem.CharacterManager.OnCharacterChanged -= uiSystem.UiEventHandler.InventoryMenu.UpdateCharacter;
            dataSystem.StatManager.OnValueChanged -= uiSystem.UiEventHandler.InventoryMenu.OnStatValueChanged;

            uiSystem.UiEventHandler.MainMenu.OnPlayButtonPressed -= HandleGameStartRequest;
            dataSystem.CurrencyManager.OnCurrencyChanged -= uiSystem.UiEventHandler.MainMenu.CurrencyChanged;


            uiSystem.UiEventHandler.MainMenu.IsWorldUnlocked -= dataSystem.WorldManger.IsWorldUnlocked;
            uiSystem.UiEventHandler.MainMenu.OnWorldChanged -= dataSystem.WorldManger.SetSelectedWorld;
            uiSystem.UiEventHandler.MainMenu.GetMaxLevelReached -= dataSystem.WorldManger.GetMaxLevelReached;

            dataSystem.EnergyManager.OnEnergyTimerUpdated -= uiSystem.UiEventHandler.MainMenu.UpdateEnergyTime;

            uiSystem.UiEventHandler.MainMenu.OnPlayButtonPressed -= HandleGameStartRequest;
            traitSystem.OnTraitsStateChange -= HandleTraitStateChange;

            dataSystem.AccountLevelManager.OnLevelUp -= uiSystem.UiEventHandler.MainMenu.AccountLevelUp;
            dataSystem.AccountLevelManager.OnLevelUp += uiSystem.UiEventHandler.LevelUpMenu.AccountLevelUp;
            //uiSystem.UiEventHandler.MainMenu.GetCurrentAccountLevelXP -= dataSystem.AccountLevelManager.GetExpIncreaseResponse;
        }

        void HandleGameStartRequest()
        {
            var dataSystem = GetService<DataSystemInterface>();
            if (dataSystem.EnergyManager.UseEnergy(5))
            {
                UnSubscribeMainMenuEvents();
                GetService<RewardSystemInterface>().SetCurrentState(GameStateType.Gameplay);
                AppStateStack.State.Set(ApplicationState.Gameplay);
            }
        }

        void HandleTraitStateChange(Dictionary<StatAttributeType, int> modifiers)
        {
            var dataSystem = GetService<DataSystemInterface>();
            dataSystem.StatManager.ProcessTraitsStatsModifiers(modifiers);
        }

        void OnDiceClicked(StatAttributeType statAttributeType, int value)
        {
            var uiSystem = GetService<IUISystem>();
            var dataSystem = GetService<DataSystemInterface>();
            var diceSystem = GetService<DiceSystemInterface>();
            uiSystem.UiEventHandler.DiceMenu.ShowDiceMenu(value,dataSystem.CurrencyManager.GetCurrencyAmount(CurrencyKeys.Gem)>= diceSystem.DiceCost, () =>
            {
                diceSystem.RollDice((rolledValue) =>
                {
                    dataSystem.StatPoints.SetDiceRollValue(statAttributeType, rolledValue);
                    uiSystem.UiEventHandler.StatePointsMenu.OnNewDiceRoll(statAttributeType, rolledValue);
                });
            });
        }
    }
}
