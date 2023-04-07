using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileType type = TileType.Floor;
    public AreaType Area { get; private set; } = AreaType.None;

    TileData tileData;

    public Unit unit;
    public Stack<ItemObject> items = new();

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject upSprite;
    [SerializeField] GameObject downSprite;
    public DungeonManager dm;
    Coordinate coord;
    public Coordinate Coord { get { return coord; } }

    [SerializeField] SpriteRenderer rangeIndicator;
    readonly Color transparentRed = new(1, 0, 0, 0.33f);
    readonly Color transparentBlue = new(0, 0, 1, 0.33f);
    public GameObject targetMark;
    public bool AvailableRange { get; private set; } = false;

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
        /*
        if (t == TileType.Floor)
            spriteRenderer.sprite = GameManager.Instance.tileSprites[0];
        else if (t == TileType.Wall)
            spriteRenderer.sprite = GameManager.Instance.tileSprites[1];
        */
    }
    public void SetTileSprite()
    {
        if (type == TileType.Floor)
            spriteRenderer.sprite = GameManager.Instance.pencilTiles1[0];
        else if (type == TileType.Wall)
        {
            if (dm.map[coord.x, coord.y + 1].type == TileType.Wall)
            {
                upSprite.SetActive(false);
            }
            else if (dm.map[coord.x, coord.y + 1].type == TileType.Floor)
            {
                upSprite.SetActive(true);
            }

            if (dm.map[coord.x - 1, coord.y].type == TileType.Wall)
            {
                if (dm.map[coord.x + 1, coord.y].type == TileType.Wall)
                {

                    if (dm.map[coord.x, coord.y - 1].type == TileType.Wall)
                    {
                        downSprite.SetActive(false);
                        if (dm.map[coord.x - 1, coord.y - 1].type == TileType.Floor) 
                            spriteRenderer.sprite = GameManager.Instance.pencilTiles1[9];
                        else if (dm.map[coord.x + 1, coord.y - 1].type == TileType.Floor)
                            spriteRenderer.sprite = GameManager.Instance.pencilTiles1[10];
                        else
                            spriteRenderer.sprite = GameManager.Instance.pencilTiles1[1];
                    }
                    else if (dm.map[coord.x, coord.y - 1].type == TileType.Floor)
                    {
                        downSprite.SetActive(true);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[7];
                    }
                }
                else if (dm.map[coord.x + 1, coord.y].type == TileType.Floor)
                {

                    if (dm.map[coord.x, coord.y - 1].type == TileType.Wall)
                    {
                        downSprite.SetActive(false);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[5];

                    }
                    else if (dm.map[coord.x, coord.y - 1].type == TileType.Floor)
                    {
                        downSprite.SetActive(true);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[3];
                    }
                }
            }
            else if (dm.map[coord.x - 1, coord.y].type == TileType.Floor)
            {
                if (dm.map[coord.x + 1, coord.y].type == TileType.Wall)
                {

                    if (dm.map[coord.x, coord.y - 1].type == TileType.Wall)
                    {
                        downSprite.SetActive(false);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[8];
                    }
                    else if (dm.map[coord.x, coord.y - 1].type == TileType.Floor)
                    {
                        downSprite.SetActive(true);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[6];
                    }
                }
                else if (dm.map[coord.x + 1, coord.y].type == TileType.Floor)
                {

                    if (dm.map[coord.x, coord.y - 1].type == TileType.Wall)
                    {
                        downSprite.SetActive(false);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[4];
                    }
                    else if (dm.map[coord.x, coord.y - 1].type == TileType.Floor)
                    {
                        downSprite.SetActive(true);
                        spriteRenderer.sprite = GameManager.Instance.pencilTiles1[2];
                    }
                }
            }
        }
    }

    public void SetAreaType(AreaType t)
    {
        Area = t;
    }

    public void SetAvailable()
    {
        rangeIndicator.gameObject.SetActive(true);
        rangeIndicator.color = transparentBlue;
        AvailableRange = true;
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
        AvailableRange = false;
    }
}
