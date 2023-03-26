using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentData : ItemData
{
    EquipmentType equipmentType;
    public EquipmentType EquipmentType { get => equipmentType; }

    public Dictionary<StatType,int> Stats { get; private set; } = new Dictionary<StatType, int>();

    int enchant = 0;
    public int Enchant { get => enchant; }

    public EquipmentData(EquipmentBase data) : base (data)
    {
        equipmentType = data.EquipmentType;

        for(int i=0; i < data.Stats.Length; i++)
        {
            Stats.Add(data.Stats[i].statType, (int)(data.Stats[i].val * (1 + Mathf.Pow(Random.value, 4))));
        }
    }
}
