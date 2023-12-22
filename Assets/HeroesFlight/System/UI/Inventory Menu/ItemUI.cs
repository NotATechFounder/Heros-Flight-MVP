using System;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.Common;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.UI.Inventory_Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Action<ItemUI> OnSelectItem;

    [Header("Item Info")]
    [SerializeField] Image itemRarityColour;
    [SerializeField] Image itemFrame;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemInfo;
    [SerializeField] AdvanceButton selectButton;

    [Header("Others")]
    [SerializeField] GameObject itemNotify;
    [SerializeField] GameObject selectionCheck;
    [SerializeField] GameObject lockImage;

    [Header("Debug")]
    [SerializeField] bool selected;
    [SerializeField] InventoryItemUiEntry item;

    public InventoryItemUiEntry GetItem => item;    

    public AdvanceButton SelectButton => selectButton;

    private void Start()
    {
        selectButton.onClick.AddListener(SelectItem);
    }

    public void SetItem(InventoryItemUiEntry item, RarityPalette rarityPalette)
    {
        OnSelectItem = null;
        this.item = item;
        itemIcon.sprite = item.Icon;
        itemRarityColour.color = rarityPalette.backgroundColour;
        itemFrame.color = rarityPalette.frameColour;
        SetItemInfo();
    }

    public void SetItemInfo()
    {
        string valueType = item.ItemType == ItemType.Equipment ? "LV." : "QTY.";
        itemInfo.text = valueType + item.Value.ToString();
    }

    private void SelectItem()
    {
        OnSelectItem?.Invoke(this);
    }
}
