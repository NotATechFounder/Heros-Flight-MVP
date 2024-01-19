using Pelumi.Juicer;
using UnityEngine;
using TMPro;
using System;
using UISystem.Entries;
using UnityEngine.UI;
using HeroesFlight.System.UI.Reward;
using System.Collections.Generic;

namespace UISystem
{
    public class SummaryMenu : BaseMenu<SummaryMenu>
    {
        public Func<string> GetCurrentGold;
        public Func<string> GetCurrentTime;
        public Func<List<RewardVisualEntry>> GetRewardVisuals;

        public event Action OnContinueButtonClicked;

        [Header("Texts")]
        [SerializeField] TextMeshProUGUI coinText;
        [SerializeField] TextMeshProUGUI timeText;

        [Header("level")]
        [SerializeField] TextMeshProUGUI levelText;
        [SerializeField] TextMeshProUGUI expText;
        [SerializeField] Image expBar;

        [Header("Buttons")]
        [SerializeField] AdvanceButton homeButton;
        [SerializeField] AdvanceButton retryButton;
        [SerializeField] AdvanceButton continueButton;

        [Header("Rewards")]
        [SerializeField] RewardView entryPrefab;
        [SerializeField] Transform rewardsParent;

        private List<RewardView> rewards = new List<RewardView>();
        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            continueButton.onClick.AddListener(CloseButtonAction);
        }

        public override void OnOpened()
        {
            openEffectBG.Start();

            if (GetCurrentGold != null)
                coinText.text = GetCurrentGold.Invoke();

            foreach (var reward in rewards)
            {
                Destroy(reward.gameObject);
            }
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
            foreach (Transform child in rewardsParent)
            {
                Destroy(child.gameObject);
            }
        }

        public override void ResetMenu()
        {

        }

        private void CloseButtonAction()
        {
            OnContinueButtonClicked?.Invoke();
            Close();
        }

        public void AddRewardEntry(RewardVisualEntry rewardVisualEntry)
        {
            RewardView entry = Instantiate(entryPrefab, rewardsParent);
            entry.SetVisual(rewardVisualEntry);
            rewards.Add(entry);
        }
    }
}
