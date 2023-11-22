using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Accessory", menuName = "Inventory System/Items/Equipment/Necklace")]
public class NecklaceSO : EquipmentObject
{
    private void Awake() => equipmentType = EquipmentType.Necklace;
}
