using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Unit unit;
    public TileType type = TileType.Floor;
    public AreaType Area { get; private set; } = AreaType.None;

    [SerializeField] SpriteRenderer spriteRenderer;
    public DungeonManager dm;
    Coordinate coord;
    public Coordinate Coord { get { return coord; } }

    public void Init(TileType t, int x, int y)
    {
        SetTileType(t);
        coord = new Coordinate(x, y);
        transform.position = new Vector2(x, y);
    }

    public bool IsReachableTile()
    {
        if ((type != TileType.Wall) && (unit == null))
            return true;
        else return false;
    }

    public void SetTileType(TileType t) { 
        type = t;
        if (t == TileType.Floor)
            spriteRenderer.sprite = GameManager.Instance.tileSprites[0];
        else if (t == TileType.Wall)
            spriteRenderer.sprite = GameManager.Instance.tileSprites[1];
    }

    public void SetAreaType(AreaType t)
    {
        Area = t;
    }
}
