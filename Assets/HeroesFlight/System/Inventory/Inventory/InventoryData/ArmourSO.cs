using System.Collections;
using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using UnityEngine;

[CreateAssetMenu(fileName = "New Armour", menuName = "Inventory System/Items/Equipment/Armour")]
public class ArmourSO : EquipmentSO
{
    private void Awake() => equipmentType = EquipmentType.Armour;
}
