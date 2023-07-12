using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileData TileData { get; private set; }

    public Unit unit;
    public Stack<ItemObject> items = new();
    public List<DungeonObject> dungeonObjects;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] SpriteRenderer upSprite;
    [SerializeField] SpriteRenderer downSprite;
    [SerializeField] SpriteRenderer cornereLeft;
    [SerializeField] SpriteRenderer cornerRight;
    DungeonManager dm;
    Coordinate coord;
    public Coordinate Coord { get { return coord; } }

    [SerializeField] SpriteRenderer rangeIndicator;
    readonly Color transparentRed = new(1, 0, 0, 0.33f);
    readonly Color transparentBlue = new(0, 0, 1, 0.33f);
    [SerializeField] Sprite redRange;
    [SerializeField] Sprite blueRange;
    public bool IsAvailable { get; private set; } = false;

    public void Init(DungeonManager dm, TileData tileData, int x, int y)
    {
        this.dm = dm;
        TileData = tileData;
        coord = new Coordinate(x, y);
        //transform.position = new Vector2(x, y);
        rangeIndicator.sortingOrder = 1000;
    }

    public bool IsReachableTile()
    {
        if ((TileData.tileType == TileType.Wall) || unit != null)
            return false;
        for (int i = 0; i < dungeonObjects.Count; i++)
        {
            if (!dungeonObjects[i].IsPassable)
                return false;
        }
        return true;
    }

    public bool IsEmptyTile()
    {
        if ((TileData.tileType != TileType.Wall) && (unit == null) && (dungeonObjects.Count == 0))
            return true;
        else return false;
    }

    public bool HasTargetable()
    {
        for (int i = 0; i < dungeonObjects.Count; i++)
        {
            if (dungeonObjects[i].IsTargetable())
                return true;
        }
        return false;
    }
    public DungeonObject GetTargetable()
    {
        for(int i=0; i<dungeonObjects.Count; i++)
        {
            if (dungeonObjects[i].IsTargetable())
                return dungeonObjects[i];
        }
        return null;
    }

    public bool IsBlockingSight()
    {
        if ((TileData.tileType == TileType.Wall))
            return true;
        for(int i=0; i<dungeonObjects.Count; i++)
        {
            if (dungeonObjects[i].IsBlockSight)
                return true;
        }

        return false;
    }

    public bool ContainsTargetableDungeonObject()
    {
        if (dungeonObjects.Count > 0)
        {
            for(int i=0; i<dungeonObjects.Count; i++)
            {
                if (dungeonObjects[i].IsTargetable())
                    return true;
            }
        }
        return false;
    }

    public void ResetTile()
    {
        unit = null;
        items.Clear();
        dungeonObjects.Clear();
        upSprite.gameObject.SetActive(false);
        downSprite.gameObject.SetActive(false);
        cornereLeft.gameObject.SetActive(false);
        cornerRight.gameObject.SetActive(false);
        TurnOffRangeIndicator();
    }

    public void SetTileSprite()
    {
        Sprite[] sprites = dm.FirstTile;
        TileType[] fourWays = new TileType[4];

        spriteRenderer.sortingOrder = 1000 - (10 * coord.y);
        if (TileData.tileType == TileType.Wall)
        {
            upSprite.sortingOrder = spriteRenderer.sortingOrder + (int)LayerOrder.TopWall;
            cornereLeft.sortingOrder = upSprite.sortingOrder;
            cornerRight.sortingOrder = upSprite.sortingOrder;
            spriteRenderer.sortingOrder += (int)LayerOrder.Wall;
            downSprite.sortingOrder = 1000 - (10 * (coord.y - 1)) + (int)LayerOrder.BottomWall;
        }
        else spriteRenderer.sortingOrder = 0;

        fourWays[0] = dm.map.GetElementAt(coord.x, coord.y + 1).TileData.tileType;
        fourWays[1] = dm.map.GetElementAt(coord.x + 1, coord.y).TileData.tileType;
        fourWays[2] = dm.map.GetElementAt(coord.x, coord.y - 1).TileData.tileType;
        fourWays[3] = dm.map.GetElementAt(coord.x - 1, coord.y).TileData.tileType;

        if (TileData.tileType == TileType.Floor)
        {
            //if (TileData.seed % 3 == 1)
            //    spriteRenderer.sprite = dm.FirstFloor[0];
            //else spriteRenderer.sprite = dm.FirstFloor[1];

            if ((coord.x + coord.y) % 2 == 0)
            {
                if (TileData.seed % 10 <= 8)
                    spriteRenderer.sprite = dm.FirstFloor[0];
                else spriteRenderer.sprite = dm.FirstFloor[2];
            }
            else
            {
                if (TileData.seed % 10 <= 8)
                    spriteRenderer.sprite = dm.FirstFloor[1];
                else spriteRenderer.sprite = dm.FirstFloor[3];
            }
        }
        else if (TileData.tileType == TileType.Wall)
        {
            if (fourWays[0] == TileType.Wall)
            {
                upSprite.gameObject.SetActive(false);

                TileType[] digonalTops = new TileType[2];
                digonalTops[0] = dm.map.GetElementAt(coord.x + 1, coord.y + 1).TileData.tileType;
                digonalTops[1] = dm.map.GetElementAt(coord.x - 1, coord.y + 1).TileData.tileType;

                if (fourWays[1]==TileType.Wall && digonalTops[0] == TileType.Floor) cornerRight.gameObject.SetActive(true);
                else cornerRight.gameObject.SetActive(false);
                if (fourWays[3] == TileType.Wall && digonalTops[1] == TileType.Floor) cornereLeft.gameObject.SetActive(true);
                else cornereLeft.gameObject.SetActive(false);
            }
            else if (fourWays[0] == TileType.Floor)
            {
                if ((fourWays[1]==TileType.Floor) && (fourWays[3] == TileType.Floor))
                    upSprite.sprite = sprites[24];
                else if ((fourWays[1] == TileType.Floor) && (fourWays[3] == TileType.Wall))
                    upSprite.sprite = sprites[22];
                else if ((fourWays[1] == TileType.Wall) && (fourWays[3] == TileType.Floor))
                    upSprite.sprite = sprites[23];
                else if ((fourWays[1] == TileType.Wall) && (fourWays[3] == TileType.Wall))
                    upSprite.sprite = sprites[25];
                upSprite.gameObject.SetActive(true);
            }

            if (fourWays[2] == TileType.Floor)
            {
                if ((fourWays[1] == TileType.Floor) && (fourWays[3] == TileType.Floor))
                {
                    spriteRenderer.sprite = sprites[0];
                    downSprite.sprite = sprites[18];
                }
                else if ((fourWays[1] == TileType.Floor) && (fourWays[3] == TileType.Wall))
                {
                    spriteRenderer.sprite = sprites[1];
                    downSprite.sprite = sprites[19];
                }
                else if ((fourWays[1] == TileType.Wall) && (fourWays[3] == TileType.Floor))
                {
                    spriteRenderer.sprite = sprites[9];
                    downSprite.sprite = sprites[20];
                }
                else if ((fourWays[1] == TileType.Wall) && (fourWays[3] == TileType.Wall))
                {
                    spriteRenderer.sprite = sprites[10];
                    downSprite.sprite = sprites[21];
                }
                downSprite.gameObject.SetActive(true);
            }
            else if (fourWays[2] == TileType.Wall)
            {
                downSprite.gameObject.SetActive(false);

                TileType[] digonalBottoms = new TileType[2];
                digonalBottoms[0] = dm.map.GetElementAt(coord.x + 1, coord.y - 1).TileData.tileType;
                digonalBottoms[1] = dm.map.GetElementAt(coord.x - 1, coord.y - 1).TileData.tileType;

                if ((fourWays[1] == TileType.Floor) && (fourWays[3] == TileType.Floor))
                {
                    if ((digonalBottoms[0] == TileType.Wall) && (digonalBottoms[1] == TileType.Wall))
                        spriteRenderer.sprite = sprites[4];
                    else if ((digonalBottoms[0] == TileType.Wall) && (digonalBottoms[1] == TileType.Floor))
                        spriteRenderer.sprite = sprites[3];
                    else if ((digonalBottoms[0] == TileType.Floor) && (digonalBottoms[1] == TileType.Wall))
                        spriteRenderer.sprite = sprites[2];
                    else if ((digonalBottoms[0] == TileType.Floor) && (digonalBottoms[1] == TileType.Floor))
                        spriteRenderer.sprite = sprites[5];
                }
                else if ((fourWays[1] == TileType.Floor) && (fourWays[3] == TileType.Wall))
                {
                    if (digonalBottoms[0] == TileType.Wall && digonalBottoms[1]==TileType.Wall)
                        spriteRenderer.sprite = sprites[7];
                    else if (digonalBottoms[0] == TileType.Wall && digonalBottoms[1] == TileType.Floor)
                        spriteRenderer.sprite = sprites[6];
                    else if (digonalBottoms[1] == TileType.Floor)
                        spriteRenderer.sprite = sprites[6];
                    else spriteRenderer.sprite = sprites[8];
                }
                else if ((fourWays[1] == TileType.Wall) && (fourWays[3] == TileType.Floor))
                {
                    if (digonalBottoms[0] == TileType.Floor)
                        spriteRenderer.sprite = sprites[12];
                    else if (digonalBottoms[1] == TileType.Wall)
                        spriteRenderer.sprite = sprites[11];
                    else spriteRenderer.sprite = sprites[13];
                }
                else if ((fourWays[1] == TileType.Wall) && (fourWays[3] == TileType.Wall))
                {
                    if ((digonalBottoms[0]==TileType.Floor) && (digonalBottoms[1]==TileType.Floor))
                        spriteRenderer.sprite = sprites[14];
                    else if ((digonalBottoms[0] == TileType.Wall) && (digonalBottoms[1] == TileType.Floor))
                        spriteRenderer.sprite = sprites[15];
                    else if ((digonalBottoms[0] == TileType.Floor) && (digonalBottoms[1] == TileType.Wall))
                        spriteRenderer.sprite = sprites[16];
                    else if ((digonalBottoms[0] == TileType.Wall) && (digonalBottoms[1] == TileType.Wall))
                        spriteRenderer.sprite = sprites[17];
                }
            }
        }
        //rangeIndicator.sortingOrder = 1000 - (10 * coord.y) + 10 + (int)LayerOrder.BottomWall;
    }

    /*
    public void SetTileSprite()
    {
        if (TileData.tileType == TileType.Floor)
            spriteRenderer.sprite = GameManager.Instance.pencilTiles1[0];
        else if (TileData.tileType == TileType.Wall)
        {
            if (dm.map.GetElementAt(coord.x, coord.y + 1).TileData.tileType == TileType.Wall)
            {
                upSprite.gameObject.SetActive(false);
            }
            else if (dm.map.GetElementAt(coord.x, coord.y + 1).TileData.tileType == TileType.Floor)
            {
                upSprite.gameObject.SetActive(true);
            }

            if (dm.map.GetElementAt(coord.x - 1, coord.y).TileData.tileType == TileType.Wall)
            {
                if (dm.map.GetElementAt(coord.x + 1, coord.y).TileData.tileType == TileType.Wall)
                {

                    if (dm.map.GetElementAt(coord.x, coord.y - 1).TileData.tileType == TileType.Wall)
                    {
                        downSprite.gameObject.SetActive(false);
                        if (dm.map.GetElementAt(coord.x - 1, coord.y - 1).TileData.tileType == TileType.Floor) 
                            spriteRenderer.sprite = GameManager.Instance.pencilTiles1[9];
                        else if (dm.map.GetElementAt(coord.x + 1, coord.y - 1).TileData.tileType == TileType.Floor)
                            spriteRenderer.sprite = GameManager.Instance.pencilTiles1[10];
                        else
                            spriteRenderer.sprite = GameManager.Instance.pencilTiles1[1];
                    }
                    else if (dm.map.GetElementAt(coord.x, coord.y - 1).TileData.tileType == TileType.Floor)
                    {
                        downSprite.gameObject.SetActive(true);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[7];
                    }
                }
                else if (dm.map.GetElementAt(coord.x + 1, coord.y).TileData.tileType == TileType.Floor)
                {

                    if (dm.map.GetElementAt(coord.x, coord.y - 1).TileData.tileType == TileType.Wall)
                    {
                        downSprite.gameObject.SetActive(false);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[5];

                    }
                    else if (dm.map.GetElementAt(coord.x, coord.y - 1).TileData.tileType == TileType.Floor)
                    {
                        downSprite.gameObject.SetActive(true);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[3];
                    }
                }
            }
            else if (dm.map.GetElementAt(coord.x - 1, coord.y).TileData.tileType == TileType.Floor)
            {
                if (dm.map.GetElementAt(coord.x + 1, coord.y).TileData.tileType == TileType.Wall)
                {

                    if (dm.map.GetElementAt(coord.x, coord.y - 1).TileData.tileType == TileType.Wall)
                    {
                        downSprite.gameObject.SetActive(false);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[8];
                    }
                    else if (dm.map.GetElementAt(coord.x, coord.y - 1).TileData.tileType == TileType.Floor)
                    {
                        downSprite.gameObject.SetActive(true);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[6];
                    }
                }
                else if (dm.map.GetElementAt(coord.x + 1, coord.y).TileData.tileType == TileType.Floor)
                {

                    if (dm.map.GetElementAt(coord.x, coord.y - 1).TileData.tileType == TileType.Wall)
                    {
                        downSprite.gameObject.SetActive(false);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[4];
                    }
                    else if (dm.map.GetElementAt(coord.x, coord.y - 1).TileData.tileType == TileType.Floor)
                    {
                        downSprite.gameObject.SetActive(true);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[2];
                    }
                }
            }
        }
    }
    */

    public void SetAvailable()
    {
        rangeIndicator.gameObject.SetActive(true);
        rangeIndicator.sprite = blueRange;
        IsAvailable = true;
    }
    public void SetUnavailable()
    {
        rangeIndicator.gameObject.SetActive(true);
        rangeIndicator.sprite = redRange;
        IsAvailable = false;
    }
    public void TurnOffRangeIndicator()
    {
        rangeIndicator.gameObject.SetActive(false);
        IsAvailable = false;
    }
}
