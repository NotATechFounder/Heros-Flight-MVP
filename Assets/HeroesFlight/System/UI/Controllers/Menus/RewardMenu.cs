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

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;
        JuicerRuntime glowBgSpinEffect;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            glowBgSpinEffect = glowBg.transform.JuicyRotate(new Vector3(0, 0, 360), 1f).SetLoop(-1);

            quitButton.onClick.AddListener(Close);
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

        public void DisplayRewardsVisual(ChestType chestType, params RewardVisual[] rewardVisual)
        {
            selectedChestEffect = Array.Find(chestEffects, x => x.chestType == chestType);
            selectedChestEffect.SetActive(true);
            selectedChestEffect.skeletonAnimation.AnimationState.SetAnimation(0, IdleAnimation, false);
            //DisplayRewardsVisual(rewardVisual);
        }

        private void AnimationState_Event(TrackEntry trackEntry, Spine.Event e)
        {
            if (e.Data.Name == "Display")
            {
  
            }
        }

        private void AnimationState_Complete(TrackEntry trackEntry)
        {
            throw new NotImplementedException();
        }


        private void OnChestRewardFinish()
        {
            selectedChestEffect.SetActive(true);
        }
    }
}
