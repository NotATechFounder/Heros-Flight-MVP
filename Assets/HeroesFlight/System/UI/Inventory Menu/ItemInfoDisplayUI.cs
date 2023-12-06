using System;
using HeroesFlight.System.UI.Inventory_Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoDisplayUI : MonoBehaviour
{
    public event Action OnDismantleAction;
    public Action OnUpgradeRequest;
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
    private InventoryDataConverterInterface converter;

    private void Start()
    {
        closeButton.onClick.AddListener(Close);
        dismantleButton.onClick.AddListener(() => OnDismantleAction?.Invoke());
        upgradeButton.onClick.AddListener(HandleUpgrade);
        equipOrUnequipButton.onClick.AddListener(() => equipOrUnequipAction?.Invoke());
    }

    public void Init(InventoryDataConverterInterface dataConverter)
    {
        converter = dataConverter;
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

        UpgradeItem(item);
    }

    private void HandleUpgrade()
    {
        OnUpgradeRequest?.Invoke();
    }

    public void UpgradeItem(EquipmentEntryUi item)
    {
        this.item = item;
        SetItemLevel();
        SeUpgradeInfo();
    }

    public void SetItemLevel()
    {
        var maxLvl =converter.GetMaxItemLvl(item);
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
            upgradeGoldPriceDisplay.text = converter.GetGoldAmount(item).ToString();
            upgradeGoldPriceDisplay.color = Color.white;
        }
    }

    void SeUpgradeInfo()
    {
        var materialItem = converter.GetMaterial("M_" + item.EquipmentType.ToString());
       
        if (materialItem == null)
        {
            var inventoryItem = converter.GetEquipment(item.ID);
           
            upgradeMaterialIcon.sprite = inventoryItem.Icon;
            requiredMaterialName.text = inventoryItem.Name;
            materialAmountDisplay.text =
              converter.GetMaterialAmount(item) + " / " + 0.ToString();
            materialAmountDisplay.color = Color.red;
            return;
        }

        var materialsRequired =  converter.GetMaterialAmount(item);
        upgradeMaterialIcon.sprite = materialItem.Icon;
        requiredMaterialName.text = materialItem.Name;
        materialAmountDisplay.text = materialsRequired + " / " +
                                     materialItem.Value.ToString();
        materialAmountDisplay.color =
            materialItem.Value >= materialsRequired
                ? Color.green
                : Color.red;

        dismantleMaterialDisplay.text = converter.GetMaterialSpentAmount(item).ToString();
        dismantleGoldDisplay.text =converter.GetGoldSpentAmount(item)
            .ToString();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}