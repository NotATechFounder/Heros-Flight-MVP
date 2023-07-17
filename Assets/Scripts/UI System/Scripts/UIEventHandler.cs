using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;

public class UIEventHandler : MonoBehaviour
{
    MainMenu mainMenu = null;
    LoadingMenu loadingMenu = null;

    private void Start()
    {
        OnGameStarted();
    }

    public void Init()
    {

    }

    public void OnGameStarted()
    {
        loadingMenu = UIManager.OpenMenu<LoadingMenu>();
    }

    public void OnGamePaused()
    {

    }
}
