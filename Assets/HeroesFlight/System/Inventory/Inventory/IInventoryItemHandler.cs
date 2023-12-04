using System;
using System.Collections.Generic;
using HeroesFlight.Common;
using HeroesFlight.Common.Enum;

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
    RarityPalette GetPalette(Rarity rarity);
    int GetItemMaxLevel(Item item);
    int GetGoldUpgradeRequiredAmount(Item item);
    int GetMaterialUpgradeRequiredAmount(Item item);
    int GetTotalUpgradeGoldSpent(ItemEquipmentData itemData);
    int GetTotalUpgradeMaterialSpent(ItemEquipmentData itemData);
    ItemSO GetItemSO(string id);
}