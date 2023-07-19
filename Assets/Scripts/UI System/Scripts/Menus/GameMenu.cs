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
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI enemyCountText;
        [SerializeField] private Button pauseButton;

        [Header("level Progress")]
        [SerializeField] private TextMeshProUGUI levelProgressText;
        [SerializeField] private Image levelProgressFill;

        [Header("Combo Counter")]
        [SerializeField] private TextMeshProUGUI comboCounterText;

        [Header("Boss")]
        [SerializeField] private GroupImageFill bossHealthFill;

        JuicerRuntime openEffect;
        JuicerRuntime closeEffect;

        public override void OnCreated()
        {
            openEffect = canvasGroup.JuicyAlpha(1, 0.5f);
            openEffect.SetOnStart(() => canvasGroup.alpha = 0);

            closeEffect = canvasGroup.JuicyAlpha(0, 0.5f);
            closeEffect.SetOnStart(() => canvasGroup.alpha = 1);
            closeEffect.SetOnComplected(CloseMenu);

            pauseButton.onClick.AddListener(() => OnPauseButtonClicked?.Invoke());

            ResetMenu();
        }

        public override void OnOpened()
        {
            openEffect.Start();
        }

        public override void OnClosed()
        {
            closeEffect.Start();
        }

        public override void ResetMenu()
        {
            coinText.text = "0";
            timerText.text = "00:00";
            enemyCountText.text = "0";
            comboCounterText.text = "0";
        }

        public void UpdateCoinText(int value)
        {
            coinText.text = value.ToString();
        }

        public void UpdateTimerText(float value)
        {
            float minutes = Mathf.FloorToInt(value / 60);
            float seconds = Mathf.FloorToInt(value % 60);
            //timerText.text = (minutes == 0) ? $"{seconds.ToString("F0")}s" : $"{minutes:00}m : {seconds:00}s";
            timerText.text = (minutes == 0) ? $"{seconds.ToString("F0")}" : $"{minutes:00} : {seconds:00}";
        }

        public void UpdateTimerText(string value)
        {
            timerText.text = value;
        }

        public void UpdateEnemyCountText(int value)
        {
            enemyCountText.text = value.ToString();
        }

        public void UpdateComboCounterText(int value)
        {
            comboCounterText.text = value.ToString();
        }

        public void UpdateLevelProgressText(int value)
        {
            levelProgressText.text = value.ToString();
        }

        public void UpdateLevelProgressFill(float value)
        {
            levelProgressFill.fillAmount = value;
        }

        public void UpdateLevelProgress(int value, float fill)
        {
            UpdateLevelProgressText(value);
            UpdateLevelProgressFill(fill);
        }

        public void UpdateBossHealthFill(float value)
        {
            bossHealthFill.SetValue(value);
        }
    }
}
