using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Accessory", menuName = "Inventory System/Items/Equipment/Accesory")]
public class AccesoryObject : EquipmentObject
{
    private void Awake() => equipmentType = EquipmentType.Accessory;
}
