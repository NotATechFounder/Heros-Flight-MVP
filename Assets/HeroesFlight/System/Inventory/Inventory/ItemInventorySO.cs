using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemInventoryS0", menuName = "Inventory System/ItemInventoryS0", order = 1)]
public class ItemInventorySO : InventorySO <ItemData>
{
    public event Action<ItemData> OnItemModified;

    public ItemData AddToItemInventory(ItemSO itemSO, int amount = 1)
    {
        ItemData itemData = null;
        switch (itemSO.itemType)
        {
            case ItemType.Material:

                itemData = GetFreeMaterialDataByID(itemSO);
                MaterialObject materialObject = (MaterialObject)itemSO;

                if (itemData == null || itemData.value >= materialObject.maxStackAmount)
                {
                    itemData = new ItemData(itemSO, amount);
                    AddToInventory(itemData);
                    return itemData;
                }
                else
                {
                    itemData.value += amount;
                    OnItemModified?.Invoke(itemData);
                    Save();
                }
                return itemData;

            case ItemType.Equipment:
                itemData = new ItemData(itemSO, amount);
                AddToInventory(itemData);
                return itemData;
        }
        return itemData;
    }

    public void RemoveItemFromInventory(Item item, int amount = 1)
    {
        RemoveItemFromInventory (item.itemSO, item.ItemData(), amount);
    }

    public void RemoveItemFromInventory(ItemSO itemSO, ItemData itemData, int amount = 1)
    {
        RemoveItemFromInventory (itemSO.itemType, itemData, amount);
    }

    public void RemoveItemFromInventory(ItemType itemType, ItemData itemData, int amount = 1)
    {
        switch (itemType)
        {
            case ItemType.Material:
                itemData.value -= amount;
                if (itemData.value <= 0) RemoveFromInventory(itemData);
                else
                {
                    OnItemModified?.Invoke(itemData);
                    Save();
                }
                break;
            case ItemType.Equipment:
                RemoveFromInventory(itemData);
                break;
        }
    }

    public ItemData GetItemDataByID (string id)
    {
        return inventoryData.savedData.FirstOrDefault(x => x.ID == id);
    }

    public ItemData GetFreeMaterialDataByID(ItemSO itemSO)
    {
        string id = itemSO.ID;
        return inventoryData.savedData.FirstOrDefault(x => x.ID == id && x.value < ((MaterialObject)itemSO).maxStackAmount);
    }

    public ItemData GetItemDataByInstanceID(string id)
    {
        return inventoryData.savedData.FirstOrDefault(x => x.instanceID == id);
    }
}
