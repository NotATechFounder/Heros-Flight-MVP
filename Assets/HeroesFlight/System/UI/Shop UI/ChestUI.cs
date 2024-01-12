using System;
using TMPro;
using UnityEngine;

namespace UISystem
{
    public class ChestUI : MonoBehaviour
    {
        [SerializeField] private ChestType chestType;
        [SerializeField] private AdvanceButton chestButton;
        [SerializeField] private TextMeshProUGUI chestInfoText;
        [SerializeField] private TextMeshProUGUI chestPriceText;

        public ChestType GetChestType => chestType;
        public AdvanceButton ChestButton => chestButton;

        public void SetChestUI(string chestInfoText, int chestPriceText)
        {
            this.chestInfoText.text = chestInfoText;
            SetPrice (chestPriceText);
        }

        public void SetPrice(int price)
        {
            string priceText = price == 0 ? "Not Ready" : price.ToString();
            chestPriceText.text = priceText;
            chestButton.SetVisibility(price == 0 ? GameButtonVisiblity.Hidden : GameButtonVisiblity.Visible);
        }
    }
}
