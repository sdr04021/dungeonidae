using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDataContainer
{
    [JsonProperty] public EquipmentData Equip { get; private set; }
    [JsonProperty] public MiscData Misc { get; private set; }

    public ItemDataContainer(ItemData item)
    {
        if(item is EquipmentData equip)
            Equip = equip;
        else if (item is MiscData misc)
            Misc = misc;
    }

    public ItemData GetItemData()
    {
        if (Equip != null) return Equip;
        else if (Misc != null) return Misc;
        else return null;
    }
}
