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
    [SerializeField] private CurrencyManager currencyManager;

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

    public void Init(CurrencyManager currencyManager)
    {
        this.currencyManager = currencyManager;
    }

    public Item AddToInventory(ItemSO itemSO, int level = 1)
    {
        ItemData itemData = mainItemInventorySO.AddToItemInventory(itemSO, level);

        switch (itemSO.itemType)
        {
            case ItemType.Material:

                if (materialItemDic.ContainsKey(itemData.ID))
                {
                    materialItemDic[itemData.ID].GetItemData<ItemMaterialData>().value = itemData.value;
                    OnItemModified?.Invoke(materialItemDic[itemData.ID]);
                }
                else
                {
                    materialItemDic.Add(itemData.ID, new Item(itemSO, itemData));
                    OnItemAdded?.Invoke(materialItemDic[itemData.ID]);
                }

                return materialItemDic[itemData.ID];

            case ItemType.Equipment:
                ItemEquipmentData itemEquipmentData = itemData as ItemEquipmentData;
                eqquipmentItemDic.Add(itemEquipmentData.instanceID, new Item(itemSO, itemData));
                OnItemAdded?.Invoke(eqquipmentItemDic[itemEquipmentData.instanceID]);
                return eqquipmentItemDic[itemEquipmentData.instanceID];
        }
        return null;
    }

    public void RemoveFromInventory(Item item)
    {   
        switch (item.itemSO.itemType)
        {
            case ItemType.Equipment:
                eqquipmentItemDic.Remove(item.GetItemData<ItemEquipmentData>().instanceID);
                break;
            case ItemType.Material:
                materialItemDic.Remove(item.GetItemData<ItemMaterialData>().ID);
                break;
            default:  break;
        }

        mainItemInventorySO.RemoveItemFromInventory(item.itemSO, item.GetItemData<ItemData>());
    }

    public void LoadInventoryItems()
    {
        mainItemInventorySO.Load();

        foreach (ItemData itemData in mainItemInventorySO.inventoryData.equipmentData)
        {
            ItemSO itemSO = itemDatabaseSO.GetItemSOByID(itemData.ID);
            ItemEquipmentData itemEquipmentData = itemData as ItemEquipmentData;
            eqquipmentItemDic.Add(itemEquipmentData.instanceID, new Item(itemSO, itemData));
        }

        foreach (ItemData itemData in mainItemInventorySO.inventoryData.materialData)
        {
            ItemSO itemSO = itemDatabaseSO.GetItemSOByID(itemData.ID);
            materialItemDic.Add(itemData.ID, new Item(itemSO, itemData));
        }
    }

    public void EquipItem(Item item)
    {
        item.GetItemData<ItemEquipmentData>().eqquiped = true;
        mainItemInventorySO.Save();
    }

    public void UnEquipItem(Item item)
    {
        item.GetItemData<ItemEquipmentData>().eqquiped = false;
        mainItemInventorySO.Save();
    }

    public void DismantleItem(Item item)
    {
        currencyManager.AddCurrency(CurrencyKeys.Gold, itemDatabaseSO.GetTotalUpgradeGoldCost (item.GetItemData<ItemEquipmentData>()));
        GetMaterialItemByID("M_" + item.GetItemSO<EquipmentSO>().equipmentType.ToString(), out Item materialItem);
        if (materialItem != null)
        {
            materialItem.GetItemData<ItemData>().value += itemDatabaseSO.GetTotalUpgradeMaterialCost(item.GetItemData<ItemData>());
            OnItemModified?.Invoke(materialItem);
        }
        else
        {
            ItemSO itemSO = GetItemSO("M_" + item.GetItemSO<EquipmentSO>().equipmentType.ToString());
            if (itemSO != null)
            {
               AddToInventory(itemSO, itemDatabaseSO.GetTotalUpgradeMaterialCost(item.GetItemData<ItemEquipmentData>()));
            }
        }

        RemoveFromInventory(item);
    }

    public bool TryUpgradeItem(Item item)
    {
        if (item.GetItemData<ItemEquipmentData>().value >= GetItemMaxLevel(item))
        {
            Debug.Log("Item is maxed");
            return false;
        }

        int goldCost = GetGoldUpgradeRequiredAmount(item);

        if (currencyManager.GetCurrencyAmount(CurrencyKeys.Gold) < goldCost)
        {
            Debug.Log("Not enough gold");
            return false;
        }

        GetMaterialItemByID("M_" + item.GetItemSO<EquipmentSO>().equipmentType.ToString(), out Item materialItem);

        if (materialItem == null)
        {
            Debug.Log("Not enough material");
            return false;
        }

        if (materialItem.GetItemData<ItemMaterialData>().value < GetMaterialUpgradeRequiredAmount(item))
        {
            Debug.Log("Not enough material");
            return false;
        }

        currencyManager.ReduceCurency(CurrencyKeys.Gold, goldCost);
        materialItem.GetItemData<ItemMaterialData>().value -= GetMaterialUpgradeRequiredAmount(item);
        item.LevelUp();
        itemDatabaseSO.SetItemBuffStat(item);
        OnItemModified?.Invoke(item);
        mainItemInventorySO.Save();
        return true;
    }

    public int GetItemMaxLevel(Item item)
    {
        return itemDatabaseSO.GetItemMaxLevel(item);
    }

    public int GetGoldUpgradeRequiredAmount(Item item)
    {
        return itemDatabaseSO.GetUpgradeGoldCost(item.GetItemData<ItemData>());
    }

    public int GetMaterialUpgradeRequiredAmount(Item item)
    {
        return itemDatabaseSO.GetUpgradeMaterialCost(item.GetItemData<ItemData>());
    }

    public ItemSO GetItemSO(string id)
    {
        return itemDatabaseSO.GetItemSOByID(id);
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

    public RarityPalette GetPalette(Rarity rarity)
    {
        return itemDatabaseSO.GetRarityPalette(rarity);
    }

    public int GetTotalUpgradeGoldSpent(ItemData itemData)
    {
        return itemDatabaseSO.GetTotalUpgradeGoldCost(itemData);
    }

    public int GetTotalUpgradeMaterialSpent(ItemData itemData)
    {
        return itemDatabaseSO.GetTotalUpgradeMaterialCost(itemData);
    }
}
