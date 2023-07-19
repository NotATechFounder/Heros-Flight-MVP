using System;
using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;

public class UIEventHandler : MonoBehaviour
{
    [SerializeField] private UIManager _uIManager;

    private MainMenu _mainMenu = null;
    private SettingsMenu _settingsMenu = null;
    private  LoadingMenu _loadingMenu = null;
    private GameMenu _gameMenu = null;
    private PauseMenu _pauseMenu = null;
    private ConfirmationMenu _confirmationMenu = null;

    private void Awake()
    {
        Init();
    }

    private void OnDestroy()
    {
        _mainMenu.OnPlayButtonPressed -= MainMenu_OnPlayButtonPressed;
        _mainMenu.OnSettingsButtonPressed -= MainMenu_OnSettingsButtonPressed;
    }

    private void Start()
    {
        OnGameOpened();
    }

    private void Init()
    {
        _loadingMenu = _uIManager.InitMenu<LoadingMenu>();
        _confirmationMenu = _uIManager.InitMenu<ConfirmationMenu>();

        _mainMenu = _uIManager.InitMenu<MainMenu>();
        _mainMenu.OnPlayButtonPressed += MainMenu_OnPlayButtonPressed;
        _mainMenu.OnSettingsButtonPressed += MainMenu_OnSettingsButtonPressed;

        _settingsMenu = _uIManager.InitMenu<SettingsMenu>();
        _settingsMenu.OnBackButtonPressed += () =>
        {
            _settingsMenu.Close();
        };

        _gameMenu = _uIManager.InitMenu<GameMenu>();
        _gameMenu.OnPauseButtonClicked += () =>
        {
            _pauseMenu.Open();
        };

        _pauseMenu = _uIManager.InitMenu<PauseMenu>();
        _pauseMenu.OnSettingsButtonClicked += () =>
        {
            _settingsMenu.Open();
        };

        _pauseMenu.OnResumeButtonClicked += () =>
        {
            _pauseMenu.Close();
        };

        _pauseMenu.OnQuitButtonClicked += () =>
        {
            _confirmationMenu.Display("Leaving ?", "Are you sure you  want to leave the game?","If you leave the game you will lose all progress.", ReturnToMaimMenu, null);
        };
    }

    public void OnGameOpened()
    {
        _mainMenu.Open();
    }

    private void MainMenu_OnPlayButtonPressed()
    {
        _gameMenu.Open();
        _mainMenu.Close();
    }

    private void MainMenu_OnSettingsButtonPressed()
    {
        _settingsMenu.Open();
    }

    public void OnGameSessionStarted()
    {
        _gameMenu.Open();
    }

    public void OnGamePaused()
    {

    }

    public void ReturnToMaimMenu()
    {
        _gameMenu.Close();
        _pauseMenu.Close();
        _mainMenu.Open();
    }
}
