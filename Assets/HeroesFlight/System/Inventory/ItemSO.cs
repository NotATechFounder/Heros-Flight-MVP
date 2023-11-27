using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ScriptableObjectDatabase;

public enum ItemType { Equipment, Material};

public class ItemSO : ScriptableObject, IHasID
{
    public string ID;
    public string Name;
    public Sprite icon;

    public ItemType itemType;

    [TextArea]
    public string description;

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
    [SerializeField] ItemEffect[] itemEffects;

    public Item(ItemSO itemObject, ItemData itemData)
    {
        this.itemSO = itemObject;
        this.itemData = itemData;

        if (itemObject is EquipmentSO) AddEffects(itemObject as EquipmentSO);
    }

    public T GetItemSO<T>() where T : ItemSO => itemSO as T;

    public ItemData GetItemData() => itemData;

    public ItemEffect[] ItemBuffs() => itemEffects;

    public void LevelUp()
    {
        if (itemSO is EquipmentSO) itemData.LevelUp();
    }

    public void AddEffects(EquipmentSO item)
    {
        itemEffects = new ItemEffect[item.effects.Length];
        for (int i = 0; i < itemEffects.Length; i++)
        {
            itemEffects[i] = new ItemEffect();
            itemEffects[i].itemEffectType = item.effects[i];
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;

        Item item = (Item)obj;
        return itemData.instanceID == item.itemData.instanceID;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(itemSO, itemData, itemEffects);
    }
}

[Serializable]
public class ItemData
{
    public string ID;
    public string instanceID;
    public bool eqquiped;
    public int value;

    public ItemData(ItemSO item, int newValue = 1) 
    {
        ID = item.ID; 
        value = newValue;
        instanceID = "Item :" + Guid.NewGuid().ToString();
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
public class ItemBaseData
{
    public string ID;
    public int value;

    public ItemBaseData ( ItemSO item, int newValue = 1)
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
public class ItemEquipmentData : ItemBaseData
{
    public string instanceID;
    public bool eqquiped;
    public ItemEquipmentData(ItemSO item, int newValue = 1) : base(item, newValue)
    {
        ID = item.ID;
        value = newValue;
        instanceID = "Item :" + Guid.NewGuid().ToString();
    }
}

[Serializable]
public class ItemMaterialData : ItemBaseData
{
    public ItemMaterialData(ItemSO item, int newValue = 1) : base(item, newValue)
    {
        ID = item.ID;
        value = newValue;
    }
}

