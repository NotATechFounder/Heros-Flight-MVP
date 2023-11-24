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

        public event Func <List<Item>> GetInventoryItems;
        public event Action<Item> OnItemEquipped;
        public event Action<Item> OnItemUnEquipped;
        public event Action<Item> OnItemDismantled;
        public event Func<Item, bool> OnItemUpgraded;

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

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            changeHeroButton.onClick.AddListener(() => OnChangeHeroButtonClicked?.Invoke());
            statPointButton.onClick.AddListener(() => OnStatPointButtonClicked?.Invoke());
            quitButton.onClick.AddListener(Close);

            InitInventoryUI();
        }

        public override void OnOpened()
        {
            canvasGroup.alpha = 0;
            openEffectBG.Start();
            UpdateCharacter(GetSelectedCharacterSO.Invoke());
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

        public void InitInventoryUI()
        {
            itemInfoDisplayUI.OnEquipAction += EquipItem;
            itemInfoDisplayUI.OnDismantleAction += DismantleItem;
            itemInfoDisplayUI.OnUpgradeAction += TryUpgradeItem;
            itemInfoDisplayUI.OnUnequipAction += UnEquipItem;

            foreach (EquippedSlot slot in equippedSlots)
            {
                slot.OnSelectItem += (item) =>
                {
                    selectedEquippedSlot = slot;
                    itemInfoDisplayUI.Display(item);
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

            foreach (Item item in GetInventoryItems?.Invoke() ?? new List<Item>())
            {
                if (item.ItemData().eqquiped)
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
        }

        private bool TryUpgradeItem()
        {
            if(OnItemUpgraded?.Invoke(selectedItemUI.GetItem) == true)
            {
                return true;
            }
            return false;
        }

        private void DismantleItem()
        {
            if (selectedItemUI != null)
            {
                OnItemDismantled?.Invoke(selectedItemUI.GetItem);
                itemUIDic.Remove(selectedItemUI.GetItem.ItemData().instanceID);
                Destroy(selectedItemUI.gameObject);
                selectedItemUI = null;
            }

            if (selectedEquippedSlot != null)
            {
                OnItemDismantled?.Invoke(selectedEquippedSlot.GetItem);
                itemUIDic.Remove(selectedEquippedSlot.GetItem.ItemData().instanceID);
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

                OnItemEquipped?.Invoke(selectedItemUI.GetItem);
                equippedSlot.Occupy(selectedItemUI.GetItem);
                itemUIDic.Remove(selectedItemUI.GetItem.ItemData().instanceID);
                Destroy(selectedItemUI.gameObject);
                selectedItemUI = null;
            }
            itemInfoDisplayUI.Close();
        }

        public void UnEquipItem()
        {
            if (selectedEquippedSlot != null)
            {  
                OnItemUnEquipped?.Invoke(selectedEquippedSlot.GetItem);
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
            itemUI.SetItem(item);

            itemUI.OnSelectItem += (itemUI) =>
            {
                selectedItemUI = itemUI;
                ItemSelected(itemUI.GetItem);
            };

            itemUIDic.Add(item.ItemData().instanceID, itemUI);
        }

        private void ItemSelected(Item item)
        {
            switch (item.itemSO.itemType)
            {
                case ItemType.Equipment:    
                    itemInfoDisplayUI.Display(item);
                    break;
                case ItemType.Material:
                    break;
            }
        }

        public void UpdateItemUI(Item item)
        {
            if (itemUIDic.ContainsKey(item.ItemData().instanceID))
            {
                itemUIDic[item.ItemData().instanceID].SetItemInfo();
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
