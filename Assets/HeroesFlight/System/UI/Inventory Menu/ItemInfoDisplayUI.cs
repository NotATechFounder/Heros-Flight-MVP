using Codice.CM.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoDisplayUI : MonoBehaviour
{
    public event Action OnDismantleAction;
    public event Func<bool> OnUpgradeAction;
    public event Action OnEquipAction;
    public event Action OnUnequipAction;

    [SerializeField] GameObject displayUI;
    [SerializeField] Image itemRarityColour;
    [SerializeField] Image itemFrame;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemRairityDisplay;
    [SerializeField] TextMeshProUGUI itemTypeDisplay;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemLevel;
    [SerializeField] TextMeshProUGUI itemLevelDetails;
    [SerializeField] TextMeshProUGUI equipOrUnequipText;

    [Header("Stats")]
    [SerializeField] Transform statsHolder;

    [Header("Dismantle")]
    [SerializeField] TextMeshProUGUI sellScrollDisplay;
    [SerializeField] TextMeshProUGUI sellPriceDisplay;

    [Header("Upgrade")]
    [SerializeField] TextMeshProUGUI upgradePriceDisplay;
    [SerializeField] TextMeshProUGUI requiredScrollDisplay;
    [SerializeField] TextMeshProUGUI scrollAmountDisplay;

    [Header("Buttons")]
    [SerializeField] AdvanceButton closeButton;
    [SerializeField] AdvanceButton dismantleButton;
    [SerializeField] AdvanceButton upgradeButton;
    [SerializeField] AdvanceButton equipOrUnequipButton;

    [Header("Debug")]
    [SerializeField] Item item;

    private Action equipOrUnequipAction;

    private void Start()
    {
        closeButton.onClick.AddListener(Close);
        dismantleButton.onClick.AddListener(() => OnDismantleAction?.Invoke());
        upgradeButton.onClick.AddListener(HandleUpgrade);
        equipOrUnequipButton.onClick.AddListener(() => equipOrUnequipAction?.Invoke());
    }

    public void Display (Item item)
    {
        gameObject.SetActive(true);
        this.item = item;
        bool isEquipped = item.ItemData().eqquiped;
        equipOrUnequipAction = isEquipped ? OnUnequipAction : OnEquipAction;
        equipOrUnequipText.text = isEquipped ? "Unequip" : "Equip";
        itemLevel.text = "LV." + item.ItemData().value.ToString();
        itemIcon.sprite = item.itemSO.icon;
        SetItemInfo();
    }

    private void HandleUpgrade()
    {
        if (OnUpgradeAction?.Invoke() == true)
        {
            SetItemInfo();
        }
    }

    public void SetItemInfo()
    {
        itemLevel.text = "LV." + item.ItemData().value.ToString();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
