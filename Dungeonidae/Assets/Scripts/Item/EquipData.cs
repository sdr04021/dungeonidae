using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipStat
{
    public EquipStatType statType;
    public int val;
}

[CreateAssetMenu(fileName = "EquipData", menuName = "Scriptable Object/EquipData")]
public class EquipData : ItemData
{
    [SerializeField]
    EquipStat[] stats;
    public EquipStat[] Stats { get => stats; }
}
