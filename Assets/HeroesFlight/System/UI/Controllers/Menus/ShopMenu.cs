using HeroesFlight.Common.Enum;
using HeroesFlight.System.UI.Inventory_Menu;
using HeroesFlight.System.UI.Reward;
using Pelumi.Juicer;
using Pelumi.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace UISystem
{
    public class ShopMenu : BaseMenu<ShopMenu>
    {
        public event Action<IAPHelper.ProductType> OnPurchaseSuccess;
        public event Action<ChestType> TryPurchaseChest;
        public event Action<GoldPack> TryPurchaseGoldPack;

        [Header("Chest Pack Buttons")]
        [SerializeField] private AdvanceButton regularChestButton;
        [SerializeField] private AdvanceButton rareChestButton;
        [SerializeField] private AdvanceButton epicChestButton;
        [SerializeField] private AdvanceButton epic10ChestButton;

        [Header("Gold Pack Buttons")]
        [SerializeField] private AdvanceButton smallGoldPackButton;
        [SerializeField] private AdvanceButton mediumGoldPackButton;
        [SerializeField] private AdvanceButton largeGoldPackButton;

        [Header("IAP Text")]
        [SerializeField] private TextMeshProUGUI gem80Text;
        [SerializeField] private TextMeshProUGUI gem500Text;
        [SerializeField] private TextMeshProUGUI gem1200Text;
        [SerializeField] private TextMeshProUGUI gem6500Text;

        [Header("Reward")]
        [SerializeField] private GameObject rewardDisplay;
        [SerializeField] private GameObject rewardViewParent;
        [SerializeField] private RewardView rewardViewPrefab;

        [Header("Other Buttons")]
        [SerializeField] private AdvanceButton restorePurchaseButton;
        [SerializeField] private AdvanceButton closeRewardViewButton;
        [SerializeField] private AdvanceButton quitButton;

        private List<RewardView> rewardViews = new List<RewardView>();

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            quitButton.onClick.AddListener(Close);

            restorePurchaseButton?.gameObject.SetActive(Application.platform == RuntimePlatform.IPhonePlayer);

            regularChestButton.onClick.AddListener(() => { TryPurchaseChest?.Invoke(ChestType.Regular); });
            rareChestButton.onClick.AddListener(() => { TryPurchaseChest?.Invoke(ChestType.Rare); });
            epicChestButton.onClick.AddListener(() => { TryPurchaseChest?.Invoke(ChestType.Epic); });
            epic10ChestButton.onClick.AddListener(() => { TryPurchaseChest?.Invoke(ChestType.Epic_10); });

            smallGoldPackButton.onClick.AddListener(() => { TryPurchaseGoldPack?.Invoke(GoldPack.Small); });
            mediumGoldPackButton.onClick.AddListener(() => { TryPurchaseGoldPack?.Invoke(GoldPack.Medium); });
            largeGoldPackButton.onClick.AddListener(() => { TryPurchaseGoldPack?.Invoke(GoldPack.Large); });

            closeRewardViewButton.onClick.AddListener(() => { rewardDisplay.SetActive(false); });
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

        public void OnProductFetched(Product product)
        {
            switch (product.definition.id)
            {
                case IAPHelper.Gem80:
                    gem80Text.text = product.metadata.localizedPriceString + product.metadata.isoCurrencyCode;
                    break;
                case IAPHelper.Gem500:
                    gem500Text.text = product.metadata.localizedPriceString + product.metadata.isoCurrencyCode;
                    break;
                case IAPHelper.Gem1200:
                    gem1200Text.text = product.metadata.localizedPriceString + product.metadata.isoCurrencyCode;
                    break;
                case IAPHelper.Gem6500:
                    gem6500Text.text = product.metadata.localizedPriceString + product.metadata.isoCurrencyCode;
                    break;
                default: break;
            }
        }

        public void OnPurchaseComplete(Product product)
        {
            switch (product.definition.id)
            {
                case IAPHelper.Gem80:
                    OnPurchaseSuccess?.Invoke(IAPHelper.ProductType.Gem80);
                    break;
                case IAPHelper.Gem500:
                    OnPurchaseSuccess?.Invoke(IAPHelper.ProductType.Gem500);
                    break;
                case IAPHelper.Gem1200:
                    OnPurchaseSuccess?.Invoke(IAPHelper.ProductType.Gem1200);
                    break;
                case IAPHelper.Gem6500:
                    OnPurchaseSuccess?.Invoke(IAPHelper.ProductType.Gem6500);
                    break;
                default: break;
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription purchaseFailureDescription)
        {
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, purchaseFailureDescription.reason));
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

            rewardDisplay.SetActive(true);
        }
    }
}
