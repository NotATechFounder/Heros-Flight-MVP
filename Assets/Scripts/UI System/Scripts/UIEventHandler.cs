using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;

public class UIEventHandler : MonoBehaviour
{
    MainMenu _mainMenu = null;
    LoadingMenu _loadingMenu = null;
    GameMenu _gameMenu = null;


    private void Start()
    {
        OnGameOpened();
    }

    public void OnGameOpened()
    {
        _mainMenu = UIManager.OpenMenu<MainMenu>();
    }

    public void OnGameSessionStarted()
    {
        _gameMenu = UIManager.OpenMenu<GameMenu>();
    }

    public void OnGamePaused()
    {

    }
}
