using HeroesFlight.System.FileManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class InventoryData<T>
{
    public abstract int Count { get; }
    public abstract void Add(T itemBaseData);
    public abstract void Remove(T itemBaseData);
    public abstract bool Contains(T itemBaseData);
    public abstract void Clear();
}

public interface IInventorySO
{
    void Save();
    void Load();
    void Clear();
    void Delete();
}

public class InventorySO<IInventoryData, T1> : ScriptableObject, IInventorySO where IInventoryData : InventoryData<T1>
{
    public int max;
    public string inventoryID;
    public IInventoryData inventoryData;
    public event Action<T1> OnValueAdded;
    public event Action<T1> OnValueRemoved;
    public event Action OnMaxReachError;

    public virtual bool AddToInventory(T1 data, bool notify = true)
    {
        if (inventoryData.Count >= max)
        {
            OnMaxReachError?.Invoke();
            return false;
        }
        inventoryData.Add(data);
        if (notify)  OnValueAdded?.Invoke(data);
        Save();
        return true;
    }

    public virtual void RemoveFromInventory(T1 data, bool notify = true)
    {
        inventoryData.Remove(data);
        if (notify) OnValueRemoved?.Invoke(data);
        Save();
    }

    public bool ExitsInInventory(T1 data)
    {
        return inventoryData.Contains(data);
    }

    public virtual void Save()
    {
        if (string.IsNullOrEmpty(inventoryID))
        {
            Debug.LogError("Inventory ID is null or empty", this);
            return;
        }
        FileManager.Save(inventoryID, inventoryData);
    }

    public virtual void Load()
    {
        if (string.IsNullOrEmpty(inventoryID))
        {
            Debug.LogError("Inventory ID is null or empty", this);
            return;
        }
        inventoryData.Clear();
        IInventoryData savedInventoryData = FileManager.Load<IInventoryData>(inventoryID);
        inventoryData = savedInventoryData != null ? savedInventoryData : default;
    }

    public virtual void Clear()
    {
        if (inventoryData != null) inventoryData.Clear();
        Save();
    }

    public void Delete()
    {
        FileManager.Delete(inventoryID);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(InventorySO<,>), true)]
public class InventorySOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.color = Color.green;
        if (GUILayout.Button("Save")) ((IInventorySO)target).Save();
        GUI.color = Color.yellow;
        if (GUILayout.Button("Clear")) ((IInventorySO)target).Clear();
        GUI.color = Color.red;
        if (GUILayout.Button("Delete")) ((IInventorySO)target).Delete();
    }
}

#endif