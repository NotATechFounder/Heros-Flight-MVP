using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace ScriptableObjectDatabase
{
    public interface IScriptableObjectDatabase
    {
        public void Pupolate();
    }

    public class ScriptableObjectDatabase<T> : ScriptableObject, IScriptableObjectDatabase where T : ScriptableObject
    {
        [SerializeField] private bool _manualInsert;

        public T[] Items;

        [ReadOnly] public List<ScriptableObjectDatabaseItem<T>> scriptableObjectDatabaseItems = new List<ScriptableObjectDatabaseItem<T>>();

        public void AddItemAndID()
        {
            scriptableObjectDatabaseItems.Clear();

            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] == null) continue;

                if (Items[i] is IHasID hasID)
                {
                    if (scriptableObjectDatabaseItems.Any(x => x.ID == hasID.GetID()))
                    {
                        Debug.LogError($"Duplicate ID found: {hasID.GetID()}");
                        continue;
                    }

                    scriptableObjectDatabaseItems.Add(new ScriptableObjectDatabaseItem<T>(hasID.GetID(), Items[i]));
                }
                else
                {
                    Debug.LogError($"ItemSO: {Items[i].name} does not implement IHasID");
                }

            }
        }

        public T GetItemSOByID(string id)
        {
            if (scriptableObjectDatabaseItems.Any(x => x.ID == id))
            {
                return scriptableObjectDatabaseItems.Find(x => x.ID == id).scriptableObject;
            }
            else
            {
                Debug.LogError($"ItemSO with ID: {id} not found");
                return null;
            }
        }

        void OnValidate()
        {
            if (_manualInsert) AddItemAndID();
        }


        public void Pupolate() 
        {
#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(this);
            path = path.Replace(this.name + ".asset", "");
            List<T> scriptableObjectBases = ScriptableObjectUtils.GetAllScriptableObjectBaseInFile<T>(path);
            Items = scriptableObjectBases.ToArray();
            AddItemAndID();

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }

}


    [System.Serializable]
    public class ScriptableObjectDatabaseItem<T> where T : ScriptableObject
    {
        public string ID;
        public T scriptableObject;

        public ScriptableObjectDatabaseItem(string _id, T _scriptableObject)
        {
            ID = _id;
            scriptableObject = _scriptableObject;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ScriptableObjectDatabase<>), true)]

    public class ScriptableObjectDatabaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            IScriptableObjectDatabase scriptableObjectDatabase = (IScriptableObjectDatabase)target;

            if (GUILayout.Button("Populate"))
            {
                scriptableObjectDatabase.Pupolate();
            }

            base.OnInspectorGUI();
        }
    }
#endif
}
