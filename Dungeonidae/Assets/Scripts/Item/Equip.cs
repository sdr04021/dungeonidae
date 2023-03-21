using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equip : Item
{
    public Dictionary<EquipStatType,int> Stats { get; private set; } = new Dictionary<EquipStatType, int>();

    int enchant = 0;
    public int Enchant { get => enchant; }

    public Equip (EquipData data) : base (data)
    {
        for(int i=0; i < data.Stats.Length; i++)
        {
            Stats.Add(data.Stats[i].statType, data.Stats[i].val);
        }
    }
}
