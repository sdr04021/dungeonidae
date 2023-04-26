using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonObjectData
{
    [JsonIgnore] public DungeonObject Owner { get; private set; }
    public Coordinate coord = new(0, 0);
    [JsonProperty] public System.Type ClassType { get; private set; }

    public void Init(DungeonObject dunObj, System.Type classType)
    {
        Owner = dunObj;
        ClassType = classType;
    }
    public void SetOwner(DungeonObject dunObj)
    {
        Owner = dunObj;
    }
}
