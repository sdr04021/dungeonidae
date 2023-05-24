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
    public int Amount { get; private set; }

    [JsonIgnore]
    public int AmountLeft { get => (GameManager.Instance.GetMiscBase(Key).MaxStack - Amount); }

    public MiscData() { }

    public MiscData(MiscBase misc, int amount) : base(misc)
    {
        Amount = amount;
    }

    public void AddAmount(int amount)
    {
        Amount += amount;
    }

    public override Sprite GetSprite()
    {
        return GameManager.Instance.GetSprite(SpriteAssetType.Misc, Key);
    }
}
