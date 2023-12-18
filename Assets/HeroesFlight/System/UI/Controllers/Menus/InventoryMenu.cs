using HeroesFlight.Common.Enum;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pelumi.Juicer;
using HeroesFlight.Common.Progression;
using HeroesFlight.System.UI.Inventory_Menu;
using Pelumi.ObjectPool;

namespace UISystem
{
    public class InventoryMenu : BaseMenu<InventoryMenu>
    {
        public event Func<StatModel> GetStatModel;
        public event Func<CharacterSO> GetSelectedCharacterSO;
        public event Action OnChangeHeroButtonClicked;
        public event Action OnStatPointButtonClicked;
        public event Action<EquipmentEntryUi> OnUpgradeRequest;
        public event Action<EquipmentEntryUi> OnDismantleRequest;
        public event Action<EquipmentEntryUi> OnEquipItemRequest;
        public event Action<EquipmentEntryUi> OnUnEquipItemRequest;

        [Header("Buttons")] 
        [SerializeField] private AdvanceButton changeHeroButton;
        [SerializeField] private AdvanceButton statPointButton;
        [SerializeField] private AdvanceButton quitButton;
        [SerializeField] private AdvanceButton viewEquipment;
        [SerializeField] private GameObject equipmentSelectVisual;
        [SerializeField] private AdvanceButton viewMaterials;
        [SerializeField] private GameObject materialSelectVisual;

        [Header("Texts")] [SerializeField] private TextMeshProUGUI currentAtk;
        [SerializeField] private TextMeshProUGUI currentHp;
        [SerializeField] private TextMeshProUGUI currentDef;

        [Header("Data")] [SerializeField] private UiSpineViewController uiSpineViewController;

        [Header("Inventory")] 
        [SerializeField] private ItemUI itemUIPrefab;
        [SerializeField] private Transform equipmentHolder;
        [SerializeField] private Transform equipmentHolderContent;
        [SerializeField] private Transform materialHolder;
        [SerializeField] private Transform materialHolderContent;
        [SerializeField] private ItemInfoDisplayUI itemInfoDisplayUI;
        [SerializeField] private EquippedSlot[] equippedSlots;

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        [Header("Debug")] private Dictionary<string, ItemUI> itemUIDic = new Dictionary<string, ItemUI>();
        [SerializeField] private ItemUI selectedItemUI;
        [SerializeField] private EquippedSlot selectedEquippedSlot;
        [SerializeField] EquipmentEntryUi selectedItem;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            changeHeroButton.onClick.AddListener(() => OnChangeHeroButtonClicked?.Invoke());
            statPointButton.onClick.AddListener(() => OnStatPointButtonClicked?.Invoke());
            quitButton.onClick.AddListener(Close);

            viewEquipment.onClick.AddListener(() =>
            {
                equipmentHolder.gameObject.SetActive(true);
                equipmentSelectVisual.SetActive(true);

                materialHolder.gameObject.SetActive(false);
                materialSelectVisual.SetActive(false);
            });

            viewMaterials.onClick.AddListener(() =>
            {
                materialHolder.gameObject.SetActive(true);
                materialSelectVisual.SetActive(true);

                equipmentHolder.gameObject.SetActive(false);
                equipmentSelectVisual.SetActive(false);
            });
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
            OnStatValueChanged(GetStatModel.Invoke());
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

        public void InitInventory(InventoryDataConverterInterface converter)
        {
            itemInfoDisplayUI.OnEquipAction += EquipItem;
            itemInfoDisplayUI.OnDismantleAction += () =>
            {
                OnDismantleRequest?.Invoke(selectedItem as EquipmentEntryUi);
            };
            itemInfoDisplayUI.OnUpgradeRequest += () => { OnUpgradeRequest?.Invoke(selectedItem as EquipmentEntryUi); };

            itemInfoDisplayUI.OnUnequipAction += UnEquipItem;

            itemInfoDisplayUI.Init(converter);

            foreach (EquippedSlot slot in equippedSlots)
            {
                slot.OnSelectItem += (item) =>
                {
                    selectedEquippedSlot = slot;
                    ItemSelected(item);
                };
            }
        }

        public void ClearInventoryItems()
        {
            foreach (var slot in equippedSlots)
            {
                slot.UnOccupy();
            }
            foreach (ItemUI itemUI in itemUIDic.Values)
            {
                ObjectPoolManager.ReleaseObject(itemUI);
            }

            itemUIDic.Clear();
        }

        public void UpdateInventoryView(List<EquipmentEntryUi> equipementItems, List<InventoryItemUiEntry> materials)
        {
            ClearInventoryItems();

            foreach (var item in equipementItems)
            {
                if (item.IsEquipped)
                {
                    EquippedSlot equippedSlot = GetEquipmentSlot(item.EquipmentType);
                    if (equippedSlot != null)
                    {
                        equippedSlot.Occupy(item, item.RarityPallete);
                    }
                }
                else
                {
                    SpawnItemUI(item);
                }

                if(selectedItem != null && selectedItem.ID == item.ID)
                {
                    selectedItem = item;
                }
            }

            foreach (var item in materials)
            {
                SpawnItemUI(item);
            }
        }

        /// <summary>
        /// Inventory systemn should do it
        /// </summary>
        /// <returns></returns>
        public void TryUpgradeItem()
        {
            if (selectedItem != null)
            {
                OnUpgradeRequest?.Invoke(selectedItem as EquipmentEntryUi);
            }
        }

        public void UpgradeItem()
        {
            if (selectedItem != null)
            {
                itemInfoDisplayUI.UpdateItemInfo(selectedItem);
            }       
        }

        /// <summary>
        /// Inventory systemn should do it
        /// </summary>
        /// <returns></returns>
        public void DismantleItem()
        {
            if (selectedItem != null)
            {
                itemUIDic.Remove(selectedItem.ID);
                selectedItem = null;
            }

            if (selectedItemUI != null)
            {
                ObjectPoolManager.ReleaseObject(selectedItemUI);

                selectedItemUI = null;
            }

            if (selectedEquippedSlot != null)
            {
                selectedEquippedSlot.UnOccupy();
                selectedItemUI = null;
            }

            itemInfoDisplayUI.Close();
        }

        /// <summary>
        /// Inventory systemn should do it
        /// </summary>
        /// <returns></returns>
        public void EquipItem()
        {
            //Debug.Log("Equipping item Start+ " + selectedItemUI.GetItem.ID);

            EquipmentEntryUi equipmentEntryUi = selectedItemUI.GetItem as EquipmentEntryUi;
            EquippedSlot equippedSlot = GetEquipmentSlot(equipmentEntryUi.EquipmentType);
        
            if (equippedSlot != null)
            {
                if (equippedSlot.IsOccupied)
                {
                    OnUnEquipItemRequest?.Invoke(equippedSlot.GetItem as EquipmentEntryUi);
                }

                OnEquipItemRequest?.Invoke(equipmentEntryUi);
                selectedItemUI = null;
            }

            itemInfoDisplayUI.Close();
            selectedItem = null;
        }

        /// <summary>
        /// Inventory systemn should do it
        /// </summary>
        /// <returns></returns>
        public void UnEquipItem()
        {
            if (selectedEquippedSlot != null)
            {
                OnUnEquipItemRequest?.Invoke(selectedEquippedSlot.GetItem as EquipmentEntryUi);
                selectedEquippedSlot.UnOccupy();
                selectedEquippedSlot = null;
            }

            itemInfoDisplayUI.Close();
            selectedItem = null;
        }

        public void SpawnItemUI(List<InventoryItemUiEntry> items)
        {
            foreach (InventoryItemUiEntry item in items)
            {
                SpawnItemUI(item);
            }
        }

        public void SpawnItemUI(InventoryItemUiEntry item)
        {
            Transform holder = item.ItemType == ItemType.Equipment ? equipmentHolderContent  : materialHolderContent;
            ItemUI itemUI = ObjectPoolManager.SpawnObject(itemUIPrefab, holder, PoolType.UI);
            itemUI.SetItem(item, item.RarityPallete);
            itemUI.OnSelectItem = OnItemSelected;
            itemUIDic.Add(item.ID, itemUI);
        }

        public void OnItemSelected(ItemUI itemUI)
        {
            selectedItemUI = itemUI;
            ItemSelected(itemUI.GetItem as EquipmentEntryUi);
        }

        private void ItemSelected(InventoryItemUiEntry item)
        {
            if(item==null)
                return;
            
            switch (item.ItemType)
            {
                case ItemType.Equipment:
                  
                    itemInfoDisplayUI.Display(item as EquipmentEntryUi);
                    selectedItem = item as EquipmentEntryUi;
                    break;
                case ItemType.Material:
                    break;
            }
        }

        public void UpdateItemUI(InventoryItemUiEntry item)
        {
            string id = "";
            switch (item.ItemType)
            {
                case ItemType.Equipment:
                    id = item.ID;
                    break;
                case ItemType.Material:
                    id = item.ID;
                    break;
            }

            if (itemUIDic.ContainsKey(id))
            {
                itemUIDic[id].SetItemInfo();
            }
            else
            {
                GetEquippedSlot(item)?.SetItemInfo();
            }
        }

        private EquippedSlot GetEquippedSlot(InventoryItemUiEntry item)
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