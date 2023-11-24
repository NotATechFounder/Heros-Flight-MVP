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

    public ItemData ItemData() => itemData;

    public ItemEffect[] ItemBuffs() => itemEffects;

    public void LevelUp()
    {
        if (itemSO is EquipmentSO) itemData.LevelUp();
       // InventoryManager.Instance.ItemManager().SetItemBuffStat(this);
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

