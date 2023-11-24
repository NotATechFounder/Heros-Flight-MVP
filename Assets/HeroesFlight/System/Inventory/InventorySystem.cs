using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public event Action<Item> OnItemAdded;
    public event Action<Item> OnItemModified;

    [SerializeField] private ItemInventorySO mainItemInventorySO;
    [SerializeField] private ItemDatabaseSO itemDatabaseSO;
    [SerializeField] private Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();

    [Header("Test Item")]
    [SerializeField] private ItemSO[] testItem;
    [SerializeField] private ItemSO[] testMaterialItem;

    private void Start()
    {
        LoadInventoryItems();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (ItemSO item in testItem)
            {
                AddToInventory(item);
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            foreach (ItemSO item in testMaterialItem)
            {
                AddToInventory(item);
            }
        }
    }

    public Item AddToInventory(ItemSO itemSO, int level = 1)
    {
        ItemData itemData = mainItemInventorySO.AddToItemInventory(itemSO, level);
        if (itemDictionary.ContainsKey(itemData.instanceID))
        {
            itemDictionary[itemData.instanceID].ItemData().value = itemData.value;
            OnItemModified?.Invoke(itemDictionary[itemData.instanceID]);
        }
        else
        {
            itemDictionary.Add(itemData.instanceID, new Item(itemSO, itemData));
            OnItemAdded?.Invoke(itemDictionary[itemData.instanceID]);    
        }
        return itemDictionary[itemData.instanceID];
    }

    public void RemoveFromInventory(Item item)
    {
        itemDictionary.Remove(item.ItemData().instanceID);
        mainItemInventorySO.RemoveItemFromInventory(item.itemSO, item.ItemData());
    }

    public void LoadInventoryItems()
    {
        mainItemInventorySO.Load();

        foreach (ItemData itemData in mainItemInventorySO.inventoryData.savedData)
        {
            itemDictionary.Add(itemData.instanceID, new Item(itemDatabaseSO.GetItemSOByID(itemData.ID), itemData));
        }
    }

    public void EquipItem(Item item)
    {
        item.ItemData().eqquiped = true;
        mainItemInventorySO.Save();
    }

    public void UnEquipItem(Item item)
    {
        item.ItemData().eqquiped = false;
        mainItemInventorySO.Save();
    }

    public void DismantleItem(Item item)
    {
        RemoveFromInventory(item);
    }

    public bool TryUpgradeItem(Item item)
    {
        // check if the level is not maxed and if they have enough materials
        item.LevelUp();
        OnItemModified?.Invoke(item);
        mainItemInventorySO.Save();
        return true;
    }

    public void SaveInventoryItems()
    {
        mainItemInventorySO.Save();
    }

    public List<Item> GetInventoryItems()
    {
        return new List<Item>(itemDictionary.Values);
    }
}
