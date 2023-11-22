using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private ItemInventorySO mainItemInventorySO;
    [SerializeField] private ItemDatabaseSO itemDatabaseSO;
    [SerializeField] private Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();

    [Header("Test Item")]
    [SerializeField] private ItemSO testItem;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddToInventory(testItem);
        }
    }

    public Item AddToInventory(ItemSO itemSO, int level = 1)
    {
        ItemData itemData = mainItemInventorySO.AddToItemInventory(itemSO, level);
        if (itemDictionary.ContainsKey(itemData.instanceID))
        {
            itemDictionary[itemData.instanceID].ItemData().value = itemData.value;
        }
        else
        {
            itemDictionary.Add(itemData.instanceID, new Item(itemSO, itemData));
        }
        return itemDictionary[itemData.instanceID];
    }

    public void RemoveFromInventory(Item item)
    {
        itemDictionary.Remove(item.ItemData().instanceID);
        mainItemInventorySO.RemoveItemFromInventory(item.itemObject, item.ItemData());
    }

    public void LoadInventoryItems()
    {
        mainItemInventorySO.Load();

        foreach (ItemData itemData in mainItemInventorySO.inventoryData.savedData)
        {
            itemDictionary.Add(itemData.instanceID, new Item(itemDatabaseSO.GetItemSOByID(itemData.ID), itemData));
        }
    }

    public void SaveInventoryItems()
    {
        mainItemInventorySO.Save();
    }
}
