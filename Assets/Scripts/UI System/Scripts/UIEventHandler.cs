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
        _mainMenu = _uIManager.InitMenu<MainMenu>(false);
        _mainMenu.OnPlayButtonPressed += MainMenu_OnPlayButtonPressed;
        _mainMenu.OnSettingsButtonPressed += MainMenu_OnSettingsButtonPressed;

        _settingsMenu = _uIManager.InitMenu<SettingsMenu>(false);
        _settingsMenu.OnBackButtonPressed += () =>
        {
            _settingsMenu.Close();
        };

        _loadingMenu = _uIManager.InitMenu<LoadingMenu>(false);
        _gameMenu = _uIManager.InitMenu<GameMenu>(false);
        _pauseMenu = _uIManager.InitMenu<PauseMenu>(false);
    }

    public void OnGameOpened()
    {
        _mainMenu.Open();

    }

    private void MainMenu_OnPlayButtonPressed()
    {
        Debug.Log("Play button pressed");
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
}
