using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public class MiscData : ItemData
{
    [JsonProperty]
    public int[] EffectValues { get; private set; }

    [JsonProperty]
    public int MaxStack { get; private set; }

    [JsonProperty]
    public int Amount { get; private set; }

    [JsonIgnore]
    public int AmountLeft { get => (MaxStack - Amount); }

    public MiscData() { }

    public MiscData(MiscBase misc, int amount) : base(misc)
    {
        EffectValues = misc.EffectValues;
        MaxStack = misc.MaxStack;
        Amount = amount;
    }

    public void AddAmount(int amount)
    {
        Amount += amount;
    }

    protected override void LoadItemIcon()
    {
        loadHandle = Addressables.LoadAssetAsync<Sprite>("Assets/Sprites/Misc Sprites/" + Key + ".png");
        loadHandle.WaitForCompletion();
        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            Sprite = loadHandle.Result;
    }
}
