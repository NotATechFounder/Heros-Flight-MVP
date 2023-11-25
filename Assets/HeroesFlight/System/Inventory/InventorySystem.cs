using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour, IInventoryItemHandler
{
    public event Action<Item> OnItemAdded;
    public event Action<Item> OnItemModified;

    [SerializeField] private ItemInventorySO mainItemInventorySO;
    [SerializeField] private ItemDatabaseSO itemDatabaseSO;
    [SerializeField] private Dictionary<string, Item> eqquipmentItemDic = new Dictionary<string, Item>();
    [SerializeField] private Dictionary<string, Item> materialItemDic = new Dictionary<string, Item>();

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

        switch (itemSO.itemType)
        {
            case ItemType.Material:

                if (materialItemDic.ContainsKey(itemData.ID))
                {
                    materialItemDic[itemData.ID].ItemData().value = itemData.value;
                    OnItemModified?.Invoke(materialItemDic[itemData.ID]);
                }
                else
                {
                    materialItemDic.Add(itemData.ID, new Item(itemSO, itemData));
                    OnItemAdded?.Invoke(materialItemDic[itemData.ID]);
                }

                return materialItemDic[itemData.ID];

            case ItemType.Equipment:
                eqquipmentItemDic.Add(itemData.instanceID, new Item(itemSO, itemData));
                OnItemAdded?.Invoke(eqquipmentItemDic[itemData.instanceID]);
                return eqquipmentItemDic[itemData.instanceID];
        }
        return null;
    }

    public void RemoveFromInventory(Item item)
    {   
        switch (item.itemSO.itemType)
        {
            case ItemType.Equipment:
                eqquipmentItemDic.Remove(item.ItemData().instanceID);
                break;
            case ItemType.Material:
                materialItemDic.Remove(item.ItemData().ID);
                break;
            default:  break;
        }

        mainItemInventorySO.RemoveItemFromInventory(item.itemSO, item.ItemData());
    }

    public void LoadInventoryItems()
    {
        mainItemInventorySO.Load();

        foreach (ItemData itemData in mainItemInventorySO.inventoryData.savedData)
        {
            ItemSO itemSO = itemDatabaseSO.GetItemSOByID(itemData.ID);

            switch (itemSO.itemType)
            {
                case ItemType.Equipment:

                    eqquipmentItemDic.Add(itemData.instanceID, new Item(itemSO, itemData));
                    break;
                case ItemType.Material:

                    materialItemDic.Add(itemData.ID, new Item(itemSO, itemData));
                    break;
                default: break;
            }
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
        itemDatabaseSO.SetItemBuffStat(item);
        OnItemModified?.Invoke(item);
        mainItemInventorySO.Save();
        return true;
    }

    public void SaveInventoryItems()
    {
        mainItemInventorySO.Save();
    }

    public List<Item> GetInventoryEquippmentItems()
    {
        return new List<Item>(eqquipmentItemDic.Values);
    }

    public bool GetMaterialItemByID(string id, out Item item)
    {
        return materialItemDic.TryGetValue(id, out item);
    }

    public List<Item> GetInventoryMaterialItems()
    {
        return new List<Item>(materialItemDic.Values);
    }
}
