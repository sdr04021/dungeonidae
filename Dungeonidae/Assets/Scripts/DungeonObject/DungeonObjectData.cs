using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonObjectData
{
    [JsonIgnore] public DungeonObject Owner { get; private set; }
    public Coordinate coord = new(0, 0);
    [JsonProperty] public string Key { get; private set; }

    public bool isActivated = false;
    public bool isHidden = false;

    public void Init(DungeonObject dunObj, string key)
    {
        Owner = dunObj;
        Key = key;
    }
    public void SetOwner(DungeonObject dunObj)
    {
        Owner = dunObj;
    }
}
