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
using Spine.Unity;
using Spine;
using StansAssets.Foundation.Async;

namespace UISystem
{
    public class RewardMenu : BaseMenu<RewardMenu>
    {
        [Serializable]
        public class ChestEffect
        {
            public ChestType chestType;
            public SkeletonGraphic skeletonAnimation;

            public void SetActive(bool value)
            {
                skeletonAnimation.gameObject.SetActive(value);
            }
        }

        private const string IdleAnimation = "Idle";
        private const string OpenAnimation = "Open";

        [SerializeField] private AdvanceButton quitButton;

        [Header("Reward")]
        [SerializeField] private RectTransform glowBg;
        [SerializeField] private GameObject rewardViewParent;
        [SerializeField] private RewardView rewardViewPrefab;

        [Header("Chest Animation")]
        [SerializeField] private ChestEffect[] chestEffects;

        private List<RewardView> rewardViews = new List<RewardView>();
        private ChestEffect selectedChestEffect;
        private RewardVisual[] rewardVisualArray;

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;
        JuicerRuntime glowBgSpinEffect;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            glowBgSpinEffect = glowBg.transform.JuicyRotate(new Vector3(0, 0, 360), 1f).SetLoop(-1, LoopType.Incremental);

            quitButton.onClick.AddListener(Close);

            AssignChestAnimation();
        }

        public override void OnOpened()
        {
            canvasGroup.alpha = 0;
            openEffectBG.Start();
            glowBgSpinEffect.Start();
        }


        public override void OnClosed()
        {
            closeEffectBG.Start();
            glowBgSpinEffect.Stop();
        }

        public override void ResetMenu()
        {

        }

        public void AssignChestAnimation()
        {
            foreach (ChestEffect chestEffect in chestEffects)
            {
                chestEffect.skeletonAnimation.AnimationState.Event += AnimationState_Event;
                chestEffect.skeletonAnimation.AnimationState.Complete += AnimationState_Complete;
            }
        }

        public void DisplayRewards(params RewardVisual[] rewardVisual)
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
        }

        public void DisplayRewardsVisual(params RewardVisual[] rewardVisual)
        {
            Open();
            DisplayRewards(rewardVisual);
        }

        public void DisplayRewardsVisual(ChestType chestType, params RewardVisual[] rewardVisual)
        {
            Open();

            if (selectedChestEffect != null)
            {
                selectedChestEffect.SetActive(false);
            }

            selectedChestEffect = Array.Find(chestEffects, x => x.chestType == chestType);
            selectedChestEffect.SetActive(true);
            rewardVisualArray = rewardVisual;

            selectedChestEffect.skeletonAnimation.AnimationState.SetAnimation(0, IdleAnimation, true);

            CoroutineUtility.WaitForSeconds(1f, () =>
            {
                selectedChestEffect.skeletonAnimation.AnimationState.SetAnimation(0, OpenAnimation, false);
            });

            DisplayRewardsVisual(rewardVisualArray);
        }

        private void AnimationState_Event(TrackEntry trackEntry, Spine.Event e)
        {
            if (e.Data.Name == "Display")
            {
  
            }
        }

        private void AnimationState_Complete(TrackEntry trackEntry)
        {
            switch (trackEntry.Animation.Name)
            {
                case IdleAnimation:


                    break;
                case OpenAnimation:

                    break;
            }
        }

        private void OnChestRewardFinish()
        {
            selectedChestEffect.SetActive(true);
        }
    }
}
