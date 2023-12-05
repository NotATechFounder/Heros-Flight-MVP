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
using HeroesFlight.System.Combat.Effects.Effects.Data;
using HeroesFlight.System.Combat.Effects.Effects;

[CreateAssetMenu(fileName = "New Helper", menuName = "Inventory System/Helper")]
public class InvenHelper : ScriptableObject
{
    [SerializeField] private ItemDatabaseSO itemDatabaseSO;
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

    [ContextMenu("Correct Effect ID and Level")]
    public void CorrectEffectIDAndLevel()
    {
        for (int i = 0; i < equipmentSOs.Length; i++)
        {
            for (int j = 0; j < equipmentSOs[i].uniqueStatModificationEffects.Length; j++)
            {
                equipmentSOs[i].uniqueStatModificationEffects[j].curve.curveType = CurveType.Linear;

                switch (equipmentSOs[i].uniqueStatModificationEffects[j].rarity)
                {
                    case Rarity.Common:
                        equipmentSOs[i].uniqueStatModificationEffects[j].curve.maxLevel = itemDatabaseSO.GetItemMaxLevel(Rarity.Common);
                        break;
                    case Rarity.UnCommon:
                        equipmentSOs[i].uniqueStatModificationEffects[j].curve.maxLevel = itemDatabaseSO.GetItemMaxLevel(Rarity.UnCommon);
                        break;
                    case Rarity.Rare:
                        equipmentSOs[i].uniqueStatModificationEffects[j].curve.maxLevel = itemDatabaseSO.GetItemMaxLevel(Rarity.Rare);
                        break;
                    case Rarity.Epic:
                        equipmentSOs[i].uniqueStatModificationEffects[j].curve.maxLevel = itemDatabaseSO.GetItemMaxLevel(Rarity.Epic);
                        break;
                    default:  break;
                }

                switch (equipmentSOs[i].uniqueStatModificationEffects[j].statType)
                {
                    case StatType.PhysicalDamage:
                        equipmentSOs[i].uniqueStatModificationEffects[j].curve.minValue = 5;
                        equipmentSOs[i].uniqueStatModificationEffects[j].curve.maxValue = 5;
                        break;
                    case StatType.MagicDamage:
                        break;
                    case StatType.MaxHealth:
                        break;
                    case StatType.MoveSpeed:
                        break;
                    case StatType.AttackSpeed:
                        break;
                    case StatType.DodgeChance:
                        break;
                    case StatType.Defense:
                        equipmentSOs[i].uniqueStatModificationEffects[j].curve.minValue = 10;
                        equipmentSOs[i].uniqueStatModificationEffects[j].curve.maxValue = 25;
                        break;
                    case StatType.CriticalHitChance:
                        break;
                    case StatType.PhysicalMagicDamage:
                        break;
                    case StatType.AllStats:
                        break;
                    case StatType.HealingBooster:
                        equipmentSOs[i].uniqueStatModificationEffects[j].curve.minValue = 20;
                        equipmentSOs[i].uniqueStatModificationEffects[j].curve.maxValue = 20;
                        break;
                    case StatType.AbilityDamage:
                        equipmentSOs[i].uniqueStatModificationEffects[j].curve.minValue = 10;
                        equipmentSOs[i].uniqueStatModificationEffects[j].curve.maxValue = 10;
                        break;
                    case StatType.ExperienceBoost:
                        equipmentSOs[i].uniqueStatModificationEffects[j].curve.minValue = 20;
                        equipmentSOs[i].uniqueStatModificationEffects[j].curve.maxValue = 40;
                        break;
                    case StatType.GoldBoost:
                        equipmentSOs[i].uniqueStatModificationEffects[j].curve.minValue = 20;
                        equipmentSOs[i].uniqueStatModificationEffects[j].curve.maxValue = 40;
                        break;
                    case StatType.LifeSteal:
                        break;
                    case StatType.CurrentHealth:
                        break;
                    case StatType.AttackRange:
                        break;
                    case StatType.CriticalHitDamage:
                        break;
                    case StatType.MulticastChance:
                        break;
                    default:
                        break;
                }

            }

            for (int j = 0; j < equipmentSOs[i].uniqueCombatEffects.Length; j++)
            {
                equipmentSOs[i].uniqueCombatEffects[j].combatEffect.SetID(equipmentSOs[i].ID);

                equipmentSOs[i].uniqueCombatEffects[j].curve.curveType = CurveType.Linear;

                switch (equipmentSOs[i].uniqueCombatEffects[j].rarity)
                {
                    case Rarity.Common:
                        equipmentSOs[i].uniqueCombatEffects[j].curve.maxLevel = itemDatabaseSO.GetItemMaxLevel(Rarity.Common);
                        break;
                    case Rarity.UnCommon:
                        equipmentSOs[i].uniqueCombatEffects[j].curve.maxLevel = itemDatabaseSO.GetItemMaxLevel(Rarity.UnCommon);
                        break;
                    case Rarity.Rare:
                        equipmentSOs[i].uniqueCombatEffects[j].curve.maxLevel = itemDatabaseSO.GetItemMaxLevel(Rarity.Rare);
                        break;
                    case Rarity.Epic:
                        equipmentSOs[i].uniqueCombatEffects[j].curve.maxLevel = itemDatabaseSO.GetItemMaxLevel(Rarity.Epic);
                        break;
                    default: break;
                }

                foreach (Effect effets in equipmentSOs[i].uniqueCombatEffects[j].combatEffect.EffectToApply)
                {
                    switch (effets)
                    {
                        case BurnStatusEffect:
                            {
                                equipmentSOs[i].uniqueCombatEffects[j].curve.minValue = 10;
                                equipmentSOs[i].uniqueCombatEffects[j].curve.maxValue = 25;
                            }
                            break;

                        case FreezeStatusEffect:
                            {
                                equipmentSOs[i].uniqueCombatEffects[j].curve.minValue = 10;
                                equipmentSOs[i].uniqueCombatEffects[j].curve.maxValue = 25;
                            }
                            break;

                        case PoisonStatusEffect:
                            {
                                equipmentSOs[i].uniqueCombatEffects[j].curve.minValue = 10;
                                equipmentSOs[i].uniqueCombatEffects[j].curve.maxValue = 25;
                            }
                            break;

                        case RootStatusEffect:
                            {
                                equipmentSOs[i].uniqueCombatEffects[j].curve.minValue = 10;
                                equipmentSOs[i].uniqueCombatEffects[j].curve.maxValue = 25;
                            }
                            break;

                        case ShockStatusEffect:
                            {
                                equipmentSOs[i].uniqueCombatEffects[j].curve.minValue = 10;
                                equipmentSOs[i].uniqueCombatEffects[j].curve.maxValue = 25;
                            }
                            break;

                        case StatModificationEffect:
                            {
                                switch (effets.GetData<StatModificationData>().TargetStat)
                                {
                                    case StatType.DodgeChance:
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.minValue = 10;
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.maxValue = 25;
                                        break;
                                    case StatType.Defense:
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.minValue = 10;
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.maxValue = 25;
                                        break;
                                    case StatType.CriticalHitChance:
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.minValue = 10;
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.maxValue = 40;
                                        break;
                                    case StatType.AllStats:
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.minValue = 5;
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.maxValue = 5;
                                        break;
                                    case StatType.HealingBooster:
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.minValue = 20;
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.maxValue = 40;
                                        break;

                                    case StatType.AbilityDamage:
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.minValue = 10;
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.maxValue = 40;
                                        break;
                                    case StatType.ExperienceBoost:
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.minValue = 10;
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.maxValue = 40;
                                        break;
                                    case StatType.GoldBoost:
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.minValue = 10;
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.maxValue = 40;
                                        break;
                                    case StatType.LifeSteal:
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.minValue = 0.05f;
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.maxValue = 0.25f;
                                        break;
                                    case StatType.CriticalHitDamage:
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.minValue = 10;
                                        equipmentSOs[i].uniqueCombatEffects[j].curve.maxValue = 25;
                                        break;
                                    default:  break;
                                }
                            }
                            break;
                        default: break;
                    }
                }
            }

            EditorUtility.SetDirty(equipmentSOs[i]);
        }
        AssetDatabase.SaveAssets();
    }
#endif
}
