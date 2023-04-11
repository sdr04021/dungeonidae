using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonData
{
    public int floor;
    [System.NonSerialized] public List<List<TileData>> mapData = new();
    [System.NonSerialized] public List<List<FogData>> fogData = new();
    [System.NonSerialized] public List<Room> rooms = new();
    public HashSet<Coordinate> observedFog = new();

    public List<UnitData> unitList = new();
    public List<ItemDataContainer> fieldItemList = new();

    [JsonIgnore]
    public System.Random Rand { get; private set; }

    public void SetRandom() 
    {
        Rand = new(GameManager.Instance.saveData.Seeds[floor]);
    }

    public void RemoveFieldItem(ItemData item)
    {
        for(int i=0; i<fieldItemList.Count; i++)
        {
            if (item == fieldItemList[i].GetItemData())
            {
                fieldItemList.RemoveAt(i);
                break;
            }
        }
    }
}
