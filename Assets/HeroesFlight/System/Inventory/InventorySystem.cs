using System;
using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Inventory.Inventory.Converter;
using HeroesFlight.System.UI;
using HeroesFlight.System.UI.Inventory_Menu;
using StansAssets.Foundation.Extensions;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.Inventory
{
    public class InventorySystem : InventorySystemInterface

    {
        public InventorySystem(DataSystemInterface dataSystemInterface, IUISystem uiSystem)
        {
            data = dataSystemInterface;
            this.uiSystem = uiSystem;
        }

        private DataSystemInterface data;
        private IUISystem uiSystem;
        private InventoryItemConverter converter;

        public void Init(Scene scene = default, Action onComplete = null)
        {
            InventoryHandler = scene.GetComponent<InventoryHandler>();
            InventoryHandler.Init(data.CurrencyManager);
            InventoryHandler.OnItemAdded += SpawnUiItem;
            InventoryHandler.OnItemModified += UpdateUiItem;
            converter = new InventoryItemConverter(InventoryHandler);
        }

        public void Reset()
        {
            //TODO: reset callback if needed 
        }

        private void HandleItemUnequipRequest(InventoryItemUiEntry obj)
        {
            Item targetItem = InventoryHandler.GetEqupItemById(obj.ID);
            InventoryHandler.UnEquipItem(targetItem);
            uiSystem.UiEventHandler.InventoryMenu.UnEquipItem();
        }

        private void HandleItemDismantleRequest(InventoryItemUiEntry obj)
        {
            Item targetItem = InventoryHandler.GetEqupItemById(obj.ID);
            InventoryHandler.DismantleItem(targetItem);
            uiSystem.UiEventHandler.InventoryMenu.DismantleItem();
        }

        private void HandleItemUpgradeRequest(InventoryItemUiEntry obj)
        {
            Item targetItem = InventoryHandler.GetEqupItemById(obj.ID);
            if (InventoryHandler.TryUpgradeItem(targetItem))
            {
                uiSystem.UiEventHandler.InventoryMenu.UpgradeItem();
            }
        }

        private void HandleItemEquipRequest(InventoryItemUiEntry obj)
        {
            Item targetItem = InventoryHandler.GetEqupItemById(obj.ID);
            InventoryHandler.EquipItem(targetItem);
        }

        private void OpenInventory()
        {
            UpdateInventoryUi();
            uiSystem.UiEventHandler.InventoryMenu.Open();
        }

        private void UpdateInventoryUi()
        {
            List<InventoryItemUiEntry> materials = new();
            List<EquipmentEntryUi> equipment = new();
            foreach (var equipmentItem in InventoryHandler.GetInventoryEquippmentItems())
            {
                var equipmentData = equipmentItem.GetItemData<ItemEquipmentData>();
                equipment.Add(new EquipmentEntryUi(equipmentItem.itemSO.ID, equipmentItem.itemSO.icon,
                    equipmentData.value,
                    equipmentItem.itemSO.itemType, InventoryHandler.GetPalette(equipmentData.rarity),
                    equipmentItem.itemSO.Name, equipmentItem.itemSO.description,
                    (equipmentItem.itemSO as EquipmentSO).equipmentType,
                    equipmentData.eqquiped, equipmentData.rarity));
            }

            foreach (var equipmentItem in InventoryHandler.GetInventoryMaterialItems())
            {
                var itemData = equipmentItem.GetItemData<ItemMaterialData>();
                materials.Add(new InventoryItemUiEntry(equipmentItem.itemSO.ID, equipmentItem.itemSO.icon,
                    itemData.value,
                    equipmentItem.itemSO.itemType, InventoryHandler.GetPalette(Rarity.Common),
                    equipmentItem.itemSO.Name, equipmentItem.itemSO.description));
            }

            uiSystem.UiEventHandler.InventoryMenu.LoadInventoryItems(equipment, materials);
        }

        private void UpdateUiItem(Item obj)
        {
            if (obj.itemSO.itemType == ItemType.Material)
            {
                var itemData = obj.GetItemData<ItemMaterialData>();

                uiSystem.UiEventHandler.InventoryMenu.UpdateItemUI(new InventoryItemUiEntry(obj.itemSO.ID,
                    obj.itemSO.icon, itemData.value,
                    obj.itemSO.itemType, InventoryHandler.GetPalette(Rarity.Common),
                    obj.itemSO.Name, obj.itemSO.description));
            }
            else
            {
                var equipmentData = obj.GetItemData<ItemEquipmentData>();
                uiSystem.UiEventHandler.InventoryMenu.UpdateItemUI(new EquipmentEntryUi(obj.itemSO.ID, obj.itemSO.icon,
                    equipmentData.value,
                    obj.itemSO.itemType, InventoryHandler.GetPalette(equipmentData.rarity),
                    obj.itemSO.Name, obj.itemSO.description,
                    (obj.itemSO as EquipmentSO).equipmentType,
                    equipmentData.eqquiped, equipmentData.rarity));
            }
        }

        private void SpawnUiItem(Item obj)
        {
            if (obj.itemSO.itemType == ItemType.Material)
            {
                var itemData = obj.GetItemData<ItemMaterialData>();
                uiSystem.UiEventHandler.InventoryMenu.SpawnItemUI((new InventoryItemUiEntry(obj.itemSO.ID,
                    obj.itemSO.icon, itemData.value,
                    obj.itemSO.itemType, InventoryHandler.GetPalette(Rarity.Common),
                    obj.itemSO.Name, obj.itemSO.description)));
            }
            else
            {
                var equipmentData = obj.GetItemData<ItemEquipmentData>();
                uiSystem.UiEventHandler.InventoryMenu.SpawnItemUI(new EquipmentEntryUi(obj.itemSO.ID, obj.itemSO.icon,
                    equipmentData.value,
                    obj.itemSO.itemType, InventoryHandler.GetPalette(equipmentData.rarity),
                    obj.itemSO.Name, obj.itemSO.description,
                    (obj.itemSO as EquipmentSO).equipmentType,
                    equipmentData.eqquiped, equipmentData.rarity));
            }
        }

        public InventoryHandler InventoryHandler { get; private set; }

        public void UpdateUiConnections()
        {
            uiSystem.UiEventHandler.MainMenu.OnInventoryButtonPressed += OpenInventory;
            uiSystem.UiEventHandler.InventoryMenu.OnEquipItemRequest += HandleItemEquipRequest;
            uiSystem.UiEventHandler.InventoryMenu.OnUpgradeRequest += HandleItemUpgradeRequest;
            uiSystem.UiEventHandler.InventoryMenu.OnDismantleRequest += HandleItemDismantleRequest;
            uiSystem.UiEventHandler.InventoryMenu.OnUnEquipItemRequest += HandleItemUnequipRequest;

            //TODO: pass methodes from InventoryHandler here
            uiSystem.UiEventHandler.InventoryMenu.InitInventory(converter);
        }
        
        
    }
}