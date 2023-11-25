using System;
using System.Collections.Generic;

public interface IInventoryItemHandler
{
    event Action<Item> OnItemAdded;
    event Action<Item> OnItemModified;

    void DismantleItem(Item item);
    void EquipItem(Item item);
    List<Item> GetInventoryEquippmentItems();
    List<Item> GetInventoryMaterialItems();
    bool TryUpgradeItem(Item item);
    void UnEquipItem(Item item);
    bool GetMaterialItemByID(string id, out Item item);
}