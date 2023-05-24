using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DungeonManager : MonoBehaviour
{
    Dictionary<string, AsyncOperationHandle<GameObject>> monsterHandles = new();
    Dictionary<string, AsyncOperationHandle<EquipmentBase>> equipmentHandles = new();

    public Sprite[] FirstTile;
    public Sprite[] FirstFloor;

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
        player.Init(this, new Coordinate(0, 0), 0);
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
        for(int i=0; i<dungeonData.unitList.Count; i++)
        {
            dungeonData.unitList[i].Owner.UpdateSightArea();
            if ( dungeonData.unitList[i].Owner.GetType() == typeof(Monster))
            {
                (dungeonData.unitList[i].Owner as Monster).LookAround();
            }
        }
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
        for (int i = 0; i < dungeonData.unitList.Count; i++)
        {
            dungeonData.unitList[i].Owner.UpdateSightArea();
            if (dungeonData.unitList[i].Owner.GetType() == typeof(Monster))
            {
                (dungeonData.unitList[i].Owner as Monster).LookAround();
            }
        }
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
        for (int i = 0; i < dungeonData.mapData.Count; i++)
        {
            for (int j = 0; j < dungeonData.mapData[0].Count; j++)
            {
                map.GetElementAt(i,j).Init(this, dungeonData.mapData[i][j], i, j);
                fogMap.GetElementAt(i, j).Init(this, i, j);
            }
        }
        foreach (Coordinate c in dungeonData.observedFog)
        {
            fogMap.GetElementAt(c).LoadObserved();
        }
        map.arrSize.x = dungeonData.mapData.Count;
        map.arrSize.y = dungeonData.mapData[0].Count;
        fogMap.arrSize.x = dungeonData.mapData.Count;
        fogMap.arrSize.y = dungeonData.mapData[0].Count;

        for (int i = 1; i < map.arrSize.x - 1; i++)
        {
            for (int j = 1; j < map.arrSize.y - 1; j++)
            {
                map.x[i].y[j].SetTileSprite();
                fogMap.GetElementAt(i, j).UpdateSprite();
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
    void GenerateUnits()
    {
        SaveData saveData = GameManager.Instance.saveData;
        
        for (int i = 0; i < dungeonData.rooms.Count; i++)
        {
            Room room = dungeonData.rooms[i];
            int amount = 1 + (int)((room.Area / 16) * Random.value);
            for (int j = 0; j < amount; j++)
            {
                Coordinate c = new(Random.Range(room.Left, room.Right), Random.Range(room.Top, room.Bottom));
                List<Coordinate> genPoints = GlobalMethods.RangeByStep(c, 1);
                for (int k = 0; k < genPoints.Count; k++)
                {
                    Tile tile = map.GetElementAt(genPoints[k]);
                    if (tile.IsReachableTile())
                    {
                        int pick = Random.Range(0, 3);
                        GameObject prefab = GetMonsterPrefab(GameManager.Instance.StringData.Monsters[saveData.MonsterLayout[(saveData.currentFloor + pick) % (saveData.MonsterLayout.Count)]]);
                        if (prefab != null)
                        {
                            Monster temp = Instantiate(prefab).GetComponent<Monster>();
                            dungeonData.unitList.Add(temp.UnitData);
                            temp.Init(this, genPoints[k], saveData.currentFloor + pick);
                        }
                        break;
                    }
                }
            }
        }
        
        Stair stairUp = Instantiate(GameManager.Instance.GetPrefab(PrefabAssetType.DungeonObject, "STAIR_UP")).GetComponent<Stair>();
        stairUp.Init("STAIR_UP",this, dungeonData.rooms[dungeonData.stairRooms.Item1].center);
        dungeonData.dungeonObjectList.Add(stairUp.DungeonObjectData);
        Stair stairDown = Instantiate(GameManager.Instance.GetPrefab(PrefabAssetType.DungeonObject, "STAIR_DOWN")).GetComponent<Stair>();
        stairDown.Init("STAIR_UP", this, dungeonData.rooms[dungeonData.stairRooms.Item2].center);
        dungeonData.dungeonObjectList.Add(stairDown.DungeonObjectData);

        DungeonObject treasureBox = Instantiate(GameManager.Instance.GetPrefab(PrefabAssetType.DungeonObject, "TREASURE_BOX_RED")).GetComponent<DungeonObject>();
        List<Coordinate> pickedCoord = GlobalMethods.RangeByStep(dungeonData.rooms[Random.Range(0, dungeonData.rooms.Count)].PickRandomCordinate(), 1);
        for(int i=0; i<pickedCoord.Count; i++)
        {
            if (map.GetElementAt(pickedCoord[i]).IsEmptyTile())
            {
                treasureBox.Init("TREASURE_BOX_RED", this, pickedCoord[i]);
                dungeonData.dungeonObjectList.Add(treasureBox.DungeonObjectData);
                break;
            }
        }

        for (int i = 0; i < 20; i++)
        {
            InstantiateDungeonObject("TOMB", dungeonData.rooms[Random.Range(0, dungeonData.rooms.Count)].PickRandomCordinate());
        }
        InstantiateDungeonObject("TOMB", dungeonData.rooms[Random.Range(0, dungeonData.rooms.Count)].PickRandomCordinate());

        pickedCoord = GlobalMethods.RangeByStep(dungeonData.rooms[Random.Range(0, dungeonData.rooms.Count)].PickRandomCordinate(), 1);
        for (int i = 0; i < pickedCoord.Count; i++)
        {
            if (map.GetElementAt(pickedCoord[i]).IsEmptyTile())
            {
                ItemObject redKey = Instantiate(GameManager.Instance.itemObjectPrefab, pickedCoord[i].ToVector2(), Quaternion.identity);
                redKey.Init(this, pickedCoord[i], new MiscData(GameManager.Instance.GetMiscBase("KEY_BOX_RED"), 1));
                dungeonData.fieldItemList.Add(redKey.Data);
                map.GetElementAt(pickedCoord[i]).items.Push(redKey);
                redKey.Drop();
                break;
            }
        }

        for(int i=0; i<dungeonData.rooms.Count; i++)
        {
            for(int j=0; j<dungeonData.rooms[i].Entrances.Count; j++)
            {
                Coordinate c = dungeonData.rooms[i].Entrances[j];
                if (map.GetElementAt(c.x - 1, c.y).TileData.tileType == TileType.Wall)
                    InstantiateDungeonObject("DOOR_FRONT", c);
                else if (map.GetElementAt(c.x, c.y - 1).TileData.tileType == TileType.Wall)
                    InstantiateDungeonObject("DOOR_SIDE", c);
            }
        }
    }
    void LoadUnits()
    {
        for(int i=0; i<dungeonData.unitList.Count; i++)
        {
            if (dungeonData.unitList[i].team != Team.Player)
            {
                GameObject prefab = GetMonsterPrefab(dungeonData.unitList[i].Key);
                if(prefab != null)
                {
                    Monster temp = Instantiate(prefab).GetComponent<Monster>();
                    temp.SetUnitData(this, dungeonData.unitList[i]);
                }
            }
        }
        for(int i=0; i<dungeonData.dungeonObjectList.Count; i++)
        {
            DungeonObject dunObj = Instantiate(GameManager.Instance.GetPrefab(PrefabAssetType.DungeonObject, dungeonData.dungeonObjectList[i].Key)).GetComponent<DungeonObject>();
            dunObj.Load(this, dungeonData.dungeonObjectList[i]);
            map.GetElementAt(dunObj.DungeonObjectData.coord).dungeonObjects.Add(dunObj);
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
        SaveData saveData = GameManager.Instance.saveData;
        Room room = dungeonData.rooms[Random.Range(0, dungeonData.rooms.Count)];
        Coordinate genPoint = new(Random.Range(room.Left, room.Right), Random.Range(room.Bottom, room.Top));
        List<Coordinate> genPoints = GlobalMethods.RangeByStep(genPoint, 1);
        for (int i = 0; i < genPoints.Count; i++)
        {
            Tile tile = map.GetElementAt(genPoints[i]);
            if (tile.IsReachableTile() && !player.TilesInSight.Contains(tile.Coord))
            {
                int pick = Random.Range(0, 3);
                GameObject prefab = GetMonsterPrefab(GameManager.Instance.StringData.Monsters[saveData.MonsterLayout[(saveData.currentFloor + pick) % (saveData.MonsterLayout.Count)]]);
                if (prefab != null)
                {
                    Monster temp = Instantiate(prefab).GetComponent<Monster>();
                    dungeonData.unitList.Add(temp.UnitData);
                    temp.Init(this, genPoints[i], saveData.currentFloor + pick);
                    if (fogMap.GetElementAt(genPoints[i]).IsOn)
                    {
                        temp.MySpriteRenderer.enabled = false;
                        temp.canvas.enabled = false;
                    }
                    temp.UpdateSightArea();
                    temp.LookAround();
                }
                break;
            }
        }
    }

    public Monster InstantiateMonster(string key, Coordinate location)
    {
        Monster monster = null;
        List<Coordinate> locations = GlobalMethods.RangeByStep(location, 1);
        for(int i=0; i<locations.Count; i++)
        {
            Tile tile = map.GetElementAt(locations[i]);
            if (tile.IsReachableTile())
            {
                GameObject prefab = GameManager.Instance.GetPrefab(PrefabAssetType.Monster, key);
                if(prefab != null)
                {
                    monster = Instantiate(prefab).GetComponent<Monster>();
                    dungeonData.unitList.Add(monster.UnitData);
                    monster.Init(this, locations[i], GameManager.Instance.saveData.currentFloor + 1);
                    if (fogMap.GetElementAt(locations[i]).IsOn)
                    {
                        monster.MySpriteRenderer.enabled = false;
                        monster.canvas.enabled = false;
                    }
                    monster.UpdateSightArea();
                    monster.LookAround();

                }
                break;
            }
        }
        return monster;
    }
    public void InstantiateDungeonObject(string key, Coordinate location)
    {
        List<Coordinate> locations = GlobalMethods.RangeByStep(location, 1);
        for (int i = 0; i < locations.Count; i++)
        {
            Tile tile = map.GetElementAt(locations[i]);
            if (tile.IsEmptyTile())
            {
                GameObject prefab = GameManager.Instance.GetPrefab(PrefabAssetType.DungeonObject, key);
                if (prefab != null)
                {
                    DungeonObject temp = Instantiate(prefab).GetComponent<DungeonObject>();
                    temp.Init(key, this, locations[i]);
                    dungeonData.dungeonObjectList.Add(temp.DungeonObjectData);
                }
                break;
            }
        }
    }
    public void InstantiateItemObject(string key, Coordinate location)
    {
        List<Coordinate> locations = GlobalMethods.RangeByStep(location, 1);
        for (int i = 0; i < locations.Count; i++)
        {
            Tile tile = map.GetElementAt(locations[i]);
            if (tile.IsReachableTile())
            {
                ItemObject temp = Instantiate(GameManager.Instance.itemObjectPrefab, locations[i].ToVector2(), Quaternion.identity);
                temp.Init(this, locations[i], new MiscData(GameManager.Instance.GetMiscBase(key), 1));
                dungeonData.fieldItemList.Add(temp.Data);
                map.GetElementAt(locations[i]).items.Push(temp);
                temp.Drop();
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
            //order = 0;
            //maxOrder = 0;
            StartCoroutine(ManageTurn());
            //dungeonData.unitList[order].Owner.StartTurn();
        }
    }

    public void UpdateUnitRenderers()
    {
        for (int i = 0; i < dungeonData.unitList.Count; i++)
        {
            if (dungeonData.unitList[i].team == Team.Player) continue;

            if (fogMap.GetElementAt(dungeonData.unitList[i].coord.x, dungeonData.unitList[i].coord.y).IsOn)
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

    public void UpdateSightAreaNearThis(Coordinate coord)
    {
        for(int i=0; i<dungeonData.unitList.Count; i++)
        {
            if (dungeonData.unitList[i].Owner.TilesInSight.Contains(coord))
                dungeonData.unitList[i].Owner.UpdateSightArea();
        }
    }

    IEnumerator ManageTurn()
    {
        while (true)
        {
            int finished = 0;
            for(int i=0; i<maxOrder; i++)
            {
                if (dungeonData.unitList[i].Owner.isAnimationFinished)
                    finished++;
            }
            if (finished == maxOrder) break;
            yield return Constants.ZeroPointZeroOne;
        }

        order = 0;
        maxOrder = 0;

        dungeonData.unitList = dungeonData.unitList.OrderBy(x => x.TurnIndicator).ToList();

        decimal min = dungeonData.unitList[0].TurnIndicator;

        int trimAmount = decimal.ToInt32(decimal.Floor(min));
        min -= trimAmount;
        curTurn += trimAmount;
        Debug.Log("Turn: " + curTurn.ToString());
        for (int i = 0; i < dungeonData.unitList.Count; i++)
        {
            dungeonData.unitList[i].Owner.TrimTurn(trimAmount);
            dungeonData.unitList[i].hpRegenCounter += trimAmount;
            dungeonData.unitList[i].mpRegenCounter += trimAmount;
            if (dungeonData.unitList[i].TurnIndicator == min)
                maxOrder++;
        }
        for (int i = 0; i < trimAmount; i++)
        {
            if ((dungeonData.unitList.Count < 50) && (Random.value <= 0.05f)) RegenerateMobs();
        }

        dungeonData.unitList[order].Owner.StartTurn();
    }

    void ManageTurnOld()
    {
        //units.Sort(SortByTurn);
        dungeonData.unitList = dungeonData.unitList.OrderBy(x => x.TurnIndicator).ToList();
 
        decimal min = dungeonData.unitList[0].TurnIndicator;

        int trimAmount = decimal.ToInt32(decimal.Floor(min));
        min -= trimAmount;
        curTurn += trimAmount;
        Debug.Log("Turn: " + curTurn.ToString());
        for (int i = 0; i < dungeonData.unitList.Count; i++)
        {
            dungeonData.unitList[i].Owner.TrimTurn(trimAmount);
            dungeonData.unitList[i].hpRegenCounter += trimAmount;
            dungeonData.unitList[i].mpRegenCounter += trimAmount;
            if (dungeonData.unitList[i].TurnIndicator == min)
                maxOrder++;
        }
        for(int i=0; i<trimAmount; i++)
        {
            if (Random.value <= 0.00f) RegenerateMobs();
        }
    }

    public void RemoveUnit(UnitData unitData)
    {
        int index = dungeonData.unitList.IndexOf(unitData);
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

    GameObject GetMonsterPrefab(string Key)
    {

        GameObject prefab = null;
        if (monsterHandles.ContainsKey(Key))
        {
            prefab = monsterHandles[Key].Result;
        }
        else
        {
            monsterHandles.Add(Key, Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Monster/" + Key + ".prefab"));
            monsterHandles[Key].WaitForCompletion();
            if (monsterHandles[Key].Status == AsyncOperationStatus.Succeeded)
                prefab = monsterHandles[Key].Result;
        }
        return prefab;
    }
    public EquipmentBase GetEquipmentBase(string key)
    {
        EquipmentBase equipment = null;
        if (equipmentHandles.ContainsKey(key))
            equipment = equipmentHandles[key].Result;
        else
        {
            equipmentHandles.Add(key, Addressables.LoadAssetAsync<EquipmentBase>("Assets/Scriptable Objects/Equip/" + key + ".asset"));
            equipmentHandles[key].WaitForCompletion();
            if (equipmentHandles[key].Status == AsyncOperationStatus.Succeeded)
                equipment = equipmentHandles[key].Result;
        }
        return equipment;
    }

    public void TestOut()
    {
        Debug.Log("On");
    }

    private void OnDestroy()
    {
        foreach(KeyValuePair<string,AsyncOperationHandle<GameObject>> pair in monsterHandles)
        {
            Addressables.Release(pair.Value);
        }
        foreach(KeyValuePair<string, AsyncOperationHandle<EquipmentBase>> pair in equipmentHandles)
        {
            Addressables.Release(pair.Value);
        }
    }
}
