using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MiscData : ItemData
{
    [JsonIgnore]
    MiscBase baseData;
    [JsonIgnore]
    public MiscBase BaseData
    {
        get
        {
            if (baseData == null) baseData = GameManager.Instance.GetMiscBase(Key);
            return baseData;
        }
    }

    [JsonProperty]
    public int Amount { get; private set; }

    [JsonIgnore]
    public int AmountLeft { get => (BaseData.MaxStack - Amount); }

    public MiscData() { }

    public MiscData(string key, int amount) : base(key)
    {
        Amount = amount;
        baseData = GameManager.Instance.GetMiscBase(key);
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
