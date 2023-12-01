using ScriptableObjectDatabase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Text.RegularExpressions;
using HeroesFlight.Common.Enum;

[CreateAssetMenu(fileName = "New Helper", menuName = "Inventory System/Helper")]
public class InvenHelper : ScriptableObject
{
    [SerializeField] int min;
    [SerializeField] int difference;
    [SerializeField]  Rarity rarity;  
    [SerializeField] private EquipmentSO[] equipmentSOs;

    [Header("Sprite")]
    [SerializeField] private Sprite[] sprites;

    [Header("Item Info")]
    [SerializeField] private TextAsset itemInfo;
    [SerializeField] private ItemInfoList itemInfoList = new ItemInfoList();

    [Serializable]
    public class ItemInfoCSV
    {
        public string Id;
        public string Name;
        public string Description;
        public float Common;
        public float Uncommon;
        public float Rare;
        public float Epic;
    }


    [Serializable]
    public class ItemInfoList
    {
        public ItemInfoCSV[] itemInfoCSVs;
    }

#if UNITY_EDITOR

    [ContextMenu("ReadCSV")]
    public void ReadCSV()
    {
        string[] lines = itemInfo.text.Split(new char[] { '\n' });
        itemInfoList.itemInfoCSVs = new ItemInfoCSV[lines.Length - 1];

        for (int i = 1; i < lines.Length - 1; i++)
        {
            string[] row = lines[i].Split(new char[] { ',' });
            itemInfoList.itemInfoCSVs[i - 1] = new ItemInfoCSV();
            itemInfoList.itemInfoCSVs[i - 1].Id = row[0];
            itemInfoList.itemInfoCSVs[i - 1].Name = row[1];
            itemInfoList.itemInfoCSVs[i - 1].Description = row[2];
            itemInfoList.itemInfoCSVs[i - 1].Common = float.Parse(row[3]);
            itemInfoList.itemInfoCSVs[i - 1].Uncommon = float.Parse(row[4]);
            itemInfoList.itemInfoCSVs[i - 1].Rare = float.Parse(row[5]);
            itemInfoList.itemInfoCSVs[i - 1].Epic = float.Parse(row[6]);
        }
    }


    [ContextMenu("UpdateItemInfo")]
    public void UpdateItemInfo()
    {
        for (int i = 0; i < itemInfoList.itemInfoCSVs.Length; i++)
        {
            for (int j = 0; j < equipmentSOs.Length; j++)
            {
                if (itemInfoList.itemInfoCSVs[i].Name == equipmentSOs[j].Name)
                {
                    equipmentSOs[j].ID = itemInfoList.itemInfoCSVs[i].Id;
                    equipmentSOs[j].description = itemInfoList.itemInfoCSVs[i].Description;
                    for (int k = 0; k < equipmentSOs[j].itemBaseStats.Length; k++)
                    {
                        switch (equipmentSOs[j].itemBaseStats[k].rarity)
                        {
                            case Rarity.Common:
                                equipmentSOs[j].itemBaseStats[k].value = (int)itemInfoList.itemInfoCSVs[i].Common;
                                break;
                            case Rarity.UnCommon:
                                equipmentSOs[j].itemBaseStats[k].value = (int)itemInfoList.itemInfoCSVs[i].Uncommon;
                                break;
                            case Rarity.Rare:
                                equipmentSOs[j].itemBaseStats[k].value = (int)itemInfoList.itemInfoCSVs[i].Rare;
                                break;
                            case Rarity.Epic:
                                equipmentSOs[j].itemBaseStats[k].value = (int)itemInfoList.itemInfoCSVs[i].Epic;
                                break;
                            default:break;
                        }
                    }
                }
                EditorUtility.SetDirty(equipmentSOs[j]);
            }
        }
        AssetDatabase.SaveAssets();
    }


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
                EditorUtility.SetDirty(equipmentSOs[j]);
            }
        }
        AssetDatabase.SaveAssets();
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
