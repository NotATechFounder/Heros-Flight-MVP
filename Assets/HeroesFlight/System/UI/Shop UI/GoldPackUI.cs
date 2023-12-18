using TMPro;
using UnityEngine;

namespace UISystem
{
    public class GoldPackUI : MonoBehaviour
    {
        [SerializeField] private GoldPackType goldPack;
        [SerializeField] private TextMeshProUGUI goldAmountText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private AdvanceButton buyButton;

        public GoldPackType GetGoldPackType => goldPack;
        public AdvanceButton BuyButton => buyButton;

        public void SetGoldPackUI(int goldAmount, int price)
        {
            goldAmountText.text = goldAmount.ToString();
            priceText.text = price.ToString();
        }
    }
}
