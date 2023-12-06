using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem.Entries;
using UnityEngine;

namespace UISystem
{
    public class DailyRewardMenu : BaseMenu<DailyRewardMenu>
    {
        public event Func<int> GetLastUnlockedIndex;
        public event Func<bool> IsRewardReady;
        public event Action OnContinueButtonClicked;
        public event Action OnClaimRewardButtonClicked; 

        [SerializeField] private DailyRewardUI[] dailyRewardUIs;

        [Header("Buttons")]
        [SerializeField] AdvanceButton homeButton;
        [SerializeField] AdvanceButton retryButton;
        [SerializeField] AdvanceButton continueButton;

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            continueButton.onClick.AddListener(CloseButtonAction);

            InitDailyRewards();
        }
       
        public override void OnOpened()
        {
            openEffectBG.Start();
            RefreshDailyRewards();
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
        }

        public override void ResetMenu()
        {

        }

        private void CloseButtonAction()
        {
            OnContinueButtonClicked?.Invoke();
            Close();
        }

        public void InitDailyRewards()
        {
            for (int i = 0; i < dailyRewardUIs.Length; i++)
            {
                dailyRewardUIs[i].SetRewardIndex(i);
                dailyRewardUIs[i].OnRewardButtonClicked += RewardButtonAction;
            }
        }

        public void RefreshDailyRewards()
        {
            int lastUnlockedIndex = GetLastUnlockedIndex?.Invoke() ?? 0;
            bool isRewardReady = IsRewardReady?.Invoke() ?? false;

            for (int i = 0; i < dailyRewardUIs.Length; i++)
            {
                if (isRewardReady && i == lastUnlockedIndex)
                {
                    dailyRewardUIs[i].SetState(DailyRewardUI.State.Ready);
                }
                else
                {
                    dailyRewardUIs[i].SetState(i < lastUnlockedIndex ? DailyRewardUI.State.Claimed : DailyRewardUI.State.NotReady);
                }
            }
        }

        private void RewardButtonAction(int index)
        {
            OnClaimRewardButtonClicked?.Invoke();
            dailyRewardUIs[index].SetState(DailyRewardUI.State.Claimed);
        }

        public void OnRewardReadyToBeCollected(int index)
        {
            for (int i = 0; i < dailyRewardUIs.Length; i++)
            {
                if (i == index)
                {
                    dailyRewardUIs[i].SetState(DailyRewardUI.State.Ready);
                }
                else
                {
                    dailyRewardUIs[i].SetState(i < index ? DailyRewardUI.State.Claimed : DailyRewardUI.State.NotReady);
                }
            }
        }
    }
}

