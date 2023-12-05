namespace  HeroesFlight.System.UI.Inventory_Menu
{
    public interface InventoryDataConverterInterface
    {
        int GetMaxItemLvl(EquipmentEntryUi targetEntry);
        int GetGoldAmount(EquipmentEntryUi targetEntry);
        InventoryItemUiEntry GetMaterial(string id);
        InventoryItemUiEntry GetEquipment(string id);
        int GetMaterialAmount(EquipmentEntryUi equipmentEntryUi);
        int GetMaterialSpentAmount(EquipmentEntryUi equipmentEntryUi);
        int GetGoldSpentAmount(EquipmentEntryUi equipmentEntryUi);
    }
}