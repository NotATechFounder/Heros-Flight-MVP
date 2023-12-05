using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using TMPro;
using System.Collections.Generic;

public class IAPManager : MonoBehaviour
{
    const string gem80 = "com.StudioGema.Heros-Flight-MVP.gem80";
    const string gem500 = "com.StudioGema.Heros-Flight-MVP.gem500";
    const string gem1200 = "com.StudioGema.Heros-Flight-MVP.gem1200";
    const string gem2500 = "com.StudioGema.Heros-Flight-MVP.gem2500";
    const string gem6500 = "com.StudioGema.Heros-Flight-MVP.gem6500";
    const string gem14000 = "com.StudioGema.Heros-Flight-MVP.gem14000";
    const string battlePass = "com.StudioGema.Heros-Flight-MVP.premiumbattlepass";

    [SerializeField] private TextMeshProUGUI gem80Text;
    [SerializeField] private TextMeshProUGUI gem500Text;
    [SerializeField] private TextMeshProUGUI gem1200Text;
    [SerializeField] private TextMeshProUGUI gem2500Text;
    [SerializeField] private TextMeshProUGUI gem6500Text;
    [SerializeField] private TextMeshProUGUI gem14000Text;
    [SerializeField] private TextMeshProUGUI battlePassText;
    [SerializeField] private GameObject restorePurchaseButton;

    private void Start()
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer) restorePurchaseButton?.SetActive(false);
    }

    public void OnProductFetched(Product product)
    {
        switch (product.definition.id)
        {
            case gem80:
                gem80Text.text = product.metadata.localizedPriceString + product.metadata.isoCurrencyCode;
                break;
            case gem500:
                gem500Text.text = product.metadata.localizedPriceString + product.metadata.isoCurrencyCode;
                break;
            case gem1200:
                gem1200Text.text = product.metadata.localizedPriceString + product.metadata.isoCurrencyCode;
                break;
            case gem2500:
                gem2500Text.text = product.metadata.localizedPriceString + product.metadata.isoCurrencyCode;
                break;
            case gem6500:
                gem6500Text.text = product.metadata.localizedPriceString + product.metadata.isoCurrencyCode;
                break;
            case gem14000:
                gem14000Text.text = product.metadata.localizedPriceString + product.metadata.isoCurrencyCode;
                break;
            case battlePass:
                battlePassText.text = product.metadata.localizedPriceString + product.metadata.isoCurrencyCode;
                break;
            default: break;
        }
    }

    public void OnPurchaseComplete(Product product)
    {
        switch (product.definition.id)
        {
            case gem80:
               // ShopManager.Instance.AddCurency(Currency.CurrencyType.Gem, 80);
                break;
                case gem500:
                  //  ShopManager.Instance.AddCurency(Currency.CurrencyType.Gem, 500);
                break;
                case gem1200:
                  //  ShopManager.Instance.AddCurency(Currency.CurrencyType.Gem, 1200);
                break;
                case gem2500:
                  //  ShopManager.Instance.AddCurency(Currency.CurrencyType.Gem, 2500);
                break;
                case gem6500:
                   // ShopManager.Instance.AddCurency(Currency.CurrencyType.Gem, 6500);
                break;
                case gem14000:
                  //  ShopManager.Instance.AddCurency(Currency.CurrencyType.Gem, 14000);
                break;
                case battlePass:
                   // BattlePassManager.Instance.UnlockPremiumReward();
                break;
            default: break;
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription purchaseFailureDescription)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, purchaseFailureDescription.reason));
    }
}