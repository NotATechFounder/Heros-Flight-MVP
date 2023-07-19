using System;
using System.Collections.Generic;
using HeroesFlight.System.UI.Controllers;
using HeroesFlight.System.UI.Enum;
using StansAssets.Foundation.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.UI
{
    public class UiSystem : IUISystem
    {
        public UiSystem()
        {
            m_UiStateChangeHandlers.Add(UiSystemState.MainMenu, () =>
            {
                m_HudController.Hide();
                m_MainMenuController.Show();
                m_LoaderController.Hide();
            });
            m_UiStateChangeHandlers.Add(UiSystemState.Gameplay, () =>
            {
                m_MainMenuController.Hide();
                m_HudController.Show();
                m_LoaderController.Hide();
            });
        }

        public event Action OnStartGameSessionRequest;
        public event Action OnReturnToMainMenuRequest;
        Dictionary<UiSystemState, Action> m_UiStateChangeHandlers = new();
        IMainMenuController m_MainMenuController;
        IHudController m_HudController;
        IUiLoaderController m_LoaderController;

        UiSystemState m_CurrentState;

        public void Init(Scene scene = default, Action OnComplete = null)
        {
            m_MainMenuController = scene.GetComponent<IMainMenuController>();
            m_HudController = scene.GetComponent<IHudController>();
            m_LoaderController = scene.GetComponent<IUiLoaderController>();
            m_LoaderController.Init();
            m_MainMenuController.Init();
            m_HudController.Init();

            m_MainMenuController.OnGameSessionStartRequest += HandleGameSessionStartRequest;
            m_HudController.OnReturnToMainMenuRequest += HandleReturnToMainMenuRequest;
            m_CurrentState = UiSystemState.Loading;
            OnComplete.Invoke();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void SetLoaderState(bool isEnabled)
        {
            if (isEnabled)
            {
                m_LoaderController.Show();
            }
            else
            {
                m_LoaderController.Hide();
            }
        }

        public void SetUiState(UiSystemState newState)
        {
            if (newState.Equals(m_CurrentState))
                return;

            m_CurrentState = newState;
            if (m_UiStateChangeHandlers.TryGetValue(newState, out var handle))
            {
                handle.Invoke();
            }
        }

        void HandleReturnToMainMenuRequest()
        {
            Debug.Log("Return to main menu request");
            OnReturnToMainMenuRequest?.Invoke();
        }

        void HandleGameSessionStartRequest()
        {
            OnStartGameSessionRequest?.Invoke();
        }
    }
}