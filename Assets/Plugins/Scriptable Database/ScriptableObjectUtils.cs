using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ScriptableObjectUtils
{
    public static List<T> GetAllScriptableObjectBaseInFile<T>(string path) where T : ScriptableObject
    {
        List<T> scriptableObjectBases = new List<T>();

        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name, new[] { path });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            T scriptableObjectBase = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            scriptableObjectBases.Add(scriptableObjectBase);
        }
        return scriptableObjectBases;
    }

    public static List<T> FindAllScriptableObjectsInProject<T>() where T : ScriptableObject
    {
        List<T> results = new List<T>();
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                results.Add(asset);
            }
        }
        return results;
    }
}
