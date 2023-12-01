using System.Collections;
using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using UnityEngine;

[CreateAssetMenu(fileName = "New Material", menuName = "Inventory System/Items/Material")]
public class MaterialObject : ItemSO
{
    public EquipmentType equipmentType;

    private void Awake() => itemType = ItemType.Material;
}
