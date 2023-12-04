using System.Collections;
using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory System/Items/Equipment/Weapon")]
public class WeaponSO : EquipmentSO
{
    private void Awake() => equipmentType = EquipmentType.Weapon;
}
