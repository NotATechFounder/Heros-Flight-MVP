using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeroesFlight.Common.Enum;
using UnityEngine;


[Serializable]
public class InventoryItemData : InventoryData<ItemData>
{
    public List<ItemEquipmentData> equipmentData = new List<ItemEquipmentData>();
    public List<ItemMaterialData> materialData = new List<ItemMaterialData>();

    public override int Count => equipmentData.Count + materialData.Count;

    public T GetByID<T>(string id) where T : ItemData
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

    public ItemEquipmentData GetItemEquipmentByUniqueID(string id)
    {
        ItemEquipmentData itemEquipmentData = equipmentData.FirstOrDefault(x => x.instanceID == id);
        return itemEquipmentData;
    }
    
    public override void Add(ItemData itemBaseData)
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
    public override void Remove(ItemData itemBaseData)
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

    public override bool Contains(ItemData itemBaseData)
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

    public override void Clear()
    {
        equipmentData.Clear();
        materialData.Clear();
    }
}

[CreateAssetMenu(fileName = "ItemInventoryS0", menuName = "Inventory System/ItemInventoryS0", order = 1)]
public class ItemInventorySO : InventorySO <InventoryItemData,ItemData>
{
    public event Action<ItemData> OnItemModified;

    public ItemEquipmentData AddEquipmentToInventory(ItemSO itemSO, int level = 1, Rarity rarity = Rarity.Common)
    {
      return  AddToItemInventory (itemSO, level, rarity) as ItemEquipmentData;
    }

    public ItemMaterialData AddMaterialToInventory(ItemSO itemSO, int amount = 1)
    {
        return AddToItemInventory(itemSO, amount) as ItemMaterialData;
    }

    private ItemData AddToItemInventory(ItemSO itemSO, int amount = 1, Rarity rarity = Rarity.Common)
    {
        ItemData itemData = null;
        switch (itemSO.itemType)
        {
            case ItemType.Material:

                itemData = GetMaterialItemByID(itemSO.ID);
                MaterialObject materialObject = (MaterialObject)itemSO;
                if (itemData == null)
                {
                    itemData = new ItemMaterialData(itemSO, amount);
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
                itemData = new ItemEquipmentData(itemSO, amount, rarity);
                AddToInventory(itemData);
                return itemData;
        }
        return itemData;
    }

    public void RemoveItemFromInventory(Item item, int amount = 1)
    {
        RemoveItemFromInventory (item.itemSO, item.GetItemData<ItemEquipmentData>(), amount);
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
        return inventoryData.GetByID<ItemData>(id);
    }

    public ItemMaterialData GetMaterialItemByID(string id)
    {
        return inventoryData.GetByID<ItemMaterialData>(id);
    }

    public ItemEquipmentData GetEquipmentItemByID(string id)
    {
        return inventoryData.GetByID<ItemEquipmentData>(id);
    }

    public ItemEquipmentData GetEquipmentItemByInstanceID(string instanceID)
    {
        return inventoryData.GetItemEquipmentByUniqueID(instanceID);
    }
}
