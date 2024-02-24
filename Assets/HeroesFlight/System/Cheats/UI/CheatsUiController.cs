using System;
using HeroesFlight.System.Cheats.Data;
using HeroesFlight.System.Cheats.Enum;
using UnityEngine;
using UnityEngine.UI;

namespace HeroesFlight.System.Cheats.UI
{
    public class CheatsUiController : MonoBehaviour,CheatsUiControllerInterface
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [Header("Target Ui elements")]
        [SerializeField] private Button navigationButton;
        [SerializeField] private Button killAllButton;
        [SerializeField] private Button addCurencyButton;
        [SerializeField] private Button addItemsButton;
        [SerializeField] private Button Unlocktraits;
        [SerializeField] private Toggle heroImmortalityTogle;

        [Header("ParentPanel")]
        [SerializeField] private RectTransform panel;
        public event Action<CheatButtonClickModel> OnCheatButtonClicked;
        private bool isShowing;
        private void Awake()
        {
            navigationButton.onClick.AddListener(() =>
            {
                OnCheatButtonClicked?.Invoke(new CheatButtonClickModel(CheatsButtonType.Navigation));
                if (!isShowing)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            });
            killAllButton.onClick.AddListener(() =>
            {
                OnCheatButtonClicked?.Invoke(new CheatButtonClickModel(CheatsButtonType.KillAllMobs));
            });
            addCurencyButton.onClick.AddListener(() =>
            {
                OnCheatButtonClicked?.Invoke(new CheatButtonClickModel(CheatsButtonType.AddCurrency));
            });
            addItemsButton.onClick.AddListener(() =>
            {
                OnCheatButtonClicked?.Invoke(new CheatButtonClickModel(CheatsButtonType.AddItems));
            });
            Unlocktraits.onClick.AddListener(() =>
            {
                OnCheatButtonClicked?.Invoke(new CheatButtonClickModel(CheatsButtonType.UnlockTraits));
            });
            heroImmortalityTogle.onValueChanged.AddListener((value) =>
            {
                OnCheatButtonClicked?.Invoke(new CheatButtonClickModel(CheatsButtonType.Immortality,value));
            }); 
        }

        public void SetState(bool isEnabled)
        {
            canvasGroup.interactable = isEnabled;
            canvasGroup.alpha = isEnabled ? 255 : 0;
        }

        void Show()
        {
            panel.anchoredPosition -= new Vector2(panel.rect.width , 0);
            isShowing = true;
        }

        void Hide()
        {
            panel.anchoredPosition += new Vector2(panel.rect.width , 0);
            isShowing = false;
        }
    }
}