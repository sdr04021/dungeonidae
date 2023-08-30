using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentStat
{
    public StatType statType;
    public StatUnit statUnit;
    public int val;
    [HideInInspector] public int bonus;
}

[System.Serializable]
public class EquipmentAbility
{
    public AdditionalEffectKey key;
    public List<int> vals;
    [HideInInspector] public List<int> bonuses;
    public List<int> increments;
}

[CreateAssetMenu(fileName = "EquipmentBase", menuName = "Scriptable Object/EquipmentBase")]
public class EquipmentBase : ItemBase
{
    public EquipmentType equipmentType;
    public EquipmentStat[] stats;
    public EquipmentAbility[] abilities;
}
