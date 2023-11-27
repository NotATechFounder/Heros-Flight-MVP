using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] Item item;

    public Item GetItem => item;    

    private void Start()
    {
        selectButton.onClick.AddListener(SelectItem);
    }

    public void SetItem(Item item, RarityPalette rarityPalette)
    {
        this.item = item;
        itemIcon.sprite = item.itemSO.icon;
        itemRarityColour.color = rarityPalette.backgroundColour;
        itemFrame.color = rarityPalette.frameColour;
        SetItemInfo();
    }

    public void SetItemInfo()
    {
        string valueType = item.itemSO.itemType == ItemType.Equipment ? "LV." : "QTY.";
        itemInfo.text = valueType + item.GetItemData().value.ToString();
    }

    private void SelectItem()
    {
        OnSelectItem?.Invoke(this);
    }
}
