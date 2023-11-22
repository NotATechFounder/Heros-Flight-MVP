using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Armour", menuName = "Inventory System/Items/Equipment/Armour")]
public class ArmourObject : EquipmentObject
{
    private void Awake() => equipmentType = EquipmentType.Armour;
}
