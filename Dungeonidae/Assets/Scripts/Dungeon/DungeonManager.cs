using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DungeonManager : MonoBehaviour
{
    AsyncOperationHandle<GameObject> unitLoadHandle;
    Dictionary<string, AsyncOperationHandle<GameObject>> monsterHandles = new();

    public Sprite[] FirstTile;

    public Arr2D<Tile> map;
    public Arr2D<Fog> fogMap;

    float curTurn = 0;

    int order = 0;
    int maxOrder = 0;

    Player player;
    public Player Player { get { return player; } }
    [SerializeField] DungeonUIManager dunUI;

    DungeonData dungeonData;

    public void StartFirstFloor()
    {
        player = Instantiate(GameManager.Instance.warriorPrefab);
        GameManager.Instance.saveData.playerData = player.UnitData;
        player.Init(this, new Coordinate(0, 0));
        StartNewFloor();
    }
    public void LoadFloor()
    {
        player = Instantiate(GameManager.Instance.warriorPrefab);
        player.SetUnitData(this, GameManager.Instance.saveData.playerData);
        StartExistingFloor(0);
    }

    void StartNewFloor()
    {
        SaveData saveData = GameManager.Instance.saveData;
        dungeonData = new();
        saveData.Floors.Add(dungeonData);
        saveData.UpdateFloorSeeds();

        dungeonData.floor = saveData.Floors.Count - 1;
        _ = new MapGenerator(dungeonData);
        order = 0;
        BuildCurrentFloor();

        player.UnitData.coord = dungeonData.rooms[dungeonData.stairRooms.Item1].center;
        player.SetPosition();

        dungeonData.unitList.Add(player.UnitData);

        GenerateUnits();

        dunUI.Init();
        maxOrder = dungeonData.unitList.Count;
        Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, Camera.main.transform.position.z);
        dungeonData.unitList[order].Owner.StartTurn();
        UpdateUnitRenderers();
    }
    public void StartExistingFloor(int movedDir)
    {
        dungeonData = GameManager.Instance.saveData.GetCurrentDungeonData();
        _ = new MapGenerator(dungeonData);
        order = 0;
        BuildCurrentFloor();

        if (movedDir == 1) player.UnitData.coord = dungeonData.rooms[dungeonData.stairRooms.Item1].center;
        else if (movedDir == -1) player.UnitData.coord = dungeonData.rooms[dungeonData.stairRooms.Item2].center;
        player.SetPosition();

        dungeonData.unitList.Add(player.UnitData);
        dungeonData.unitList = dungeonData.unitList.OrderBy(x => x.TurnIndicator).ToList();

        LoadUnits();
        LoadFieldItems();

        dunUI.Init();
        maxOrder = dungeonData.unitList.Count;
        dungeonData.unitList[order].Owner.StartTurn();
        UpdateUnitRenderers();
    }

    public void MoveFloor(int movedDir)
    {
        SaveData saveData = GameManager.Instance.saveData;
        movedDir = (int)Mathf.Sign(movedDir);
        if ((saveData.currentFloor == 0) && (movedDir < 0)) return;
        saveData.currentFloor += movedDir;
        dunUI.ShowFloorCurtain(saveData.currentFloor);
        DestroyAllObjects();
        dungeonData.unitList.Remove(GameManager.Instance.saveData.playerData);

        if(saveData.currentFloor >= saveData.Floors.Count)
        {
            saveData.playerData.TurnIndicator = 0;
            StartNewFloor();
        }
        else
        {
            saveData.playerData.TurnIndicator = 1;
            StartExistingFloor(movedDir);
        }
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
        //ObserveAllFog();////////////////////////////////

        Physics2D.SyncTransforms();
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
    GameObject LoadMonsterPrefab(string Key)
    {

        GameObject prefab = null;
        if (monsterHandles.ContainsKey(Key))
        {
            prefab = monsterHandles[Key].Result;
        }
        else
        {
            monsterHandles.Add(Key, Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Monsters/" + Key + ".prefab"));
            monsterHandles[Key].WaitForCompletion();
            if (monsterHandles[Key].Status == AsyncOperationStatus.Succeeded)
                prefab = monsterHandles[Key].Result;
        }
        return prefab;
    }
    void GenerateUnits()
    {
        SaveData saveData = GameManager.Instance.saveData;
        for (int i = 0; i < dungeonData.rooms.Count; i++)
        {
            Coordinate c = dungeonData.rooms[i].center;
            List<Coordinate> genPoints = GlobalMethods.RangeByStep(c, 1);
            for (int j = 0; j < genPoints.Count; j++)
            {
                Tile tile = map.GetElementAt(genPoints[j]);
                if (tile.IsReachableTile())
                {
                    int pick = Random.Range(0, 3);
                    GameObject prefab = LoadMonsterPrefab(GameManager.Instance.StringData.Monsters[saveData.MonsterLayout[(saveData.currentFloor + pick) % (saveData.MonsterLayout.Count)]]);
                    if(prefab != null)
                    {
                        Monster temp  = Instantiate(prefab).GetComponent<Monster>();
                        dungeonData.unitList.Add(temp.UnitData);
                        temp.Init(this, genPoints[j]);
                    }
                    break;
                }
            }
        }

        Stair stairUp = Instantiate(GameManager.Instance.stairUpPrefab);
        stairUp.Init(this, dungeonData.rooms[dungeonData.stairRooms.Item1].center);
        dungeonData.dungeonObjectList.Add(stairUp.DungeonObjectData);
        map.GetElementAt(stairUp.DungeonObjectData.coord).dungeonObjects.Add(stairUp);
        Stair stairDown = Instantiate(GameManager.Instance.stairDownPrefab);
        stairDown.Init(this, dungeonData.rooms[dungeonData.stairRooms.Item2].center);
        dungeonData.dungeonObjectList.Add(stairDown.DungeonObjectData);
        map.GetElementAt(stairDown.DungeonObjectData.coord).dungeonObjects.Add(stairDown);
    }
    void LoadUnits()
    {
        for(int i=0; i<dungeonData.unitList.Count; i++)
        {
            if (dungeonData.unitList[i].team != Team.Player)
            {
                Monster temp = Instantiate(GameManager.Instance.testMob1Prefab);
                temp.SetUnitData(this, dungeonData.unitList[i]);
            }
        }
        for(int i=0; i<dungeonData.dungeonObjectList.Count; i++)
        {
            if (dungeonData.dungeonObjectList[i].ClassType == typeof(Stair))
            {
                if (i == 0)
                {
                    Stair stairUp= Instantiate(GameManager.Instance.stairUpPrefab);
                    stairUp.Load(this, dungeonData.dungeonObjectList[i]);
                    map.GetElementAt(stairUp.DungeonObjectData.coord).dungeonObjects.Add(stairUp);
                }
                else if (i == 1)
                {
                    Stair stairDown = Instantiate(GameManager.Instance.stairDownPrefab);
                    stairDown.Load(this, dungeonData.dungeonObjectList[i]);
                    map.GetElementAt(stairDown.DungeonObjectData.coord).dungeonObjects.Add(stairDown);
                }
            }
        }
    }

    void LoadFieldItems()
    {
        for(int i=0; i<dungeonData.fieldItemList.Count; i++)
        {
            ItemData item = dungeonData.fieldItemList[i];
            ItemObject itemObj = Instantiate(GameManager.Instance.itemObjectPrefab, new Vector3(item.coord.x, item.coord.y - 0.2f, 0), Quaternion.identity);
            itemObj.Init(this, item.coord, dungeonData.fieldItemList[i]);
            map.GetElementAt(item.coord).items.Push(itemObj);
        }
    }

    void RegenerateMobs()
    {
        Room room = dungeonData.rooms[Random.Range(0, dungeonData.rooms.Count)];
        Coordinate genPoint = new(Random.Range(room.Left, room.Right), Random.Range(room.Bottom, room.Top));
        List<Coordinate> genPoints = GlobalMethods.RangeByStep(genPoint, 1);
        for (int i = 0; i < genPoints.Count; i++)
        {
            Tile tile = map.GetElementAt(genPoints[i]);
            if (tile.IsReachableTile() && !player.TilesInSight.Contains(tile.Coord))
            {

                break;
            }
        }
    }

    public void EndTurn()
    {
        for(int i= dungeonData.unitList.Count-1; i>=0; i--)
        {
            if (dungeonData.unitList[i].Owner.IsDead)
            {
                RemoveUnit(dungeonData.unitList[i]);
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
            if (dungeonData.unitList[i].team == Team.Player) continue;

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
        dungeonData.unitList = dungeonData.unitList.OrderBy(x => x.TurnIndicator).ToList();
 
        float min = dungeonData.unitList[0].TurnIndicator;

        int trimAmount = Mathf.FloorToInt(min);
        min -= trimAmount;
        curTurn += trimAmount;
        Debug.Log("Turn: " + curTurn.ToString());
        for (int i = 0; i < dungeonData.unitList.Count; i++)
        {
            dungeonData.unitList[i].Owner.TrimTurn(trimAmount);
            if (dungeonData.unitList[i].TurnIndicator == min)
                maxOrder++;
            else break;
        }
    }

    public void RemoveUnit(UnitData unitData)
    {
        int index = dungeonData.unitList.IndexOf(unitData);
        Destroy(unitData.Owner.gameObject);
        dungeonData.unitList.RemoveAt(index);
        if (index < maxOrder)
        {
            maxOrder--;
            if (index < order)
                order--;
        }
    }
    
    public void DestroyAllObjects()
    {
        for(int i=0; i<dungeonData.unitList.Count; i++)
        {
            if ((dungeonData.unitList[i].Owner != null) && (dungeonData.unitList[i].team != Team.Player))
                Destroy(dungeonData.unitList[i].Owner.gameObject);
        }
        for(int i=0; i<dungeonData.fieldItemList.Count; i++)
        {
            if (dungeonData.fieldItemList[i].owner != null)
                Destroy(dungeonData.fieldItemList[i].owner.gameObject);
        }
        for (int i = 0; i < dungeonData.dungeonObjectList.Count; i++)
        {
            if (dungeonData.dungeonObjectList[i].Owner != null)
                Destroy(dungeonData.dungeonObjectList[i].Owner.gameObject);
        }
        for(int i=0; i<dungeonData.mapData.Count; i++)
        {
            for(int j=0; j < dungeonData.mapData[0].Count; j++)
            {
                map.GetElementAt(i, j).ResetTile();
                fogMap.GetElementAt(i, j).ResetFog();
            }
        }
    }

    int SortByTurn(Unit a, Unit b)
    {
        if (a.UnitData.TurnIndicator < b.UnitData.TurnIndicator) return -1;
        else if (a.UnitData.TurnIndicator > b.UnitData.TurnIndicator) return 1;
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
