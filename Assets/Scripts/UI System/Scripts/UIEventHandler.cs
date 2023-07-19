using System;
using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;

public class UIEventHandler : MonoBehaviour
{
    [SerializeField] private UIManager uIManager;
    [SerializeField] private ConfirmationUISO backToMenu;

    private MainMenu mainMenu = null;
    private SettingsMenu settingsMenu = null;
    private  LoadingMenu _loadingMenu = null;
    private GameMenu gameMenu = null;
    private PauseMenu pauseMenu = null;
    private ConfirmationMenu confirmationMenu = null;

    private CountDownTimer startTimer;

    private void Awake()
    {
        Init();
    }

    private void OnDestroy()
    {
        mainMenu.OnPlayButtonPressed -= MainMenu_OnPlayButtonPressed;
        mainMenu.OnSettingsButtonPressed -= MainMenu_OnSettingsButtonPressed;
    }

    private void Start()
    {
        OnGameOpened();
    }

    private void Init()
    {
        _loadingMenu = uIManager.InitMenu<LoadingMenu>();
        confirmationMenu = uIManager.InitMenu<ConfirmationMenu>();

        mainMenu = uIManager.InitMenu<MainMenu>();
        mainMenu.OnPlayButtonPressed += MainMenu_OnPlayButtonPressed;
        mainMenu.OnSettingsButtonPressed += MainMenu_OnSettingsButtonPressed;

        settingsMenu = uIManager.InitMenu<SettingsMenu>();
        settingsMenu.OnBackButtonPressed += () =>
        {
            settingsMenu.Close();
        };

        gameMenu = uIManager.InitMenu<GameMenu>();
        gameMenu.OnPauseButtonClicked += () =>
        {
            pauseMenu.Open();
        };

        pauseMenu = uIManager.InitMenu<PauseMenu>();
        pauseMenu.OnSettingsButtonClicked += () =>
        {
            settingsMenu.Open();
        };

        pauseMenu.OnResumeButtonClicked += () =>
        {
            pauseMenu.Close();
        };

        pauseMenu.OnQuitButtonClicked += () =>
        {
            confirmationMenu.Display(backToMenu, ReturnToMainMenu, null);
        };

        startTimer = new CountDownTimer(this);
    }

    public void OnGameOpened()
    {
        mainMenu.Open();
    }

    private void MainMenu_OnPlayButtonPressed()
    {
        mainMenu.Close();
        gameMenu.Open();

        startTimer.Start(3, (time) =>
        {
            gameMenu.UpdateTimerText(time);
        }, 
        () =>
        {
            startTimer.Start(200, (time) =>
            {
                gameMenu.UpdateTimerText(time);
            },
          () =>
          {
              Debug.Log("Game Time Lapse");
          });
        });
    }

    private void MainMenu_OnSettingsButtonPressed()
    {
        settingsMenu.Open();
    }

    public void OnGameSessionStarted()
    {
        gameMenu.Open();
    }

    public void OnGamePaused()
    {

    }

    public void ReturnToMainMenu()
    {
        gameMenu.Close();
        pauseMenu.Close();
        mainMenu.Open();
    }
}
