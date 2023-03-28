using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentStat
{
    public StatType statType;
    public int val;
}

[CreateAssetMenu(fileName = "EquipmentBase", menuName = "Scriptable Object/EquipmentBase")]
public class EquipmentBase : ItemBase
{
    [field: SerializeField]
    public EquipmentType EquipmentType { get; private set; }

    [field: SerializeField]
    public EquipmentStat[] Stats { get; private set; }
}
