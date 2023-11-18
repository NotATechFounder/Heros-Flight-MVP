using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.Core.StateStack.Enum;
using HeroesFlight.System.Character;
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
                    Debug.Log(ApplicationState);
                    progressReporter.SetDone();
                    var uiSystem = GetService<IUISystem>();
                    var dataSystem = GetService<DataSystemInterface>();
                    TraitSystemInterface traitSystemInterface = GetService<TraitSystemInterface>();
                    Debug.Log("Initing trait system");
                    traitSystemInterface.Init();
                    uiSystem.UiEventHandler.MainMenu.Open();

                    uiSystem.UiEventHandler.MainMenu.OnInventoryButtonPressed += uiSystem.UiEventHandler.InventoryMenu.Open;
                    uiSystem.UiEventHandler.MainMenu.AddGem += () => dataSystem.CurrencyManager.AddCurency(CurrencyKeys.Gem, 10000);
                    uiSystem.UiEventHandler.MainMenu.AddGold += () => dataSystem.CurrencyManager.AddCurency(CurrencyKeys.Gold, 10000);

                    uiSystem.UiEventHandler.InventoryMenu.OnChangeHeroButtonClicked += uiSystem.UiEventHandler.CharacterSelectMenu.Open;
                    uiSystem.UiEventHandler.InventoryMenu.OnStatPointButtonClicked += uiSystem.UiEventHandler.StatePointsMenu.Open;
                    uiSystem.UiEventHandler.InventoryMenu.GetStatModel += dataSystem.StatManager.GetStatModel;

                    uiSystem.UiEventHandler.CharacterSelectMenu.OnMenuClosed += uiSystem.UiEventHandler.InventoryMenu.Open;
                    uiSystem.UiEventHandler.CharacterSelectMenu.GetAllCharacterSO += dataSystem.CharacterManager.GetAllCharacterSO;
                    uiSystem.UiEventHandler.CharacterSelectMenu.OnTryBuyCharacter += dataSystem.CharacterManager.TryBuyCharacter;

                    uiSystem.UiEventHandler.InventoryMenu.GetSelectedCharacterSO += dataSystem.CharacterManager.GetSelectedCharacter;
                    uiSystem.UiEventHandler.CharacterSelectMenu.OnCharacterSelected += dataSystem.CharacterManager.ToggleCharacterSelected;

                    uiSystem.UiEventHandler.StatePointsMenu.GetCurrentSpLevel += dataSystem.StatPoints.GetSp;
                    uiSystem.UiEventHandler.StatePointsMenu.OnAddSpClicked += dataSystem.StatPoints.TryAddSp;
                    uiSystem.UiEventHandler.StatePointsMenu.OnRemoveSpClicked += dataSystem.StatPoints.TrytRemoveSp;
                    uiSystem.UiEventHandler.StatePointsMenu.GetAvailabletSp += dataSystem.StatPoints.GetAvailableSp;
                    uiSystem.UiEventHandler.StatePointsMenu.OnCompletePressed += dataSystem.StatPoints.Confirm;

                    void HandleGameStartRequest()
                    {
                        uiSystem.UiEventHandler.MainMenu.OnPlayButtonPressed -= HandleGameStartRequest;
                        
                        AppStateStack.State.Set(ApplicationState.Gameplay);
                    } 
                    
                    uiSystem.UiEventHandler.MainMenu.OnPlayButtonPressed += HandleGameStartRequest;
                    dataSystem.CurrencyManager.OnCurrencyChanged += uiSystem.UiEventHandler.MainMenu.CurrencyChanged;
                    dataSystem.CurrencyManager.TriggerAllCurrencyChange();

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
    }
}