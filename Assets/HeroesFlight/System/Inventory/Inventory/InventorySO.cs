using HeroesFlight.System.FileManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public interface IInventorySO
{
    void Save();
    void Load();
    void Clear();
    void Delete();
}

public class InventorySO<T> : ScriptableObject, IInventorySO
{
    public int max;
    public string inventoryID;
    public InventoryData<T> inventoryData;
    public event Action<T> OnValueAdded;
    public event Action<T> OnValueRemoved;
    public event Action OnMaxReachError;

    public virtual bool AddToInventory(T data, bool notify = true)
    {
        if (inventoryData.savedData.Count >= max)
        {
            OnMaxReachError?.Invoke();
            return false;
        }
        inventoryData.savedData.Add(data);
        if (notify)  OnValueAdded?.Invoke(data);
        Save();
        return true;
    }

    public virtual void RemoveFromInventory(T data, bool notify = true)
    {
        inventoryData.savedData.Remove(data);
        if (notify) OnValueRemoved?.Invoke(data);
        Save();
    }

    public bool ExitsInInventory(T data)
    {
        return inventoryData.savedData.Contains(data);
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
        inventoryData.savedData.Clear();
        InventoryData<T> savedInventoryData = FileManager.Load<InventoryData<T>>(inventoryID);
        inventoryData = savedInventoryData != null ? savedInventoryData : new InventoryData<T>();
    }

    public virtual void Clear()
    {
        inventoryData.savedData.Clear();
        Save();
    }

    public void Delete()
    {
        FileManager.Delete(inventoryID);
    }
}

[Serializable]
public class InventoryData<T>
{
    public List<T> savedData = new List<T>();
}

#if UNITY_EDITOR

[CustomEditor(typeof(InventorySO<>), true)]
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