using System;
using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using HeroesFlight.Common.Progression;
using HeroesFlight.Core.StateStack.Enum;
using HeroesFlight.System.Character;
using HeroesFlight.System.Dice;
using HeroesFlight.System.Stats;
using HeroesFlight.System.Stats.Handlers;
using HeroesFlight.System.UI;
using JetBrains.Annotations;
using StansAssets.Foundation.Async;
using StansAssets.Foundation.Patterns;
using StansAssets.SceneManagement;
using UISystem;
using UnityEngine;

namespace HeroesFlight.StateStack.State
{
    [UsedImplicitly]
    public class MainMenuState : BaseApplicationLoadSceneState, IAppState
    {
        public ApplicationState ApplicationState => ApplicationState.MainMenu;

        public void Init(ServiceLocator serviceLocator)
        {
            InitLocator(serviceLocator);
        }

        public override void ChangeState(StackChangeEvent<ApplicationState> evt, IProgressReporter progressReporter)
        {
            switch (evt.Action)
            {
                case StackAction.Added:
                   // Debug.Log(ApplicationState);
                    progressReporter.SetDone();
                    var uiSystem = GetService<IUISystem>();
                    var dataSystem = GetService<DataSystemInterface>();
                    var traitSystem = GetService<TraitSystemInterface>();
                    var diceSystem = GetService<DiceSystemInterface>();
                    
                    SubscribeEvents();

                    traitSystem.Init();
                    uiSystem.UiEventHandler.MainMenu.Open(dataSystem.AccountLevelManager.CurrentPlayerLvl);
                    dataSystem.CurrencyManager.TriggerAllCurrencyChange();
                    dataSystem.StatManager.ProcessTraitsStatsModifiers(traitSystem.GetUnlockedEffects());
                    uiSystem.UiEventHandler.MainMenu.LoadWorlds(dataSystem.WorldManger.Worlds);
                    uiSystem.UiEventHandler.MainMenu.AccountLevelUp(dataSystem.AccountLevelManager.GetExpIncreaseResponse());
                    break;
                case StackAction.Paused:
                    break;
                case StackAction.Resumed:
                    break;
                case StackAction.Removed:
                    progressReporter.SetDone();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(evt.Action), evt.Action, null);
            }
        }

        public void SubscribeEvents()
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

            uiSystem.UiEventHandler.LevelUpMenu.GetRewardVisuals += LevelUpMenu_GetRewardVisuals;
            dataSystem.AccountLevelManager.OnLevelUp += AccountLevelManager_OnLevelUp;
        }

        private System.UI.Reward.RewardVisual[] LevelUpMenu_GetRewardVisuals()
        {
            var dataSystem = GetService<DataSystemInterface>();
            return GetService<RewardSystemInterface>().GiveLevelUpReward(dataSystem.AccountLevelManager.GetGemReward(),
            dataSystem.AccountLevelManager.GetGoldReward());
        }

        private void AccountLevelManager_OnLevelUp(LevelSystem.ExpIncreaseResponse obj)
        {
            var uiSystem = GetService<IUISystem>();
            uiSystem.UiEventHandler.MainMenu.AccountLevelUp (obj);
            uiSystem.UiEventHandler.LevelUpMenu.AccountLevelUp (obj);
            uiSystem.UiEventHandler.MainMenu.ProcessButtonStates(obj.currentLevel);
        }

        public void UnSubscribeEvents()
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

            uiSystem.UiEventHandler.LevelUpMenu.GetRewardVisuals -= LevelUpMenu_GetRewardVisuals;
            dataSystem.AccountLevelManager.OnLevelUp -= AccountLevelManager_OnLevelUp;
        }

        void HandleGameStartRequest()
        {
            var dataSystem = GetService<DataSystemInterface>();
            if (dataSystem.EnergyManager.UseEnergy(5))
            {
                UnSubscribeEvents();
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
            uiSystem.UiEventHandler.DiceMenu.ShowDiceMenu(value, () =>
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