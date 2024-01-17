using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeroesFlight.Common;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.FileManager.Stats;
using UnityEngine;

public class InventoryAddModificator
{
    public ItemSO itemSO;
    public int value;   
    public Rarity rarity;  
    
    public InventoryAddModificator(ItemSO itemSO, int value, Rarity rarity)
    {
        this.itemSO = itemSO;
        this.value = value;
        this.rarity = rarity;
    }
}

public class InventoryHandler : MonoBehaviour, IInventoryChangeSignal
{
    public event Action OnItemEquipped;
    public event Action OnInventoryUpdated;
    public event Action<List<StatTypeWithValue>> OnEqiuppedItemsStatChanged;

    public event Action<Item> OnEquipmentAdded;
    public event Action<Item> OnEquipmentUpdated;

    [SerializeField] private ItemInventorySO mainItemInventorySO;
    [SerializeField] private ItemDatabaseSO itemDatabaseSO;
    [SerializeField] private Dictionary<string, Item> equipmentItemDic = new Dictionary<string, Item>();
    [SerializeField] private Dictionary<string, Item> materialItemDic = new Dictionary<string, Item>();
    [SerializeField] private Dictionary<string, Item> equippedItemDic = new Dictionary<string, Item>();


    private List<StatTypeWithValue> equippedItemsStatDic = new List<StatTypeWithValue>();

    [Header("Test Item")] [SerializeField] private ItemSO[] testItem;
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
            List<InventoryAddModificator> itemsToAdd = new List<InventoryAddModificator>();
            foreach (ItemSO item in testItem)
            {
                Rarity randomRarity = (Rarity)UnityEngine.Random.Range(0, 4);

                itemsToAdd.Add(new InventoryAddModificator(item, 1, randomRarity));
            }
            AddMultipleToInventory(itemsToAdd.ToArray());
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            List<InventoryAddModificator> itemsToAdd = new List<InventoryAddModificator>();
            foreach (ItemSO item in testMaterialItem)
            {
                itemsToAdd.Add(new InventoryAddModificator(item, 1, Rarity.Common));
            }
            AddMultipleToInventory(itemsToAdd.ToArray());
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RemoveMultipleToInventory(GetInventoryEquippedItems().ToArray());
        }
    }

    public void Init(CurrencyManager currencyManager)
    {
        this.currencyManager = currencyManager;
    }

    public Item AddToInventory(ItemSO itemSO, int level = 1, Rarity rarity = Rarity.Common)
    {
        switch (itemSO.itemType)
        {
            case ItemType.Material:

                ItemData itemData = mainItemInventorySO.AddMaterialToInventory(itemSO, level);

                if (materialItemDic.ContainsKey(itemData.ID))
                {
                    materialItemDic[itemData.ID].GetItemData<ItemMaterialData>().value = itemData.value;
                }
                else
                {
                    materialItemDic.Add(itemData.ID, new Item(itemSO, itemData));
                }

                return materialItemDic[itemData.ID];

            case ItemType.Equipment:

                itemData = mainItemInventorySO.AddEquipmentToInventory(itemSO, level, rarity);

                ItemEquipmentData itemEquipmentData = itemData as ItemEquipmentData;
                equipmentItemDic.Add(itemEquipmentData.instanceID, new Item(itemSO, itemData));
                OnEquipmentAdded?.Invoke(equipmentItemDic[itemEquipmentData.instanceID]);
                return equipmentItemDic[itemEquipmentData.instanceID];
        }

        SaveInventoryItems();
        return null;
    }

    public void AddMultipleToInventory(params InventoryAddModificator[] inventoryModificators)
    {
        foreach (var inventoryModificator in inventoryModificators)
        {
            AddToInventory (inventoryModificator.itemSO, inventoryModificator.value, inventoryModificator.rarity);
        }
        SaveInventoryItems();
        OnInventoryUpdated?.Invoke();   
    }

    public void RemoveFromInventory(Item item)
    {
        switch (item.itemSO.itemType)
        {
            case ItemType.Equipment:

                if (item.GetItemData<ItemEquipmentData>().eqquiped)
                {
                    equippedItemDic.Remove(item.GetItemData<ItemEquipmentData>().instanceID);
                    ProcessEquippedItemStats();
                }
                equipmentItemDic.Remove(item.GetItemData<ItemEquipmentData>().instanceID);
                break;
            case ItemType.Material:
                materialItemDic.Remove(item.GetItemData<ItemMaterialData>().ID);
                break;
            default: break;
        }

        mainItemInventorySO.RemoveItemFromInventory(item.itemSO, item.GetItemData<ItemData>());
    }


    public void RemoveMultipleToInventory(params Item[] itemsToRemove)
    {
        foreach (Item item in itemsToRemove)
        {
            RemoveFromInventory (item);
        }
        SaveInventoryItems();
        OnInventoryUpdated?.Invoke();
    }

    public void RemoveAllItems()
    {
        RemoveMultipleToInventory(equipmentItemDic.Values.ToArray());
        RemoveMultipleToInventory(materialItemDic.Values.ToArray());
        RemoveMultipleToInventory(equippedItemDic.Values.ToArray());

        equipmentItemDic.Clear();
        materialItemDic.Clear();
        equippedItemDic.Clear();
    }

    public void LoadInventoryItems()
    {
        mainItemInventorySO.Load();

        if (mainItemInventorySO.inventoryData == null)
            return;
        foreach (ItemData itemData in mainItemInventorySO.inventoryData.equipmentData)
        {
            ItemSO itemSO = itemDatabaseSO.GetItemSOByID(itemData.ID);
            ItemEquipmentData itemEquipmentData = itemData as ItemEquipmentData;
            equipmentItemDic.Add(itemEquipmentData.instanceID, new Item(itemSO, itemData));

            if (itemEquipmentData.eqquiped)
            {
                equippedItemDic.Add(itemEquipmentData.instanceID, equipmentItemDic[itemEquipmentData.instanceID]);
            }
        }

        foreach (ItemData itemData in mainItemInventorySO.inventoryData.materialData)
        {
            ItemSO itemSO = itemDatabaseSO.GetItemSOByID(itemData.ID);
            materialItemDic.Add(itemData.ID, new Item(itemSO, itemData));
        }

        ProcessEquippedItemStats();
    }

    public void EquipItem(Item item)
    {
        item.GetItemData<ItemEquipmentData>().eqquiped = true;
        equippedItemDic.Add(item.GetItemData<ItemEquipmentData>().instanceID, item);

        ProcessEquippedItemStats();

        mainItemInventorySO.Save();

        OnItemEquipped?.Invoke();
    }

    public void UnEquipItem(Item item)
    {
        item.GetItemData<ItemEquipmentData>().eqquiped = false;
        equippedItemDic.Remove(item.GetItemData<ItemEquipmentData>().instanceID);

        ProcessEquippedItemStats();

        mainItemInventorySO.Save();
    }

    public void DismantleItem(Item item)
    {
        currencyManager.AddCurrency(CurrencyKeys.Gold,
            itemDatabaseSO.GetEquipmentTotalUpgradeGoldCost(item.GetItemData<ItemEquipmentData>()));
        GetMaterialItemByID("M_" + item.GetItemSO<EquipmentSO>().equipmentType.ToString(), out Item materialItem);
        if (materialItem != null)
        {
            materialItem.GetItemData<ItemData>().value +=
                itemDatabaseSO.GetEquipmentTotalUpgradeMaterialCost(item.GetItemData<ItemEquipmentData>());
           // OnItemModified?.Invoke(materialItem);
        }
        else
        {
            ItemSO itemSO = GetItemSO("M_" + item.GetItemSO<EquipmentSO>().equipmentType.ToString());
            if (itemSO != null)
            {
                AddToInventory(itemSO,
                    itemDatabaseSO.GetEquipmentTotalUpgradeMaterialCost(item.GetItemData<ItemEquipmentData>()));
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

        if (materialItem == null || materialItem.GetItemData<ItemMaterialData>().value < GetMaterialUpgradeRequiredAmount(item))
        {
            Debug.Log("Not enough material");
            return false;
        }

        currencyManager.ReduceCurency(CurrencyKeys.Gold, goldCost);
        materialItem.GetItemData<ItemMaterialData>().value -= GetMaterialUpgradeRequiredAmount(item);
        item.LevelUp();
        mainItemInventorySO.Save();


        if (item.GetItemData<ItemEquipmentData>().eqquiped)
        {
            ProcessEquippedItemStats();
        }

        OnEquipmentUpdated?.Invoke(item);

        return true;
    }

    public void ProcessEquippedItemStats()
    {
        //Debug.Log("ProcessEquippedItemStats");

        equippedItemsStatDic = new List<StatTypeWithValue>();
        foreach (Item item in equippedItemDic.Values)
        {
            // Apply base stat value

            int actualValue = itemDatabaseSO.GetCurrentStatValue(item.GetItemSO<EquipmentSO>(), item.GetItemData<ItemEquipmentData>().rarity,
                item.GetItemData<ItemEquipmentData>().value);

            StatType statType = item.GetItemSO<EquipmentSO>().statType;
            equippedItemsStatDic.Add(new StatTypeWithValue(statType, actualValue, StatModel.StatCalculationType.Flat));
            //Debug.Log($"Adding stat {statType} with value {actualValue}");

            // Add Special Hero Effect if any
            if (item.GetItemSO<EquipmentSO>().specialHeroEffect.value != 0)
            {
                // Only add this effect if the hero type is the same as the equipped hero
                equippedItemsStatDic.Add(item.GetItemSO<EquipmentSO>().specialHeroEffect);
            }

            // Add Unique Stat Modification Effects if any
            foreach (UniqueStatModificationEffect uniqueStatModificationEffect in item.GetItemSO<EquipmentSO>().uniqueStatModificationEffects)
            {
                int uniqueStatValue = uniqueStatModificationEffect.curve.GetCurrentValueInt(item.GetItemData<ItemEquipmentData>().value);
                equippedItemsStatDic.Add(new StatTypeWithValue(uniqueStatModificationEffect.statType, uniqueStatValue, StatModel.StatCalculationType.Percentage));
            }
        }

        OnEqiuppedItemsStatChanged?.Invoke(equippedItemsStatDic);
    }

    public int GetItemCurrentStat(Item item, int level)
    {
        int actualValue = itemDatabaseSO.GetCurrentStatValue(item.GetItemSO<EquipmentSO>(), item.GetItemData<ItemEquipmentData>().rarity, level);
        return actualValue;
    }

    public int GetItemMaxLevel(Item item) => itemDatabaseSO.GetItemMaxLevel(item);

    public int GetGoldUpgradeRequiredAmount(Item item) =>
        itemDatabaseSO.GetUpgradeGoldCost(item.GetItemData<ItemData>());

    public int GetMaterialUpgradeRequiredAmount(Item item) =>
        itemDatabaseSO.GetUpgradeMaterialCost(item.GetItemData<ItemData>());

    public ItemSO GetItemSO(string id) => itemDatabaseSO.GetItemSOByID(id);

    public void SaveInventoryItems() => mainItemInventorySO.Save();

    public List<Item> GetInventoryEquippmentItems() => new List<Item>(equipmentItemDic.Values);

    public bool GetMaterialItemByID(string id, out Item item) => materialItemDic.TryGetValue(id, out item);

    public List<Item> GetInventoryMaterialItems() => new List<Item>(materialItemDic.Values);

    public RarityPalette GetPalette(Rarity rarity) => itemDatabaseSO.GetRarityPalette(rarity);

    public int GetTotalUpgradeGoldSpent(ItemEquipmentData itemEquipmentData) =>
        itemDatabaseSO.GetEquipmentTotalUpgradeGoldCost(itemEquipmentData);

    public int GetTotalUpgradeMaterialSpent(ItemEquipmentData itemEquipmentData) =>
        itemDatabaseSO.GetEquipmentTotalUpgradeMaterialCost(itemEquipmentData);

    public Item GetEqupItemById(string objID)
    {
       
        if (equipmentItemDic.TryGetValue(objID, out var item))
        {
            return item;
        }

        return null;
    }

    public List<Item> GetInventoryEquippedItems() => new List<Item>(equippedItemDic.Values);
}