
using HeroesFlight.Common.Enum;
using UnityEngine;

[CreateAssetMenu(fileName = "New Belt", menuName = "Inventory System/Items/Equipment/Belt")]
public class BeltSO : EquipmentSO
{
    private void Awake() => equipmentType = EquipmentType.Belt;
}
