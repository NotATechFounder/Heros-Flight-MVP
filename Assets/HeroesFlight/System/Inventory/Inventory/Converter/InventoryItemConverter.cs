using HeroesFlight.Common.Enum;
using HeroesFlight.System.UI.Inventory_Menu;
using System.Collections.Generic;

namespace HeroesFlight.System.Inventory.Inventory.Converter
{
    public class InventoryItemConverter : InventoryDataConverterInterface
    {
        public InventoryItemConverter(InventoryHandler inventoryHandler)
        {
            handler = inventoryHandler;
        }

        private InventoryHandler handler;

        public int GetMaxItemLvl(EquipmentEntryUi targetEntry)
        {
            return handler.GetItemMaxLevel(handler.GetEqupItemById(targetEntry.ID));
        }

        public int GetGoldAmount(EquipmentEntryUi targetEntry)
        {
            return handler.GetGoldUpgradeRequiredAmount(handler.GetEqupItemById(targetEntry.ID));
        }

        public InventoryItemUiEntry GetMaterial(string id)
        {
            handler.GetMaterialItemByID(id,out var item);
            if (item == null)
                return null;
            
            var itemData = item.GetItemData<ItemMaterialData>();
            return new InventoryItemUiEntry(item.itemSO.ID,
                item.itemSO.icon, itemData.value,
                item.itemSO.itemType, handler.GetPalette(Rarity.Common),
                item.itemSO.Name, item.itemSO.description);
        }

        public InventoryItemUiEntry GetEquipment(string id)
        {
            var item= handler.GetEqupItemById(id);
            var equipmentData = item.GetItemData<ItemEquipmentData>();
            List<ItemEffectEntryUi> itemEffectEntryUis = new List<ItemEffectEntryUi>();
            EquipmentSO equipmentSO = item.GetItemSO<EquipmentSO>();

            itemEffectEntryUis.Add(new ItemEffectEntryUi(equipmentSO.specialHeroEffect.statType.ToString(), equipmentSO.specialHeroEffect.value,0, equipmentData.rarity, handler.GetPalette(equipmentData.rarity)));
            foreach (var effect in equipmentSO.uniqueStatModificationEffects)
            {
                itemEffectEntryUis.Add(new ItemEffectEntryUi(effect.statType.ToString(), effect.curve.GetCurrentValueInt(equipmentData.value), effect.curve.GetCurrentValueInt(equipmentData.value + 1), effect.rarity, handler.GetPalette(effect.rarity)));
            }

            foreach (var effect in equipmentSO.uniqueCombatEffects)
            {
                itemEffectEntryUis.Add(new ItemEffectEntryUi(effect.combatEffect.EffectToApply[0].name, effect.curve.GetCurrentValueInt(equipmentData.value), effect.curve.GetCurrentValueInt(equipmentData.value + 1), effect.rarity, handler.GetPalette(effect.rarity)));
            }
        
            return new EquipmentEntryUi(equipmentData.instanceID, item.itemSO.icon,
                equipmentData.value,
                item.itemSO.itemType, handler.GetPalette(equipmentData.rarity),
                item.itemSO.Name, item.itemSO.description,
                (item.itemSO as EquipmentSO).equipmentType,
                equipmentData.eqquiped, equipmentData.rarity, itemEffectEntryUis);
        }

        public int GetMaterialAmount(EquipmentEntryUi targetEntry)
        {
            return handler.GetMaterialUpgradeRequiredAmount(handler.GetEqupItemById(targetEntry.ID));
        }

        public int GetMaterialSpentAmount(EquipmentEntryUi equipmentEntryUi)
        {
            var item= handler.GetEqupItemById(equipmentEntryUi.ID);
            return handler.GetTotalUpgradeMaterialSpent(item.GetItemData<ItemEquipmentData>());
        }

        public int GetGoldSpentAmount(EquipmentEntryUi equipmentEntryUi)
        {
            var item= handler.GetEqupItemById(equipmentEntryUi.ID);
            return handler.GetTotalUpgradeGoldSpent(item.GetItemData<ItemEquipmentData>());
        }
    }
}