using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DungeonManager : MonoBehaviour
{
    MapGenerator mapGenerator = new MapGenerator();
    public Tile[,] map = new Tile[80, 80];
    List<Unit> units = new List<Unit>();
    public Fog[,] FogMap { get; private set; }
    //public Fog[,] FogMap { get { return fogMap; } }

    float curTurn = 0;

    int order = 0;
    int maxOrder = 0;

    Player player;
    public Player Player { get { return player; } }
    [SerializeField] DungeonUIManager dungeonUIManager;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        //GenerateSimpleMap();

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                map[i, j] = Instantiate(GameManager.Instance.tilePrefab);
                map[i, j].Init(TileType.Wall, i, j);
                map[i, j].dm = this;
            }
        }
        mapGenerator.GenerateRooms(map);
        for (int i = 1; i < map.GetLength(0) - 1; i++)
        {
            for (int j = 1; j < map.GetLength(1) - 1; j++)
            {
                map[i, j].SetTileSprite();
            }
        }

        FogMap = new Fog[map.GetLength(0), map.GetLength(1)];
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                GameObject tempf = Instantiate(GameManager.Instance.fogPrefab);
                tempf.transform.position = new Vector2(0 + i, 0 + j);
                FogMap[i, j] = tempf.GetComponent<Fog>();
                FogMap[i, j].Init(this, i, j);
                //FogMap[i, j].Clear();////////////////////
            }
        }
        Physics2D.SyncTransforms();
        player = Instantiate(GameManager.Instance.warriorPrefab);
        player.SetUnitListIndex(units.Count);
        units.Add(player);
        player.Init(this, mapGenerator.Rooms[0].center);
        //Monster temp = Instantiate(GameManager.Instance.testMob1Prefab);
        //temp.SetListNumber(units.Count);
        //units.Add(temp);
        //temp.Init(this,mapGenerator.Rooms[1].center);

        for(int i=0; i < mapGenerator.Rooms.Count; i++)
        {
            Coordinate c = mapGenerator.Rooms[i].center;
            if (map[c.x, c.y].IsReachableTile())
            {
                Monster temp = Instantiate(GameManager.Instance.testMob1Prefab);
                temp.SetUnitListIndex(units.Count);
                units.Add(temp);
                temp.Init(this, c);
            }
        }

        dungeonUIManager.Init();

        maxOrder = units.Count;
        units[order].StartTurn();
    }

    public void EndTurn()
    {
        for(int i=units.Count-1; i>=0; i--)
        {
            if (units[i].IsDead)
            {
                RemoveUnit(i);
            }
        }

        order++;
        if (order < maxOrder)
        {
            units[order].StartTurn();
        }
        else
        {
            order = 0;
            maxOrder = 0;
            ManageTurn();
            units[order].StartTurn();
        }
    }

    public void UpdateUnitRenderers()
    {
        for (int i = 0; i < units.Count; i++)
        {
            if (FogMap[units[i].Coord.x, units[i].Coord.y].IsOn)
                units[i].MySpriteRenderer.enabled = false;
            else
                units[i].MySpriteRenderer.enabled = true;
        }
    }

    void ManageTurn()
    {
        //units.Sort(SortByTurn);
        units = units.OrderBy(x => x.TurnIndicator).ToList();
 
        float min = units[0].TurnIndicator;
        curTurn += min;
        Debug.Log("Turn: " + curTurn.ToString());

        int trimAmount = Mathf.FloorToInt(min);
        min -= trimAmount;
        for (int i = 0; i < units.Count; i++)
        {
            units[i].SetUnitListIndex(i);
            units[i].TrimTurn(trimAmount);
            if (units[i].TurnIndicator == min)
                maxOrder++;
            else break;
        }

    }

    public void RemoveUnit(int listNumber)
    {
        Destroy(units[listNumber].gameObject);
        units.RemoveAt(listNumber);
        if (listNumber < maxOrder)
        {
            maxOrder--;
            if (listNumber < order)
                order--;
        }
    }
    
    int SortByTurn(Unit a, Unit b)
    {
        if (a.TurnIndicator < b.TurnIndicator) return -1;
        else if (a.TurnIndicator > b.TurnIndicator) return 1;
        else return 0;
    }

    void GenerateSimpleMap()
    {   
        Tile[,] map = GameManager.Instance.dungeonManager.map;

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                map[i, j] = Instantiate(GameManager.Instance.tilePrefab);
                map[i, j].transform.position = new Vector2(0 + i, 0 + j);
                if (Random.Range(0, 3) == 0)
                    map[i, j].Init(TileType.Wall, i, j);
                else map[i, j].Init(TileType.Floor, i, j);

                //if((i==0)&&(i==1)) map[i, j].Init(TileType.Wall, i, j);
                //if ((i == 1) && (i == 1)) map[i, j].Init(TileType.Wall, i, j);
                //if ((i == 1) && (i == 0)) map[i, j].Init(TileType.Wall, i, j);

                map[i, j].dm = this;
            }
        }
    }

    public void UpdatePlayerHpBar()
    {
        dungeonUIManager.UpdateHpBar();
    }

    public Tile GetTileByCoordinate(Coordinate c)
    {
        if (c.IsValidCoordForMap(map))
        {
            return map[c.x, c.y];
        }
        else throw new System.Exception("Invalid coordinate");
    }
    public bool IsReachable(Coordinate c)
    {
        if (c.IsValidCoordForMap(map))
        {
            if ((map[c.x, c.y].type != TileType.Wall) && (map[c.x, c.y].unit == null))
                return true;
            else return false;
        }
        else throw new System.Exception("Invalid coordinate");
    }
    public bool IsValidIndexForMap(int x, int y)
    {
        if ((x >= 0 && x < map.GetLength(0)) && (y >= 0 && y < map.GetLength(1)))
            return true;
        else return false;
    }
    public bool IsValidCoordForMap(Coordinate c)
    {
        if ((c.x >= 0 && c.x < map.GetLength(0)) && (c.y >= 0 && c.y < map.GetLength(1)))
            return true;
        else return false;
    }

    public void TestOut()
    {
        Debug.Log("On");
    }
}
