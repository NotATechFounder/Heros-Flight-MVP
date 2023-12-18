using System;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using UnityEngine;
using UnityEditor;
using ScriptableObjectDatabase;


public class ItemSO : RewardBaseSO, IHasID
{
    public string ID;
    public string Name;
    public Sprite icon;

    public ItemType itemType;

    [TextArea] public string description;

    public string GetID() => ID;

#if UNITY_EDITOR
    [ContextMenu("ResetName")]
    public void RenameFile()
    {
        string assetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
        AssetDatabase.RenameAsset(assetPath, ID);
    }
#endif
}

[Serializable]
public class Item
{
    public ItemSO itemSO;
    [SerializeField] ItemData itemData;

    public Item(ItemSO itemObject, ItemData itemData)
    {
        this.itemSO = itemObject;
        this.itemData = itemData;
    }

    public T GetItemSO<T>() where T : ItemSO => itemSO as T;
    public T GetItemData<T>() where T : ItemData => itemData as T;


    public void LevelUp()
    {
        if (itemSO is EquipmentSO) itemData.LevelUp();
    }


    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;

        Item item = (Item)obj;

        switch (itemData)
        {
            case ItemEquipmentData itemEquipmentData:
                return itemEquipmentData.instanceID == item.GetItemData<ItemEquipmentData>().instanceID;
            case ItemMaterialData itemMaterialData:
                return itemMaterialData.ID == item.itemData.ID;
            default: return false;
        }
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(itemSO, itemData);
    }
}

[Serializable]
public abstract class ItemData
{
    public string ID;
    public int value;

    public ItemData(ItemSO item, int newValue = 1)
    {
        ID = item.ID;
        value = newValue;
    }

    public int GetValue()
    {
        return value;
    }

    public void IncrementValue(int increment)
    {
        value += increment;
    }

    public void LevelUp()
    {
        value++;
    }
}

[Serializable]
public class ItemEquipmentData : ItemData
{
    public string instanceID;
    public Rarity rarity;
    public bool eqquiped;

    public ItemEquipmentData(ItemSO item, int newValue = 1, Rarity rarity = Rarity.Common) : base(item, newValue)
    {
        ID = item.ID;
        value = newValue;
        this.rarity = rarity;
        instanceID = "Item :" + Guid.NewGuid().ToString();
        //instanceID = instanceID.Substring(instanceID.Length - 5);
    }

    public void SetRarity(Rarity rarity)
    {
        this.rarity = rarity;
    }
}

[Serializable]
public class ItemMaterialData : ItemData
{
    public ItemMaterialData(ItemSO item, int newValue = 1) : base(item, newValue)
    {
    }
}