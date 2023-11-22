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
    private void OnValidate()
    {
        RenameFile();
    }

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
    public ItemSO itemObject;
    [SerializeField] ItemData itemData;
    [SerializeField] ItemBuff[] buffs;

    public Item(ItemSO itemObject, ItemData itemData)
    {
        this.itemObject = itemObject;
        this.itemData = itemData;

        if (itemObject is EquipmentObject) AddBuffs(itemObject as EquipmentObject);
    }

    public ItemData ItemData() => itemData;

    public ItemBuff[] ItemBuffs() => buffs;

    public void LevelUp()
    {
        if (itemObject is EquipmentObject) itemData.LevelUp();
       // InventoryManager.Instance.ItemManager().SetItemBuffStat(this);
    }

    public void AddBuffs(EquipmentObject item)
    {
        buffs = new ItemBuff[item.buffs.Length];
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff();
            buffs[i].buffType = item.buffs[i];
        }
    }
}

[Serializable]
public class ItemData
{
    public string ID;
    public string instanceID;
    public bool eqquiped;
    public bool deleted;
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

