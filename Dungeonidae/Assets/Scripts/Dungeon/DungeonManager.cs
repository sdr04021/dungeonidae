using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DungeonManager : MonoBehaviour
{
    //[field: SerializeField] public Tile[,] Map { get; private set; }
    public Arr2D<Tile> map;
    //public Fog[,] FogMap { get; private set; }
    public Arr2D<Fog> fogMap;

    float curTurn = 0;

    int order = 0;
    int maxOrder = 0;

    Player player;
    public Player Player { get { return player; } }
    [SerializeField] DungeonUIManager dunUI;

    DungeonData dungeonData;

    public void StartNewFloor()
    {
        dungeonData = new();
        dungeonData.floor = GameManager.Instance.saveData.Floors.Count;
        dungeonData.SetRandom();
        _ = new MapGenerator(dungeonData);
        
        GameManager.Instance.saveData.Floors.Add(dungeonData);

        BuildCurrentFloor();
    }
    public void LoadFloor()
    {
        dungeonData = GameManager.Instance.saveData.GetCurrentDungeonData();
        dungeonData.SetRandom();
        _ = new MapGenerator(dungeonData);
        BuildCurrentFloor();
    }
    void BuildCurrentFloor()
    {
        foreach(Coordinate c in dungeonData.observedFog)
        {
            dungeonData.fogData[c.x][c.y].IsObserved = true;
        }

        for (int i = 0; i < dungeonData.mapData.Count; i++)
        {
            for (int j = 0; j < dungeonData.mapData[0].Count; j++)
            {
                map.GetElementAt(i,j).Init(this, dungeonData.mapData[i][j], i, j);
                fogMap.GetElementAt(i, j).Init(this, dungeonData.fogData[i][j], i, j);
            }
        }
        map.arrSize.x = dungeonData.mapData.Count;
        map.arrSize.y = dungeonData.mapData[0].Count;

        for (int i = 1; i < map.arrSize.x - 1; i++)
        {
            for (int j = 1; j < map.arrSize.y - 1; j++)
            {
                map.x[i].y[j].SetTileSprite();
                fogMap.GetElementAt(i, j).SetSprite();
            }
        }

        //ClearAllFog();///////////////////////////////////
        //ObserveAllFog();

        Physics2D.SyncTransforms();

        if (GameManager.Instance.saveData.playerData == null)
            GenerateUnits();
        else
            LoadUnits();
        LoadFieldItems();

        dunUI.Init();
        maxOrder = dungeonData.unitList.Count;
        dungeonData.unitList[order].Owner.StartTurn();
        UpdateUnitRenderers();
    }
    void ObserveAllFog()
    {
        for (int i = 1; i < map.arrSize.x - 1; i++)
        {
            for (int j = 1; j < map.arrSize.y - 1; j++)
            {
                fogMap.GetElementAt(i, j).Clear();
                fogMap.GetElementAt(i, j).Cover();
            }
        }
    }
    void ClearAllFog()
    {
        for (int i = 1; i < map.arrSize.x - 1; i++)
        {
            for (int j = 1; j < map.arrSize.y - 1; j++)
            {
                fogMap.GetElementAt(i, j).Clear();
            }
        }
    }
    void GenerateUnits()
    {
        player = Instantiate(GameManager.Instance.warriorPrefab);
        GameManager.Instance.saveData.playerData = player.UnitData;
        player.Init(this, dungeonData.rooms[0].center);
        dungeonData.unitList.Add(player.UnitData);
        player.SetUnitListIndex(0);

        for (int i = 0; i < dungeonData.rooms.Count; i++)
        {
            Coordinate c = dungeonData.rooms[i].center;
            if (map.GetElementAt(c.x, c.y).IsReachableTile())
            {
                Monster temp = Instantiate(GameManager.Instance.testMob1Prefab);
                temp.SetUnitListIndex(dungeonData.unitList.Count);
                dungeonData.unitList.Add(temp.UnitData);
                temp.Init(this, c);
            }
        }
    }
    void LoadUnits()
    {
        for(int i=0; i<dungeonData.unitList.Count; i++)
        {
            if (dungeonData.unitList[i].team == Team.Player)
            {
                player = Instantiate(GameManager.Instance.warriorPrefab);
                player.SetUnitData(this, dungeonData.unitList[i]);
                //GameManager.Instance.saveData.playerData = dungeonData.unitList[i];
                player.SetUnitListIndex(i);
            }
            else
            {
                Monster temp = Instantiate(GameManager.Instance.testMob1Prefab);
                temp.SetUnitData(this, dungeonData.unitList[i]);
                temp.SetUnitListIndex(i);
            }
        }
    }

    void LoadFieldItems()
    {
        for(int i=0; i<dungeonData.fieldItemList.Count; i++)
        {
            ItemData item = dungeonData.fieldItemList[i].GetItemData();
            ItemObject itemObj = Instantiate(GameManager.Instance.itemObjectPrefab, new Vector3(item.coord.x, item.coord.y - 0.2f, 0), Quaternion.identity);
            itemObj.Init(this, item.coord, dungeonData.fieldItemList[i].GetItemData());
            map.GetElementAt(item.coord).items.Push(itemObj);
        }
    }

    public void EndTurn()
    {
        for(int i= dungeonData.unitList.Count-1; i>=0; i--)
        {
            if (dungeonData.unitList[i].Owner.IsDead)
            {
                RemoveUnit(i);
            }
        }

        order++;
        if (order < maxOrder)
        {
            dungeonData.unitList[order].Owner.StartTurn();
        }
        else
        {
            order = 0;
            maxOrder = 0;
            ManageTurn();
            dungeonData.unitList[order].Owner.StartTurn();
        }
    }

    public void UpdateUnitRenderers()
    {
        for (int i = 0; i < dungeonData.unitList.Count; i++)
        {
            if (fogMap.GetElementAt(dungeonData.unitList[i].coord.x, dungeonData.unitList[i].coord.y).FogData.IsOn)
            {
                dungeonData.unitList[i].Owner.MySpriteRenderer.enabled = false;
                dungeonData.unitList[i].Owner.canvas.enabled = false;
            }
            else
            {
                dungeonData.unitList[i].Owner.MySpriteRenderer.enabled = true;
                dungeonData.unitList[i].Owner.canvas.enabled = true;
            }
        }
    }

    void ManageTurn()
    {
        //units.Sort(SortByTurn);
        dungeonData.unitList = dungeonData.unitList.OrderBy(x => x.turnIndicator).ToList();
 
        float min = dungeonData.unitList[0].turnIndicator;

        int trimAmount = Mathf.FloorToInt(min);
        min -= trimAmount;
        curTurn += trimAmount;
        Debug.Log("Turn: " + curTurn.ToString());
        for (int i = 0; i < dungeonData.unitList.Count; i++)
        {
            dungeonData.unitList[i].Owner.SetUnitListIndex(i);
            dungeonData.unitList[i].Owner.TrimTurn(trimAmount);
            if (dungeonData.unitList[i].turnIndicator == min)
                maxOrder++;
            else break;
        }
    }

    public void RemoveUnit(int listNumber)
    {
        Destroy(dungeonData.unitList[listNumber].Owner.gameObject);
        dungeonData.unitList.RemoveAt(listNumber);
        if (listNumber < maxOrder)
        {
            maxOrder--;
            if (listNumber < order)
                order--;
        }
    }
    
    int SortByTurn(Unit a, Unit b)
    {
        if (a.UnitData.turnIndicator < b.UnitData.turnIndicator) return -1;
        else if (a.UnitData.turnIndicator > b.UnitData.turnIndicator) return 1;
        else return 0;
    }

    /*
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
    */

    public Tile GetTileByCoordinate(Coordinate c)
    {
        if (c.IsValidCoordForMap(map))
        {
            return map.GetElementAt(c.x, c.y);
        }
        else return null;
    }
    public bool IsValidIndexForMap(int x, int y)
    {
        if ((x >= 0 && x < map.arrSize.x) && (y >= 0 && y < map.arrSize.y))
            return true;
        else return false;
    }
    public bool IsValidCoordForMap(Coordinate c)
    {
        if ((c.x >= 0 && c.x < map.arrSize.x) && (c.y >= 0 && c.y < map.arrSize.y))
            return true;
        else return false;
    }

    public void TestOut()
    {
        Debug.Log("On");
    }
}
