using System;
using HeroesFlight.Common;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.UI.Inventory_Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquippedSlot : MonoBehaviour
{
    public Action<InventoryItemUiEntry> OnSelectItem;

    [SerializeField] private EquipmentType equipmentType;
    [SerializeField] AdvanceButton selectButton;
    [SerializeField] GameObject content;
    [SerializeField] Image itemRarityColour;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemInfo;

    private bool isOccupied;
    private InventoryItemUiEntry itemInSlot;

    public EquipmentType GetEquipmentType => equipmentType;
    public bool IsOccupied => isOccupied;
    public InventoryItemUiEntry GetItem => itemInSlot;

    private void Start()
    {
        selectButton.onClick.AddListener(SelectItem);
    }

    public void Occupy(InventoryItemUiEntry item, RarityPalette rarityPalette)
    {
        content.SetActive(true);
        isOccupied = true;
        itemInSlot = item;
        itemIcon.sprite = item.Icon;
        itemRarityColour.color = rarityPalette.backgroundColour;
        SetItemInfo();
    }

    public void SetItemInfo()
    {
        itemInfo.text = "LV." + itemInSlot.Value.ToString();
    }

    private void SelectItem()
    {
        if (isOccupied)
        {
            OnSelectItem?.Invoke(itemInSlot);
        }
    }

    public void UnOccupy()
    {
        isOccupied = false;
        itemInSlot = null;
        content.SetActive(false);
    }
}
