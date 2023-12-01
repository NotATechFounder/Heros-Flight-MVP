using HeroesFlight.Common.Enum;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ring", menuName = "Inventory System/Items/Equipment/Ring")]
public class RingSO : EquipmentSO
{
    private void Awake() => equipmentType = EquipmentType.Ring;
}