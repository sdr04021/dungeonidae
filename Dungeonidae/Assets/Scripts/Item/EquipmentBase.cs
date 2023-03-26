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
    [SerializeField] EquipmentType equipmentType;   
    public EquipmentType EquipmentType { get => equipmentType; }

    [SerializeField]
    EquipmentStat[] stats;
    public EquipmentStat[] Stats { get => stats; }
}
