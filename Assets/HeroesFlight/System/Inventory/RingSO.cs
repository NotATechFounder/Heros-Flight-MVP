using UnityEngine;

[CreateAssetMenu(fileName = "New Ring", menuName = "Inventory System/Items/Equipment/Ring")]
public class RingSO : EquipmentObject
{
    private void Awake() => equipmentType = EquipmentType.Ring;
}