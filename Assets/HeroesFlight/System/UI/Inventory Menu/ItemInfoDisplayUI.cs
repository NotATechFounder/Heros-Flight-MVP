using System;
using HeroesFlight.System.UI.Inventory_Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoDisplayUI : MonoBehaviour
{
    public event Action OnDismantleAction;
    public event Func<bool> OnUpgradeAction;
    Func<EquipmentEntryUi, int> GetItemMaxLvlAction;
    Func<EquipmentEntryUi, int> GetGoldForUpdateAction;
    Func<string, InventoryItemUiEntry> GetInventoryItemByIdAction;
    Func<string, InventoryItemUiEntry> GetMaterialItemByIdAction;
    Func<EquipmentEntryUi, int> GetMaterialUpgradeAmount;
    Func<EquipmentEntryUi, int> GetTotalMaterialSpent;
    Func<EquipmentEntryUi, int> GetTotalGoldSpent;
    public event Action OnEquipAction;
    public event Action OnUnequipAction;

    [SerializeField] Image itemBackground;
    [SerializeField] Image itemFrame;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemRairityDisplay;
    [SerializeField] TextMeshProUGUI itemTypeDisplay;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemLevel;
    [SerializeField] TextMeshProUGUI itemDescription;
    [SerializeField] TextMeshProUGUI equipOrUnequipText;

    [Header("Stats")] [SerializeField] Transform statsHolder;

    [Header("Dismantle")] [SerializeField] TextMeshProUGUI dismantleMaterialDisplay;
    [SerializeField] TextMeshProUGUI dismantleGoldDisplay;

    [Header("Upgrade")] [SerializeField] TextMeshProUGUI upgradeGoldPriceDisplay;
    [SerializeField] GameObject upgradeMaterialHolder;
    [SerializeField] Image upgradeMaterialIcon;
    [SerializeField] TextMeshProUGUI requiredMaterialName;
    [SerializeField] TextMeshProUGUI materialAmountDisplay;

    [Header("Buttons")] [SerializeField] AdvanceButton closeButton;
    [SerializeField] AdvanceButton dismantleButton;
    [SerializeField] AdvanceButton upgradeButton;
    [SerializeField] AdvanceButton equipOrUnequipButton;

    [Header("Debug")] [SerializeField] EquipmentEntryUi item;

    private Action equipOrUnequipAction;


    private void Start()
    {
        closeButton.onClick.AddListener(Close);
        dismantleButton.onClick.AddListener(() => OnDismantleAction?.Invoke());
        upgradeButton.onClick.AddListener(HandleUpgrade);
        equipOrUnequipButton.onClick.AddListener(() => equipOrUnequipAction?.Invoke());
    }

    public void Init(Func<EquipmentEntryUi, int> maxLvlCallback, Func<EquipmentEntryUi, int> goldcallback,
        Func<string, InventoryItemUiEntry> getMaterialCallback, Func<string, InventoryItemUiEntry> getitemCallback,
        Func<EquipmentEntryUi, int> materialAmountCallback, Func<EquipmentEntryUi, int> materialSpent,
        Func<EquipmentEntryUi, int> goldSpent)
    {
        GetItemMaxLvlAction = maxLvlCallback;
        GetGoldForUpdateAction = goldcallback;
        GetMaterialItemByIdAction = getMaterialCallback;
        GetInventoryItemByIdAction = getitemCallback;
        GetMaterialUpgradeAmount = materialAmountCallback;
        GetTotalMaterialSpent = materialSpent;
        GetTotalGoldSpent = goldSpent;
    }

    public void Display(EquipmentEntryUi item)
    {
        gameObject.SetActive(true);
        this.item = item;
        bool isEquipped = item.IsEquipped;
        equipOrUnequipAction = isEquipped ? OnUnequipAction : OnEquipAction;
        equipOrUnequipText.text = isEquipped ? "Unequip" : "Equip";
        itemLevel.text = "LV." + item.Value.ToString();
        itemIcon.sprite = item.Icon;

        itemRairityDisplay.text = item.ItemRarity.ToString();
        itemTypeDisplay.text = item.EquipmentType.ToString();
        itemName.text = item.Name;
        itemDescription.text = item.Description;

        var rarityPalette = this.item.RarityPallete;
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
        var maxLvl = GetItemMaxLvlAction?.Invoke(item);
        itemLevel.text = "LV." + item.Value.ToString() + " / " + maxLvl.ToString();

        if (item.Value >= maxLvl)
        {
            upgradeMaterialHolder.SetActive(false);
            upgradeGoldPriceDisplay.text = "MAX";
            upgradeGoldPriceDisplay.color = Color.red;
        }
        else
        {
            upgradeMaterialHolder.SetActive(true);
            upgradeGoldPriceDisplay.text = GetGoldForUpdateAction?.Invoke(item).ToString();
            upgradeGoldPriceDisplay.color = Color.white;
        }
    }

    void SeUpgradeInfo()
    {
        var materialItem = GetMaterialItemByIdAction?.Invoke("M_" + item.EquipmentType.ToString());
        // inventoryItemHandler.GetMaterialItemByID(,
        //     out Item materialItem);
        if (materialItem == null)
        {
            var inventoryItem = GetInventoryItemByIdAction?.Invoke("M_" + item.EquipmentType.ToString());
            //   inventoryItemHandler.GetItemSO("M_" + item.GetItemSO<EquipmentSO>().equipmentType.ToString());

            upgradeMaterialIcon.sprite = inventoryItem.Icon;
            requiredMaterialName.text = inventoryItem.Name;
            materialAmountDisplay.text =
                GetMaterialUpgradeAmount?.Invoke(item) + " / " + 0.ToString();
            materialAmountDisplay.color = Color.red;
            return;
        }

        var materialsRequired = GetMaterialUpgradeAmount?.Invoke(item);
        upgradeMaterialIcon.sprite = materialItem.Icon;
        requiredMaterialName.text = materialItem.Name;
        materialAmountDisplay.text = materialsRequired + " / " +
                                     materialItem.Value.ToString();
        materialAmountDisplay.color =
            materialItem.Value >= materialsRequired
                ? Color.green
                : Color.red;

        dismantleMaterialDisplay.text = GetTotalMaterialSpent?.Invoke(item).ToString();
        dismantleGoldDisplay.text = GetTotalGoldSpent?.Invoke(item)
            .ToString();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}