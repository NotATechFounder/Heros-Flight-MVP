
using UnityEngine;

[CreateAssetMenu(fileName = "New Belt", menuName = "Inventory System/Items/Equipment/Belt")]
public class BeltSO : EquipmentObject
{
    private void Awake() => equipmentType = EquipmentType.Belt;
}
