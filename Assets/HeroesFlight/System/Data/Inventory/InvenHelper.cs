using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Helper", menuName = "Inventory System/Helper")]
public class InvenHelper : ScriptableObject
{
    [SerializeField] int min;
    [SerializeField] int difference;
    [SerializeField]  Rarity rarity;  
    [SerializeField] private EquipmentSO[] equipmentSOs;


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
}
