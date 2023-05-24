using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public TileType tileType = TileType.Wall;
    public AreaType areaType = AreaType.None;
    public readonly int seed;

    public TileData(int seed)
    {
        this.seed = seed;
    }
}
