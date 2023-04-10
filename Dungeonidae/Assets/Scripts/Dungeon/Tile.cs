using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileData TileData { get; private set; }

    public Unit unit;
    public Stack<ItemObject> items = new();

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject upSprite;
    [SerializeField] GameObject downSprite;
    DungeonManager dm;
    Coordinate coord;
    public Coordinate Coord { get { return coord; } }

    [SerializeField] SpriteRenderer rangeIndicator;
    readonly Color transparentRed = new(1, 0, 0, 0.33f);
    readonly Color transparentBlue = new(0, 0, 1, 0.33f);
    public GameObject targetMark;

    public void Init(DungeonManager dm, TileData tileData, int x, int y)
    {
        this.dm = dm;
        TileData = tileData;
        coord = new Coordinate(x, y);
        transform.position = new Vector2(x, y);
    }

    public bool IsReachableTile()
    {
        if ((TileData.tileType != TileType.Wall) && (unit == null))
            return true;
        else return false;
    }

    public void SetTileSprite()
    {
        if (TileData.tileType == TileType.Floor)
            spriteRenderer.sprite = GameManager.Instance.pencilTiles1[0];
        else if (TileData.tileType == TileType.Wall)
        {
            if (dm.Map[coord.x, coord.y + 1].TileData.tileType == TileType.Wall)
            {
                upSprite.SetActive(false);
            }
            else if (dm.Map[coord.x, coord.y + 1].TileData.tileType == TileType.Floor)
            {
                upSprite.SetActive(true);
            }

            if (dm.Map[coord.x - 1, coord.y].TileData.tileType == TileType.Wall)
            {
                if (dm.Map[coord.x + 1, coord.y].TileData.tileType == TileType.Wall)
                {

                    if (dm.Map[coord.x, coord.y - 1].TileData.tileType == TileType.Wall)
                    {
                        downSprite.SetActive(false);
                        if (dm.Map[coord.x - 1, coord.y - 1].TileData.tileType == TileType.Floor) 
                            spriteRenderer.sprite = GameManager.Instance.pencilTiles1[9];
                        else if (dm.Map[coord.x + 1, coord.y - 1].TileData.tileType == TileType.Floor)
                            spriteRenderer.sprite = GameManager.Instance.pencilTiles1[10];
                        else
                            spriteRenderer.sprite = GameManager.Instance.pencilTiles1[1];
                    }
                    else if (dm.Map[coord.x, coord.y - 1].TileData.tileType == TileType.Floor)
                    {
                        downSprite.SetActive(true);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[7];
                    }
                }
                else if (dm.Map[coord.x + 1, coord.y].TileData.tileType == TileType.Floor)
                {

                    if (dm.Map[coord.x, coord.y - 1].TileData.tileType == TileType.Wall)
                    {
                        downSprite.SetActive(false);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[5];

                    }
                    else if (dm.Map[coord.x, coord.y - 1].TileData.tileType == TileType.Floor)
                    {
                        downSprite.SetActive(true);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[3];
                    }
                }
            }
            else if (dm.Map[coord.x - 1, coord.y].TileData.tileType == TileType.Floor)
            {
                if (dm.Map[coord.x + 1, coord.y].TileData.tileType == TileType.Wall)
                {

                    if (dm.Map[coord.x, coord.y - 1].TileData.tileType == TileType.Wall)
                    {
                        downSprite.SetActive(false);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[8];
                    }
                    else if (dm.Map[coord.x, coord.y - 1].TileData.tileType == TileType.Floor)
                    {
                        downSprite.SetActive(true);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[6];
                    }
                }
                else if (dm.Map[coord.x + 1, coord.y].TileData.tileType == TileType.Floor)
                {

                    if (dm.Map[coord.x, coord.y - 1].TileData.tileType == TileType.Wall)
                    {
                        downSprite.SetActive(false);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[4];
                    }
                    else if (dm.Map[coord.x, coord.y - 1].TileData.tileType == TileType.Floor)
                    {
                        downSprite.SetActive(true);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[2];
                    }
                }
            }
        }
    }

    public void SetAvailable()
    {
        rangeIndicator.gameObject.SetActive(true);
        rangeIndicator.color = transparentBlue;
    }
    public void SetUnavailable()
    {
        rangeIndicator.gameObject.SetActive(true);
        rangeIndicator.color = transparentRed;
    }
    public void TurnOffRangeIndicator()
    {
        rangeIndicator.gameObject.SetActive(false);
        targetMark.SetActive(false);
    }
}
