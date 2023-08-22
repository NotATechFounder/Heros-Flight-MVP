using System;
using HeroesFlight.Core.StateStack.Enum;
using HeroesFlight.System.Character;
using HeroesFlight.System.UI;
using JetBrains.Annotations;
using StansAssets.Foundation.Async;
using StansAssets.Foundation.Patterns;
using StansAssets.SceneManagement;
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
                    var characterSystem = GetService<CharacterSystemInterface>();
                    var dataSystem = GetService<DataSystemInterface>();

                    uiSystem.UiEventHandler.MainMenu.Open();

                    void HandleGameStartRequest()
                    {
                        uiSystem.UiEventHandler.MainMenu.OnPlayButtonPressed -= HandleGameStartRequest;
                        uiSystem.UiEventHandler.MainMenu.OnCharacterSelectButtonPressed -=
                            HandleCharacterSelectionRequest;
                        AppStateStack.State.Set(ApplicationState.Gameplay);
                    }

                    void HandleCharacterSelectionRequest()
                    {
                        uiSystem.UiEventHandler.CharacterSelectionMenu.SetUnlockedCharacters(characterSystem.GetUnlockedClasses());
                        uiSystem.UiEventHandler.CharacterSelectionMenu.Open();
                    }
                    
                    uiSystem.UiEventHandler.MainMenu.OnPlayButtonPressed += HandleGameStartRequest;
                    uiSystem.UiEventHandler.MainMenu.OnCharacterSelectButtonPressed +=
                        HandleCharacterSelectionRequest;
                    if (dataSystem.RewardHandler.RewardPending)
                    {
                       
                        CoroutineUtility.WaitForSeconds(.2f, () =>
                        {
                            dataSystem.RewardHandler.ConsumeReward();
                            uiSystem.UiEventHandler.RewardPopup.Open();
                        });
                       
                    }
                    
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