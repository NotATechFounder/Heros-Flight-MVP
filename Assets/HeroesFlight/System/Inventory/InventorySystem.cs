using System;
using System.Collections.Generic;
using HeroesFlight.Common.Enum;
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

        public void Init(Scene scene = default, Action onComplete = null)
        {
            InventoryHandler = scene.GetComponent<InventoryHandler>();
            InventoryHandler.Init(data.CurrencyManager);
            InventoryHandler.OnItemAdded += SpawnUiItem;
            InventoryHandler.OnItemModified += UpdateUiItem;
            uiSystem.UiEventHandler.MainMenu.OnInventoryButtonPressed += OpenInventory;
            uiSystem.UiEventHandler.InventoryMenu.OnEquipItemRequest += HandleItemEquipRequest;
            uiSystem.UiEventHandler.InventoryMenu.OnUpgradeRequest += HandleItemUpgradeRequest;
            uiSystem.UiEventHandler.InventoryMenu.OnDismantleRequest += HandleItemDismantleRequest;
            uiSystem.UiEventHandler.InventoryMenu.OnUnEquipItemRequest += HandleItemUnequipRequest;
            //TODO: pass methodes from InventoryHandler here
            //uiSystem.UiEventHandler.InventoryMenu.InitInventory();
        }

        private void HandleItemUnequipRequest(InventoryItemUiEntry obj)
        {
            if (InventoryHandler.TryUpgradeItem(obj))
            {
                uiSystem.UiEventHandler.InventoryMenu.UnEquipItem();
            }
        }

        private void HandleItemDismantleRequest(InventoryItemUiEntry obj)
        {
           InventoryHandler.DismantleItem(obj);
           uiSystem.UiEventHandler.InventoryMenu.DismantleItem();
        }

        private void HandleItemUpgradeRequest(InventoryItemUiEntry obj)
        {
            
        }

        private void HandleItemEquipRequest(InventoryItemUiEntry obj)
        {
            InventoryHandler.EquipItem(obj);
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
                var data = equipmentItem.GetItemData<ItemEquipmentData>();
                equipment.Add(new EquipmentEntryUi(equipmentItem.itemSO.ID, equipmentItem.itemSO.icon, data.value,
                    equipmentItem.itemSO.itemType, InventoryHandler.GetPalette(data.rarity),
                    equipmentItem.itemSO.Name, equipmentItem.itemSO.description,
                    (equipmentItem.itemSO as EquipmentSO).equipmentType,
                    data.eqquiped, data.rarity));
            }

            foreach (var equipmentItem in InventoryHandler.GetInventoryMaterialItems())
            {
                var data = equipmentItem.GetItemData<ItemMaterialData>();
                materials.Add(new InventoryItemUiEntry(equipmentItem.itemSO.ID, equipmentItem.itemSO.icon, data.value,
                    equipmentItem.itemSO.itemType, InventoryHandler.GetPalette(Rarity.Common),
                    equipmentItem.itemSO.Name, equipmentItem.itemSO.description));
            }

            uiSystem.UiEventHandler.InventoryMenu.LoadInventoryItems(equipment, materials);
        }

        private void UpdateUiItem(Item obj)
        {
            // uiSystem.UiEventHandler.InventoryMenu.UpdateItemUI(new InventoryItemUiEntry());
        }

        private void SpawnUiItem(Item obj)
        {
            //uiSystem.UiEventHandler.InventoryMenu.SpawnItemUI(new InventoryItemUiEntry());
        }

        public void Reset()
        {
            //TODO: reset callback if needed 
        }

        public InventoryHandler InventoryHandler { get; private set; }
    }
}