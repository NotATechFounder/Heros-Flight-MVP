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
            this.chestPriceText.text = chestPriceText.ToString();
        }
    }
}
