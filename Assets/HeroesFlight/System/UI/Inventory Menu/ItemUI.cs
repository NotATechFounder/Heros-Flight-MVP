using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Action<ItemUI> OnSelectItem;

    [SerializeField] GameObject selectionCheck;
    [SerializeField] Image itemRarityColour;
    [SerializeField] Image itemFrame;
    [SerializeField] GameObject itemNotify;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemInfo;
    [SerializeField] Button selectButton;
    [SerializeField] GameObject lockImage;

    [Header("Debug")]
    [SerializeField] bool selected;
    [SerializeField] Item item;

    public Item GetItem => item;    

    private void Start()
    {
        selectButton.onClick.AddListener(SelectItem);
    }

    public void SetItem(Item item)
    {
        this.item = item;
        itemIcon.sprite = item.itemSO.icon;
    }

    private void SelectItem()
    {
        OnSelectItem?.Invoke(this);
    }
}
