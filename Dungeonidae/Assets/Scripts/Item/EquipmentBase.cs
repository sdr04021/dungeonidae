using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentStat
{
    public StatType statType;
    public StatUnit statUnit;
    public int val;
    public int bonus;
}

[CreateAssetMenu(fileName = "EquipmentBase", menuName = "Scriptable Object/EquipmentBase")]
public class EquipmentBase : ItemBase
{
    public EquipmentType equipmentType;
    public EquipmentStat[] stats;
}
