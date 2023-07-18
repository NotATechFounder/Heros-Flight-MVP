using Pelumi.Juicer;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace UISystem
{
    public class GameMenu : BaseMenu<GameMenu>
    {
        public event Action OnPauseButtonClicked;

        [Header("Main")]
        [SerializeField] private TextMeshProUGUI _coinText;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private TextMeshProUGUI _enemyCountText;
        [SerializeField] private Button _pauseButton;

        [Header("level Progress")]
        [SerializeField] private TextMeshProUGUI _levelProgressText;
        [SerializeField] private Image _levelProgressFill;

        [Header("Combo Counter")]
        [SerializeField] private TextMeshProUGUI _comboCounterText;

        JuicerRuntime _openEffect;
        JuicerRuntime _closeEffect;

        public override void OnCreated()
        {
            _openEffect = _canvasGroup.JuicyAlpha(1, 0.5f);
            _openEffect.SetOnStart(() => _canvasGroup.alpha = 0);

            _closeEffect = _canvasGroup.JuicyAlpha(0, 0.5f);
            _closeEffect.SetOnStart(() => _canvasGroup.alpha = 1);
            _closeEffect.SetOnComplected(CloseMenu);

            _pauseButton.onClick.AddListener(() => OnPauseButtonClicked?.Invoke());

            ResetMenu();
        }

        public override void OnOpened()
        {
            _openEffect.Start();
        }

        public override void OnClosed()
        {
            _closeEffect.Start();
        }

        public override void ResetMenu()
        {
            _coinText.text = "0";
            _timerText.text = "00:00";
            _enemyCountText.text = "0";
            _comboCounterText.text = "0";
        }

        public void UpdateCoinText(int value)
        {
            _coinText.text = value.ToString();
        }

        public void UpdateTimerText(float value)
        {
            _timerText.text = value.ToString("00:00");
        }

        public void UpdateEnemyCountText(int value)
        {
            _enemyCountText.text = value.ToString();
        }

        public void UpdateComboCounterText(int value)
        {
            _comboCounterText.text = value.ToString();
        }

        public void UpdateLevelProgressText(int value)
        {
            _levelProgressText.text = value.ToString();
        }

        public void UpdateLevelProgressFill(float value)
        {
            _levelProgressFill.fillAmount = value;
        }

        public void UpdateLevelProgress(int value, float fill)
        {
            UpdateLevelProgressText(value);
            UpdateLevelProgressFill(fill);
        }
    }
}
