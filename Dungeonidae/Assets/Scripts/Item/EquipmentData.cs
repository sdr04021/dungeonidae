using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


[System.Serializable]
public class EquipmentData : ItemData
{
    [JsonProperty]
    public EquipmentType EquipmentType { get; private set; }

    [JsonProperty]
    public Dictionary<StatType,int> Stats { get; private set; } = new Dictionary<StatType, int>();

    [JsonProperty]
    public int Enchant { get; private set; }

    public EquipmentData() { }

    public EquipmentData(EquipmentBase data) : base (data)
    {
        EquipmentType = data.EquipmentType;

        for(int i=0; i < data.Stats.Length; i++)
        {
            Stats.Add(data.Stats[i].statType, (int)(data.Stats[i].val * (1 + Mathf.Pow(Random.value, 4))));
        }
    }

    protected override void LoadItemIcon()
    {
        loadHandle = Addressables.LoadAssetAsync<Sprite>("Assets/Sprites/Equipment Sprites/" + Key + ".png");
        loadHandle.WaitForCompletion();
        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            Sprite = loadHandle.Result;
    }
}
