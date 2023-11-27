using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ItemInventoryData : IInventoryData<ItemBaseData>
{
    public List<ItemEquipmentData> equipmentData = new List<ItemEquipmentData>();
    public List<ItemMaterialData> materialData = new List<ItemMaterialData>();

    public int Count => equipmentData.Count + materialData.Count;

    public ItemBaseData GetByID(string id)
    {
        ItemBaseData itemBaseData = equipmentData.FirstOrDefault(x => x.ID == id);
        if (itemBaseData == null) itemBaseData = materialData.FirstOrDefault(x => x.ID == id);
        return itemBaseData;
    }


    public T GetByID<T>(string id) where T : ItemBaseData
    {
        if (typeof(T) == typeof(ItemEquipmentData))
        {
            return equipmentData.FirstOrDefault(x => x.ID == id) as T;
        }
        else if (typeof(T) == typeof(ItemMaterialData))
        {
            return materialData.FirstOrDefault(x => x.ID == id) as T;
        }
        return null;
    }

    public void Add(ItemBaseData itemBaseData)
    {
        switch (itemBaseData)
        {
            case ItemEquipmentData:
                equipmentData.Add(itemBaseData as ItemEquipmentData);
                break;
            case ItemMaterialData:
                materialData.Add(itemBaseData as ItemMaterialData);
                break;
        }
    }
    public void Remove(ItemBaseData itemBaseData)
    {
        switch (itemBaseData)
        {
            case ItemEquipmentData:
                equipmentData.Remove(itemBaseData as ItemEquipmentData);
                break;
            case ItemMaterialData:
                materialData.Remove(itemBaseData as ItemMaterialData);
                break;
        }
    }

    public bool Contains(ItemBaseData itemBaseData)
    {
        switch (itemBaseData)
        {
            case ItemEquipmentData:
                return equipmentData.Contains(itemBaseData as ItemEquipmentData);
            case ItemMaterialData:
                return materialData.Contains(itemBaseData as ItemMaterialData);
        }
        return false;
    }

    public void Clear()
    {
        equipmentData.Clear();
        materialData.Clear();
    }
}

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

                itemData = GetItemDataByID(itemSO.ID);
                MaterialObject materialObject = (MaterialObject)itemSO;
                if (itemData == null)
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
        RemoveItemFromInventory (item.itemSO, item.GetItemData(), amount);
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

    public ItemData GetItemDataByInstanceID(string instanceID)
    {
        return inventoryData.savedData.FirstOrDefault(x => x.instanceID == instanceID);
    }
}
