using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public abstract class ItemData
{
    [JsonProperty]
    public string Key { get; private set; }

    public Coordinate coord;

    [JsonIgnore]
    public ItemObject owner = null;

    public ItemData() { }

    public ItemData(ItemBase item)
    {
        Key = item.Key;
    }

    public abstract Sprite GetSprite();
}
