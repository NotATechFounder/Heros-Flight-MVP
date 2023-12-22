using HeroesFlight.System.UI.Reward;
using Pelumi.Juicer;
using Pelumi.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem;
using UnityEngine;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing;

namespace UISystem
{
    public class RewardMenu : BaseMenu<RewardMenu>
    {
        [SerializeField] private AdvanceButton quitButton;

        [Header("Reward")]
        [SerializeField] private GameObject rewardViewParent;
        [SerializeField] private RewardView rewardViewPrefab;

        private List<RewardView> rewardViews = new List<RewardView>();

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            quitButton.onClick.AddListener(Close);
        }

        public override void OnOpened()
        {
            canvasGroup.alpha = 0;
            openEffectBG.Start();
        }


        public override void OnClosed()
        {
            closeEffectBG.Start();
        }

        public override void ResetMenu()
        {

        }

        public void DisplayRewardsVisual(params RewardVisual[] rewardVisual)
        {
            foreach (RewardView rewardView in rewardViews)
            {
                ObjectPoolManager.ReleaseObject(rewardView.gameObject);
            }

            rewardViews.Clear();

            for (int i = 0; i < rewardVisual.Length; i++)
            {
                RewardView rewardView = ObjectPoolManager.SpawnObject(rewardViewPrefab, rewardViewParent.transform, PoolType.UI);
                rewardView.SetVisual(rewardVisual[i]);
                rewardViews.Add(rewardView);
            }

            Open();
        }
    }
}
