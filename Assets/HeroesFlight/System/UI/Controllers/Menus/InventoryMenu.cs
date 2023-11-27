using HeroesFlight.Common.Enum;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pelumi.Juicer;
using HeroesFlight.Common.Progression;

namespace UISystem
{
    public class InventoryMenu : BaseMenu<InventoryMenu>
    {
        public event Func<StatModel> GetStatModel;
        public event Func<CharacterSO> GetSelectedCharacterSO;
        public event Action OnChangeHeroButtonClicked;
        public event Action OnStatPointButtonClicked;

        [Header("Buttons")]
        [SerializeField] private AdvanceButton changeHeroButton;
        [SerializeField] private AdvanceButton statPointButton;
        [SerializeField] private AdvanceButton quitButton;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI currentAtk;
        [SerializeField] private TextMeshProUGUI currentHp;
        [SerializeField] private TextMeshProUGUI currentDef;

        [Header("Data")]
        [SerializeField] private UiSpineViewController uiSpineViewController;

        [Header("Inventory")]
        [SerializeField] private ItemUI itemUIPrefab;
        [SerializeField] private Transform itemHolder;
        [SerializeField] private ItemInfoDisplayUI itemInfoDisplayUI;
        [SerializeField] private EquippedSlot[] equippedSlots;

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        [Header("Debug")]
        private Dictionary<string, ItemUI> itemUIDic = new Dictionary<string, ItemUI>();
        [SerializeField] private ItemUI selectedItemUI;
        [SerializeField] private EquippedSlot selectedEquippedSlot;
        [SerializeField] Item selectedItem;

        IInventoryItemHandler inventoryItemHandler;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            changeHeroButton.onClick.AddListener(() => OnChangeHeroButtonClicked?.Invoke());
            statPointButton.onClick.AddListener(() => OnStatPointButtonClicked?.Invoke());
            quitButton.onClick.AddListener(Close);
        }

        public override void OnOpened()
        {
            canvasGroup.alpha = 0;
            openEffectBG.Start();
            UpdateCharacter(GetSelectedCharacterSO.Invoke());
            LoadInventoryItems();
        }

        public void UpdateCharacter(CharacterSO characterSO)
        {
            uiSpineViewController.SetupView(characterSO);
            OnStatValueChanged (GetStatModel.Invoke());
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
        }

        public override void ResetMenu()
        {

        }

        public void OnStatValueChanged(StatModel statModel)
        {
            foreach (var stat in statModel.CurrentStatDic)
            {
                switch (stat.Key)
                {
                    case StatType.PhysicalDamage:
                        currentAtk.text = stat.Value.ToString("F0");
                        break;
                    case StatType.MaxHealth:
                        currentHp.text = stat.Value.ToString("F0");
                        break;
                    case StatType.Defense:
                        currentDef.text = stat.Value.ToString("F0");
                        break;
                }
            }
        }

        public void InitInventory(IInventoryItemHandler inventoryItemHandler)
        {
            this.inventoryItemHandler = inventoryItemHandler;
            inventoryItemHandler.OnItemAdded += SpawnItemUI;
            inventoryItemHandler.OnItemModified += UpdateItemUI;

            itemInfoDisplayUI.OnEquipAction += EquipItem;
            itemInfoDisplayUI.OnDismantleAction += DismantleItem;
            itemInfoDisplayUI.OnUpgradeAction += TryUpgradeItem;
            itemInfoDisplayUI.OnUnequipAction += UnEquipItem;

            itemInfoDisplayUI.Init(inventoryItemHandler);

            foreach (EquippedSlot slot in equippedSlots)
            {
                slot.OnSelectItem += (item) =>
                {
                    selectedEquippedSlot = slot;
                    ItemSelected (item);
                };
            }
        }

        public void ClearInventoryItems()
        {
            foreach (ItemUI itemUI in itemUIDic.Values)
            {
                Destroy(itemUI.gameObject);
            }
            itemUIDic.Clear();
        }

        public void LoadInventoryItems()
        {
            ClearInventoryItems();

            foreach (Item item in inventoryItemHandler.GetInventoryEquippmentItems())
            {
                if (item.GetItemData().eqquiped)
                {
                    EquippedSlot equippedSlot = GetEquipmentSlot((item.itemSO as EquipmentSO).equipmentType);
                    if (equippedSlot != null)
                    {
                        equippedSlot.Occupy(item);
                    }
                }
                else
                {
                    SpawnItemUI(item);
                }
            }

            foreach (Item item in inventoryItemHandler.GetInventoryMaterialItems())
            {
                SpawnItemUI(item);
            }
        }

        private bool TryUpgradeItem()   
        {
            if (selectedItem != null)
            {
                if (inventoryItemHandler.TryUpgradeItem(selectedItem) == true)
                {
                    return true;
                }
            }
            return false;
        }

        private void DismantleItem()
        {
            if (selectedItem != null)
            {
                inventoryItemHandler.DismantleItem(selectedItem);
                itemUIDic.Remove(selectedItem.GetItemData().instanceID);
            }

            if (selectedItemUI != null)
            {
                Destroy(selectedItemUI.gameObject);
                selectedItemUI = null;
            }

            if (selectedEquippedSlot != null)
            {
                selectedEquippedSlot.UnOccupy();
                selectedItemUI = null;
            }

            itemInfoDisplayUI.Close();
        }

        private void EquipItem()
        {
            EquippedSlot equippedSlot = GetEquipmentSlot((selectedItemUI.GetItem.itemSO as EquipmentSO).equipmentType);
            if (equippedSlot != null)
            {
                if (equippedSlot.IsOccupied)
                {
                    SpawnItemUI(equippedSlot.GetItem);
                    equippedSlot.UnOccupy();
                }

                inventoryItemHandler.EquipItem(selectedItemUI.GetItem);
                equippedSlot.Occupy(selectedItemUI.GetItem);
                itemUIDic.Remove(selectedItemUI.GetItem.GetItemData().instanceID);
                Destroy(selectedItemUI.gameObject);
                selectedItemUI = null;
            }
            itemInfoDisplayUI.Close();
        }

        public void UnEquipItem()
        {
            if (selectedEquippedSlot != null)
            {
                inventoryItemHandler.UnEquipItem(selectedEquippedSlot.GetItem);
                SpawnItemUI(selectedEquippedSlot.GetItem);
                selectedEquippedSlot.UnOccupy();
                selectedEquippedSlot = null;
            }
            itemInfoDisplayUI.Close();
        }

        public void SpawnItemUI(List<Item> items)
        {
            foreach (Item item in items)
            {
                SpawnItemUI (item);
            }
        }

        public void SpawnItemUI(Item item)
        {
            ItemUI itemUI = Instantiate(itemUIPrefab, itemHolder);
            switch (item.itemSO.itemType)
            {
                case ItemType.Equipment:
                    itemUI.SetItem(item, inventoryItemHandler.GetPalette(item.GetItemSO<EquipmentSO>().rarity));
                    break;
                case ItemType.Material:
                    itemUI.SetItem(item, inventoryItemHandler.GetPalette(Rarity.Common));
                    break;
                default:
                    break;
            }

            itemUI.OnSelectItem += (itemUI) =>
            {
                selectedItemUI = itemUI;
                ItemSelected(itemUI.GetItem);
            };

            itemUIDic.Add(item.GetItemData().instanceID, itemUI);
        }

        private void ItemSelected(Item item)
        {
            switch (item.itemSO.itemType)
            {
                case ItemType.Equipment:    
                    itemInfoDisplayUI.Display(item);
                    selectedItem = item;
                    break;
                case ItemType.Material:
                    break;
            }
        }

        public void UpdateItemUI(Item item)
        {
            if (itemUIDic.ContainsKey(item.GetItemData().instanceID))
            {
                itemUIDic[item.GetItemData().instanceID].SetItemInfo();
            }
            else
            {
                GetEquippedSlot (item)?.SetItemInfo();
            }
        }

        private EquippedSlot GetEquippedSlot(Item item)
        {
            foreach (EquippedSlot slot in equippedSlots)
            {
                if (slot.GetItem == item)
                {
                    return slot;
                }
            }
            return null;
        }

        private EquippedSlot GetEquipmentSlot(EquipmentType equipmentType)
        {
            int duplicateCount = 0;
            foreach (EquippedSlot slot in equippedSlots)
            {
                if (slot.GetEquipmentType == equipmentType)
                {
                    if (equipmentType == EquipmentType.Ring)
                    {
                        if (!slot.IsOccupied || duplicateCount == 1)
                        {
                            return slot;
                        }
                        else
                        {
                            duplicateCount++;
                        }
                    }
                    else
                    {
                        return slot;
                    }
       
                }
            }
            return null;
        }
    }
}
