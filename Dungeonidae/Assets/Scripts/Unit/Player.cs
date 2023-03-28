using DG.Tweening;
using System;
using UnityEngine;

public class Player : Unit
{
    public PlayerData PlayerData { get; private set; } = new PlayerData();
    public bool IsThrowingMode { get; private set; } = false;
    Tuple<ItemType, int> itemToThrow;
    ItemObject throwingItem;

    AbilityDirector abilityDirector;

    protected override void Awake()
    {
        base.Awake();
        abilityDirector = new AbilityDirector(this);
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void Init(DungeonManager dungeonManager, Coordinate c)
    {
        base.Init(dungeonManager, c);;

        PlayerData.abilityPoint += 9;
        for(int i=0; i<GameManager.Instance.testAbility.Length; i++)
        {
            PlayerData.AddAbility(new AbilityData(GameManager.Instance.testAbility[i]));
        }
    }

    public override void StartTurn()
    {
        base.StartTurn();
    }

    protected override void EndMove()
    {
        base.EndMove();
        dm.UpdateUnitRenderers();
    }

    protected override void UpdateSightArea()
    {
        for (int i = 0; i < tilesInSight.Count; i++)
        {
            dm.FogMap[tilesInSight[i].x, tilesInSight[i].y].Cover();
        }

        base.UpdateSightArea();

        for (int i = 0; i < tilesInSight.Count; i++)
        {
            //map[clearFog[i].x, clearFog[i].y].isObserved = true;
            dm.FogMap[tilesInSight[i].x, tilesInSight[i].y].Clear();
        }
    }

    protected override void FollowPath()
    {
        if (isFollowingPath && foundSomething)
        {
            foundSomething = false;
            isFollowingPath = false;
            path.Clear();
            StartTurn();
            return;
        }
        base.FollowPath();
    }

    public void LootItem()
    {
        Tile tile = dm.GetTileByCoordinate(Coord);
        if (tile.items.Count > 0)
        {   
            if(tile.items.Peek().data is EquipmentData equip) {
                if (PlayerData.AddEquipment(equip))
                {
                    Destroy(tile.items.Peek().gameObject);
                    tile.items.Pop();
                }
            }
            else if(tile.items.Peek().data is MiscData misc)
            {
                int before = misc.Amount;
                if (PlayerData.AddMisc(misc))
                {
                    Destroy(tile.items.Peek().gameObject);
                    tile.items.Pop();
                    EndTurn(1);
                }
                else if (before != misc.Amount)
                {
                    //인벤토리공간이없어서일부만획득
                    EndTurn(1);
                }
                else
                {
                    //인벤토리공간이없음
                }
            }
        }
    }
    //https://stackoverflow.com/questions/983030/type-checking-typeof-gettype-or-is

    public void UseItem(int index)
    {
        if (itemEffectDirector.ItemEffect(PlayerData.miscInventory[index])){
            PlayerData.RemoveOneMisc(index);
            EndTurn(1);
        }
    }

    public void DropEquip(int index)
    {
        ItemObject item = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
        item.Init(dm, new Coordinate((Vector2)transform.position), PlayerData.equipInventory[index]);
        item.Bounce();
        dm.GetTileByCoordinate(item.Coord).items.Push(item);
        PlayerData.equipInventory.RemoveAt(index);
        EndTurn(1);
    }
    public void DropMisc(int index)
    {
        ItemObject item = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
        item.Init(dm, new Coordinate((Vector2)transform.position), PlayerData.miscInventory[index]);
        item.Bounce();
        dm.GetTileByCoordinate(item.Coord).items.Push(item);
        PlayerData.miscInventory.RemoveAt(index);
        EndTurn(1);
    }
    public void PrepareThrowing(ItemType type, int index)
    {
        Controllable = false;
        IsThrowingMode = true;
        itemToThrow = new Tuple<ItemType,int>(type, index);
    }
    void ThrowItem(Coordinate to)
    {
        IsThrowingMode = false;
        throwingItem = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
        if (itemToThrow.Item1 == ItemType.Equipment)
        {
            throwingItem.Init(dm, to, PlayerData.equipInventory[itemToThrow.Item2]);
            PlayerData.equipInventory.RemoveAt(itemToThrow.Item2);
        }
        else if (itemToThrow.Item1 == ItemType.Misc)
        {
            throwingItem.Init(dm, to, new MiscData(PlayerData.miscInventory[itemToThrow.Item2].MiscBase, 1));
            PlayerData.RemoveOneMisc(itemToThrow.Item2);
        }
        dm.GetTileByCoordinate(to).items.Push(throwingItem);
        throwingItem.transform.DOMove(to.ToVector3(0), Coordinate.Distance(to, Coord) * 0.08f).OnComplete(EndThrowing);
    }
    void EndThrowing()
    {
        throwingItem.Drop();
        EndTurn(1);
    }

    public void TileClicked(Coordinate coord)
    {
        if (!Controllable)
        {
            if (isFollowingPath)
            {
                isFollowingPath = false;
                MyAnimator.SetBool("Walk", false);
                path.Clear();
            }
            else if (IsThrowingMode)
            {
                if (tilesInSight.Contains(coord))
                    ThrowItem(coord);
            }
            return;
        }

        if (coord == Coord)
            return;
        else if ((coord.x - Coord.x) > 0)
            FlipSprite(Directions.E);
        else if ((coord.x - Coord.x) < 0)
            FlipSprite(Directions.W);


        if (map[coord.x, coord.y].type == TileType.Floor)
        {
            if (map[coord.x, coord.y].unit == null)
            {
                if (dm.FogMap[coord.x, coord.y].IsObserved)
                {
                    if(FindPath(coord))
                        FollowPath();
                }
            }
            else
            {
                StartBasicAttack(map[coord.x, coord.y].unit);
            }
        }
    }

    public void EquipEquipment(int inventoryIndex, int equippedIndex)
    {
        EquipmentData equip = PlayerData.equipInventory[inventoryIndex];
        PlayerData.equipped[equippedIndex] = equip;
        PlayerData.equipInventory.RemoveAt(inventoryIndex);
        UnitData.ApplyEquipStats(equip);
        EndTurn(3);
    }

    public void ExchangeEquipment(int inventoryIndex, int equippedIndex)
    {
        (PlayerData.equipInventory[inventoryIndex], PlayerData.equipped[equippedIndex]) = (PlayerData.equipped[equippedIndex], PlayerData.equipInventory[inventoryIndex]);
        UnitData.ApplyEquipStats(PlayerData.equipped[equippedIndex]);
        UnitData.RemoveEquipStats(PlayerData.equipInventory[inventoryIndex]);
        EndTurn(3);
    }

    public bool UnequipEquipment(int index)
    {
        if (PlayerData.equipInventory.Count < PlayerData.maxEquip)
        {
            EquipmentData equip = PlayerData.equipped[index];
            PlayerData.equipInventory.Add(equip);
            PlayerData.equipped[index] = null;
            UnitData.RemoveEquipStats(equip);
            EndTurn(3);
            return true;
        }
        else return false;
    }

    public bool IncreaseAbilityLevel(int index)
    {
        if ((PlayerData.Abilities[index].Level < 3) && (PlayerData.abilityPoint >= 1))
        {
            PlayerData.abilityPoint--;
            abilityDirector.RemoveAbility(PlayerData.Abilities[index]);
            PlayerData.Abilities[index].IncreaseLevel();
            abilityDirector.ApplyAbility(PlayerData.Abilities[index]);
            return true;
        }
        else return false;
    }
}
