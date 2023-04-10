using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonData
{
    public int floor;
    [System.NonSerialized] public List<List<TileData>> mapData = new();
    public List<List<FogData>> fogData = new();
    [System.NonSerialized] public List<Room> rooms = new();

    public List<UnitData> unitList = new();

    [JsonIgnore]
    public System.Random Rand { get; private set; }

    public void SetRandom() 
    {
        Rand = new(GameManager.Instance.saveData.Seeds[floor]);
    }
}
