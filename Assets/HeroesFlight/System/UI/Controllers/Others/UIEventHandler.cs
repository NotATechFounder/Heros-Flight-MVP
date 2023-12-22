using System;
using HeroesFlight.System.UI.DIce;
using HeroesFlight.System.UI.Traits;
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
    public GodsBenevolencePuzzleMenu GodsBenevolencePuzzleMenu { get; private set; }
    public AngelGambitMenu AngelGambitMenu { get; private set; }
    public AngelPermanetCardMenu AngelPermanetCardMenu { get; private set; }
    public StatPointsMenu StatePointsMenu { get; private set; }
    public ConfirmationUISO BackToMenuConfirmation => backToMenu;
    public ConfirmationUISO PuzzleConfirmation => puzzleConfirmation;
    public RewardPopupController RewardPopup { get; private set; }

    public HealingNPCMenu HealingNPCMenu { get; private set; }
    public ActiveAbilityRerollerNPCMenu ActiveAbilityRerollerNPCMenu { get; private set; }
    public PassiveAbilityRerollerNPCMenu PassiveAbilityRerollerNPCMenu { get; private set; }
    public TraitTreeMenu TraitTreeMenu { get; private set; }
    public DiceMenu DiceMenu { get; private set; }

    public AbilitySelectMenu AbilitySelectMenu { get; private set; }

    public CharacterSelectMenu CharacterSelectMenu { get; private set; }
    public InventoryMenu InventoryMenu { get; private set; }

    public DailyRewardMenu DailyRewardMenu { get; private set; }
    public ShopMenu ShopMenu { get; private set; }
    public RewardMenu RewardMenu { get; private set; }
    public TutorialMenu TutorialMenu { get; private set; }

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
        GodsBenevolencePuzzleMenu = uIManager.InitMenu<GodsBenevolencePuzzleMenu>();
        AngelGambitMenu = uIManager.InitMenu<AngelGambitMenu>();
        AngelPermanetCardMenu = uIManager.InitMenu<AngelPermanetCardMenu>();
        StatePointsMenu = uIManager.InitMenu<StatPointsMenu>();
        RewardPopup = uIManager.InitMenu<RewardPopupController>();
        HealingNPCMenu = uIManager.InitMenu<HealingNPCMenu>();
        TraitTreeMenu = uIManager.InitMenu<TraitTreeMenu>();
        ActiveAbilityRerollerNPCMenu = uIManager.InitMenu<ActiveAbilityRerollerNPCMenu>();
        AbilitySelectMenu = uIManager.InitMenu<AbilitySelectMenu>();
        PassiveAbilityRerollerNPCMenu = uIManager.InitMenu<PassiveAbilityRerollerNPCMenu>();
        DiceMenu=uIManager.InitMenu<DiceMenu>();
        CharacterSelectMenu = uIManager.InitMenu<CharacterSelectMenu>();
        InventoryMenu = uIManager.InitMenu<InventoryMenu>();
        DailyRewardMenu = uIManager.InitMenu<DailyRewardMenu>();
        ShopMenu = uIManager.InitMenu<ShopMenu>();
        RewardMenu = uIManager.InitMenu<RewardMenu>();
        TutorialMenu = uIManager.InitMenu<TutorialMenu>();
        OnComplecte?.Invoke();
    }
}
