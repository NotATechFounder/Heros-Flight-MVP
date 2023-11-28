using ScriptableObjectDatabase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Helper", menuName = "Inventory System/Helper")]
public class InvenHelper : ScriptableObject
{
    [SerializeField] int min;
    [SerializeField] int difference;
    [SerializeField]  Rarity rarity;  
    [SerializeField] private EquipmentSO[] equipmentSOs;

    [SerializeField] private string spritePath;
    [SerializeField] private Sprite[] sprites;  


    [ContextMenu("Update Base Value")]
    public void UpdateBaseValue()
    {
        for (int i = 0; i < equipmentSOs.Length; i++)
        {
            for (int j = 0; j < equipmentSOs[i].itemBaseStats.Length; j++)
            {
                if (equipmentSOs[i].itemBaseStats[j].rarity == rarity)
                {
                    equipmentSOs[i].itemBaseStats[j].value = min + (difference * j);
                }
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("InsertItemIcon")]
    public void InsertItemIcon()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            for (int j = 0; j < equipmentSOs.Length; j++)
            {
                if (equipmentSOs[j].name == sprites[i].name)
                {
                    equipmentSOs[j].icon = sprites[i];
                }
            }
        }
    }

    [ContextMenu("PopulateItemEffects")]
    public void PopulateItemEffects()
    {
        string path = AssetDatabase.GetAssetPath(this);
        path = path.Replace(this.name + ".asset", "");
        List<EquipmentSO> scriptableObjectBases = ScriptableObjectUtils.GetAllScriptableObjectBaseInFile<EquipmentSO>(path);
        equipmentSOs = scriptableObjectBases.ToArray();
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
