using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoDisplayUI : MonoBehaviour
{
    public event Action OnDismantleAction;
    public event Func<bool> OnUpgradeAction;
    public event Action OnEquipAction;
    public event Action OnUnequipAction;

    [SerializeField] Image itemBackground;
    [SerializeField] Image itemFrame;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemRairityDisplay;
    [SerializeField] TextMeshProUGUI itemTypeDisplay;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemLevel;
    [SerializeField] TextMeshProUGUI equipOrUnequipText;

    [Header("Stats")]
    [SerializeField] Transform statsHolder;

    [Header("Dismantle")]
    [SerializeField] TextMeshProUGUI dismantleMaterialDisplay;
    [SerializeField] TextMeshProUGUI dismantleGoldDisplay;

    [Header("Upgrade")]
    [SerializeField] TextMeshProUGUI upgradeGoldPriceDisplay;
    [SerializeField] GameObject upgradeMaterialHolder;
    [SerializeField] Image upgradeMaterialIcon;
    [SerializeField] TextMeshProUGUI requiredMaterialName;
    [SerializeField] TextMeshProUGUI materialAmountDisplay;

    [Header("Buttons")]
    [SerializeField] AdvanceButton closeButton;
    [SerializeField] AdvanceButton dismantleButton;
    [SerializeField] AdvanceButton upgradeButton;
    [SerializeField] AdvanceButton equipOrUnequipButton;

    [Header("Debug")]
    [SerializeField] Item item;

    private Action equipOrUnequipAction;
    IInventoryItemHandler inventoryItemHandler;

    private void Start()
    {
        closeButton.onClick.AddListener(Close);
        dismantleButton.onClick.AddListener(() => OnDismantleAction?.Invoke());
        upgradeButton.onClick.AddListener(HandleUpgrade);
        equipOrUnequipButton.onClick.AddListener(() => equipOrUnequipAction?.Invoke());
    }

    public void Init(IInventoryItemHandler inventoryItemHandler)
    {
        this.inventoryItemHandler = inventoryItemHandler;
    }

    public void Display (Item item)
    {
        gameObject.SetActive(true);
        this.item = item;
        bool isEquipped = item.GetItemData<ItemEquipmentData>().eqquiped;
        equipOrUnequipAction = isEquipped ? OnUnequipAction : OnEquipAction;
        equipOrUnequipText.text = isEquipped ? "Unequip" : "Equip";
        itemLevel.text = "LV." + item.GetItemData<ItemEquipmentData>().value.ToString();
        itemIcon.sprite = item.itemSO.icon;

        itemRairityDisplay.text = item.GetItemData<ItemEquipmentData>().rarity.ToString();
        itemTypeDisplay.text = item.GetItemSO<EquipmentSO>().equipmentType.ToString();
        itemName.text = item.itemSO.Name;

        RarityPalette rarityPalette = inventoryItemHandler.GetPalette(item.GetItemData<ItemEquipmentData>().rarity);
        itemBackground.color = rarityPalette.backgroundColour;
        itemFrame.color = rarityPalette.frameColour;

        SetItemLevel();
        SeUpgradeInfo();
    }

    private void HandleUpgrade()
    {
        if (OnUpgradeAction?.Invoke() == true)
        {
            SetItemLevel();
            SeUpgradeInfo();
        }
    }

    public void SetItemLevel()
    {
        itemLevel.text = "LV." + item.GetItemData<ItemEquipmentData>().value.ToString() + " / " + inventoryItemHandler.GetItemMaxLevel(item).ToString();

        if (item.GetItemData<ItemEquipmentData>().value >= inventoryItemHandler.GetItemMaxLevel(item))
        {
            upgradeMaterialHolder.SetActive(false);
            upgradeGoldPriceDisplay.text = "MAX";
            upgradeGoldPriceDisplay.color = Color.red;
        }
        else
        {
            upgradeMaterialHolder.SetActive(true);
            upgradeGoldPriceDisplay.text = inventoryItemHandler.GetGoldUpgradeRequiredAmount(item).ToString();
            upgradeGoldPriceDisplay.color = Color.white;
        }
    }

    void SeUpgradeInfo()
    {
        inventoryItemHandler.GetMaterialItemByID("M_" + item.GetItemSO<EquipmentSO>().equipmentType.ToString(), out Item materialItem);
        if (materialItem == null)
        {        
            ItemSO itemSO = inventoryItemHandler.GetItemSO("M_" + item.GetItemSO<EquipmentSO>().equipmentType.ToString());
            upgradeMaterialIcon.sprite = itemSO.icon;
            requiredMaterialName.text = itemSO.Name;
            materialAmountDisplay.text = inventoryItemHandler.GetMaterialUpgradeRequiredAmount(item) + " / " + 0.ToString();
            materialAmountDisplay.color = Color.red;
            return;
        }
        upgradeMaterialIcon.sprite = materialItem.itemSO.icon;
        requiredMaterialName.text = materialItem.itemSO.Name;
        materialAmountDisplay.text = inventoryItemHandler.GetMaterialUpgradeRequiredAmount(item) + " / " + materialItem.GetItemData<ItemData>().value.ToString();
        materialAmountDisplay.color = materialItem.GetItemData<ItemData>().value >= inventoryItemHandler.GetMaterialUpgradeRequiredAmount(item) ? Color.green : Color.red;

        dismantleMaterialDisplay.text = inventoryItemHandler.GetTotalUpgradeMaterialSpent(item.GetItemData<ItemEquipmentData>()).ToString();
        dismantleGoldDisplay.text = inventoryItemHandler.GetTotalUpgradeGoldSpent(item.GetItemData<ItemEquipmentData>()).ToString();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
