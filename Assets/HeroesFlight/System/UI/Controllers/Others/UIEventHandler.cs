using System;
using UISystem;
using UnityEngine;

public class UIEventHandler : MonoBehaviour
{
    [SerializeField] private ConfirmationUISO backToMenu;
    [SerializeField] private ConfirmationUISO puzzleConfirmation;

    private UIManager uIManager;

    public MainMenu MainMenu { get; private set;}
    public SettingsMenu SettingsMenu { get; private set; }
    public LoadingMenu LoadingMenu { get; private set; }
    public GameMenu GameMenu { get; private set; }
    public PauseMenu PauseMenu { get; private set; }
    public ConfirmationMenu ConfirmationMenu { get; private set; }
    public ReviveMenu ReviveMenu { get; private set; }
    public SummaryMenu SummaryMenu { get; private set; }
    public PuzzleMenu PuzzleMenu { get; private set; }
    public PopUpManager PopupManager { get; private set; }
    public AngelGambitMenu AngelGambitMenu { get; private set; }
    public AngelPermanetCardMenu AngelPermanetCardMenu { get; private set; }
    public HeroProgressionMenu HeroProgressionMenu { get; private set; }
    public ConfirmationUISO BackToMenuConfirmation => backToMenu;
    public ConfirmationUISO PuzzleConfirmation => puzzleConfirmation;

    public void Init(Action OnComplecte = null)
    {
        uIManager = GetComponent<UIManager>();
        LoadingMenu = uIManager.InitMenu<LoadingMenu>();
        ConfirmationMenu = uIManager.InitMenu<ConfirmationMenu>();
        MainMenu = uIManager.InitMenu<MainMenu>();
        SettingsMenu = uIManager.InitMenu<SettingsMenu>();
        GameMenu = uIManager.InitMenu<GameMenu>();
        PauseMenu = uIManager.InitMenu<PauseMenu>();
        ReviveMenu = uIManager.InitMenu<ReviveMenu>();
        SummaryMenu = uIManager.InitMenu<SummaryMenu>();
        PuzzleMenu = uIManager.InitMenu<PuzzleMenu>();
        AngelGambitMenu = uIManager.InitMenu<AngelGambitMenu>();
        AngelPermanetCardMenu = uIManager.InitMenu<AngelPermanetCardMenu>();
        HeroProgressionMenu = uIManager.InitMenu<HeroProgressionMenu>();
        PopupManager = PopUpManager.Instance;
        OnComplecte?.Invoke();
    }
}
