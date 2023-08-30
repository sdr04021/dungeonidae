using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ItemData
{
    [JsonProperty]
    public string Key { get; private set; }

    public Coordinate coord;

    [JsonIgnore]
    public ItemObject owner = null;

    public ItemData() { }

    public ItemData(string key)
    {
        Key = key;
    }

    public abstract Sprite GetSprite();
}
