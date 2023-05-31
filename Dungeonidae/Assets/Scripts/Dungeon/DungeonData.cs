using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonData
{
    public int floor;
    [System.NonSerialized] public List<List<TileData>> mapData;
    [System.NonSerialized] public List<Room> rooms;
    [System.NonSerialized] public System.Tuple<int, int> stairRooms;
    [System.NonSerialized] public List<Coordinate> genArea;
    [System.NonSerialized] public DungeonType dungeonType;
    public HashSet<Coordinate> observedFog = new();

    public List<UnitData> unitList = new();
    public List<ItemData> fieldItemList = new();
    public List<DungeonObjectData> dungeonObjectList = new();
}
