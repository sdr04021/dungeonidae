using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public TileType type = TileType.Floor;
    public AreaType Area { get; private set; } = AreaType.None;
}
