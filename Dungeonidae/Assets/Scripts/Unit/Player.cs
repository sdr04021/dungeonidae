using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class Player : Unit
{
    public bool IsThrowingMode { get; private set; } = false;
    Tuple<ItemType, ItemSlotType, int> itemToThrow;
    ItemObject throwingItem;

    public bool IsSkillMode { get; private set; } = false;
    public SkillBase CurrentSkill { get; private set; }
    public int CurrentSkillIndex { get; private set; } = -1;

    public Coordinate Targeting { get; private set; }

    public delegate void EventHandler();
    public event EventHandler MoveCamera;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void Init(DungeonManager dungeonManager, Coordinate c, int level)
    {
        base.Init(dungeonManager, c, level);

        UnitData.abilityPoint += 9;
        for (int i = 0; i < GameManager.Instance.testAbility.Length; i++)
        {
            UnitData.AddAbility(new AbilityData(GameManager.Instance.testAbility[i]));
        }
        //UnitData.AddSkill(new SkillData(GameManager.Instance.testSkill[0]), 0);
        //UnitData.AddSkill(new SkillData(GameManager.Instance.testSkill[1]), 3);
        //UnitData.AddSkill(new SkillData(GameManager.Instance.testSkill[2]), 4);
        UnitData.acquiredSkills.Add("POWER_STRIKE");
        UnitData.acquiredSkills.Add("RUSH");
        UnitData.acquiredSkills.Add("SPRINT");
        UnitData.acquiredSkills.Add("CRITICAL_BLOW");
        //UnitData.skillDeck[0] = "POWER_STRIKE";s
        UnitData.equipped[1] = new EquipmentData(GameManager.Instance.GetEquipmentBase("WOODEN_SWORD"));
        UnitData.equipped[3] = new EquipmentData(GameManager.Instance.GetEquipmentBase("WORN_WOODEN_SHIELD"));
        UnitData.ApplyEquipStats(UnitData.equipped[1]);
        UnitData.ApplyEquipStats(UnitData.equipped[3]);
        UnitData.AddMisc(new MiscData(GameManager.Instance.GetMiscBase("POTION_HASTE"), 5));
    }

    public void Step(Directions directions)
    {
        if (!Move(directions)) DecideBehavior();
        else MoveCamera.Invoke();
    }

    public override void Teleportation()
    {
        base.Teleportation();
        MoveCamera.Invoke();
    }

    public override bool Move(Directions direction)
    {
        bool result = base.Move(direction);
        MoveCamera.Invoke();
        return result;
    }

    protected override void EndMove()
    {
        base.EndMove();
        dm.UpdateUnitRenderers();
    }

    public override void UpdateSightArea()
    {
        for (int i = 0; i < TilesInSight.Count; i++)
        {
            dm.fogMap.GetElementAt(TilesInSight[i].x, TilesInSight[i].y).Cover();
        }

        base.UpdateSightArea();

        for (int i = 0; i < TilesInSight.Count; i++)
        {
            dm.fogMap.GetElementAt(TilesInSight[i].x, TilesInSight[i].y).Clear();
        }

        /*
        for(int i=0; i < dm.fogMap.arrSize.x; i++)
        {
            for (int j = 0; j < dm.fogMap.arrSize.y; j++)
                dm.fogMap.GetElementAt(i, j).UpdateSprite();
        }
        */
    }

    protected override void FollowPath()
    {
        if (isFollowingPath && FoundSomething)
        {
            FoundSomething = false;
            isFollowingPath = false;
            MyAnimator.SetBool("Walk", false);
            path.Clear();
            DecideBehavior();
            return;
        }
        base.FollowPath();
    }

    public void Interact()
    {
        if (dm.map.GetElementAt(UnitData.coord).items.Count > 0)
            LootItem();
        else if ((dm.map.GetElementAt(UnitData.coord).dungeonObjects.Count > 0) && (dm.map.GetElementAt(UnitData.coord).dungeonObjects[^1].IsInteractable))
        {
            dm.map.GetElementAt(UnitData.coord).dungeonObjects[^1].Activate(this);
        }
    }
    public void LootItem()
    {
        Tile tile = dm.GetTileByCoordinate(UnitData.coord);
        if (tile.items.Count > 0)
        {
            ItemObject itemObj = tile.items.Peek();
            if (itemObj.Data is EquipmentData equip)
            {
                if (UnitData.AddEquipment(equip))
                {
                    tile.items.Pop();
                    GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Remove(itemObj.Data);
                    itemObj.Loot();
                    EndTurn(1);
                }
                else
                {
                    //인벤토리공간이없음
                }
            }
            else if (itemObj.Data is MiscData misc)
            {
                int before = misc.Amount;
                if (UnitData.AddMisc(misc))
                {
                    tile.items.Pop();
                    GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Remove(itemObj.Data);
                    itemObj.Loot(); 
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
        if (itemEffectDirector.ItemEffect(UnitData.miscInventory[index]))
        {
            UnitData.RemoveOneMisc(index);
            EndTurn(1);
        }
    }

    public void DropEquip(int index, ItemSlotType itemSlotType)
    {
        ItemObject item = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
        if (itemSlotType == ItemSlotType.Item)
            item.Init(dm, new Coordinate((Vector2)transform.position), UnitData.equipInventory[index]);
        else if(itemSlotType == ItemSlotType.Equipped)
            item.Init(dm, new Coordinate((Vector2)transform.position), UnitData.equipped[index]);
        GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(item.Data);
        item.Bounce();
        dm.GetTileByCoordinate(item.Coord).items.Push(item);
        if (itemSlotType == ItemSlotType.Item)
            UnitData.equipInventory.RemoveAt(index);
        else if (itemSlotType == ItemSlotType.Equipped)
        {
            UnitData.RemoveEquipStats(UnitData.equipped[index]);
            UnitData.equipped[index] = null;
        }
        EndTurn(1);
    }
    public void DropMisc(int index)
    {
        ItemObject item = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
        item.Init(dm, new Coordinate((Vector2)transform.position), UnitData.miscInventory[index]);
        GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(item.Data);
        item.Bounce();
        dm.GetTileByCoordinate(item.Coord).items.Push(item);
        UnitData.miscInventory.RemoveAt(index);
        EndTurn(1);
    }

    public void PrepareThrowing(ItemType type, ItemSlotType itemSlotType, int index)
    {
        Controllable = false;
        IsThrowingMode = true;
        itemToThrow = new Tuple<ItemType, ItemSlotType, int>(type, itemSlotType, index);

        for(int i=0; i< TilesInSight.Count; i++)
        {
            Tile tile = dm.GetTileByCoordinate(TilesInSight[i]);
            if ((tile.Coord != UnitData.coord) && (tile.TileData.tileType == TileType.Floor))
            {
                AvailableRange.Add(tile.Coord);
                tile.SetAvailable();
            }
        }
    }
    void ThrowItem(Coordinate to)
    {
        IsThrowingMode = false;
        throwingItem = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
        if (itemToThrow.Item1 == ItemType.Equipment)
        {
            if(itemToThrow.Item2 == ItemSlotType.Item)
            {
                throwingItem.Init(dm, to, UnitData.equipInventory[itemToThrow.Item3]);
                GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(throwingItem.Data);
                UnitData.equipInventory.RemoveAt(itemToThrow.Item3);
            }
            else if (itemToThrow.Item2 == ItemSlotType.Equipped)
            {
                throwingItem.Init(dm, to, UnitData.equipped[itemToThrow.Item3]);
                GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(throwingItem.Data);
                UnitData.RemoveEquipStats(UnitData.equipped[itemToThrow.Item3]);
                UnitData.equipped[itemToThrow.Item3] = null;
            }

        }
        else if (itemToThrow.Item1 == ItemType.Misc)
        {
            throwingItem.Init(dm, to, new MiscData(GameManager.Instance.GetMiscBase(UnitData.miscInventory[itemToThrow.Item3].Key), 1));
            GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(throwingItem.Data);
            UnitData.RemoveOneMisc(itemToThrow.Item3);
        }
        dm.GetTileByCoordinate(to).items.Push(throwingItem);
        throwingItem.transform.DOMove(to.ToVector3(0), Coordinate.Distance(to, UnitData.coord) * 0.08f).OnComplete(EndThrowing);
    }
    void EndThrowing()
    {
        throwingItem.Drop();
        Tile tile = dm.map.GetElementAt(throwingItem.Coord);
        for(int i=0; i<tile.dungeonObjects.Count; i++)
        {
            if (tile.dungeonObjects[i].IsActivatesByThrownItem)
            {
                tile.dungeonObjects[i].Activate(this);
                break;
            }
        }
        if (throwingItem.Data.GetType() == typeof(MiscData))
        {
            if (tile.unit != null && itemEffectDirector.ThrownEffect(tile.unit, (MiscData)throwingItem.Data))
            {
                dm.GetTileByCoordinate(throwingItem.Data.coord).items.Pop();
                GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Remove(throwingItem.Data);
                Destroy(throwingItem.gameObject);
            }
        }
        EndTurn(1);
    }
    protected override void ResetSkillRange()
    {
        for (int i = 0; i < AvailableRange.Count; i++)
        {
            dm.GetTileByCoordinate(AvailableRange[i]).TurnOffRangeIndicator();
        }
        for (int i = 0; i < UnavailableRange.Count; i++)
        {
            dm.GetTileByCoordinate(UnavailableRange[i]).TurnOffRangeIndicator();
        }
        dm.targetMark.SetActive(false);
        base.ResetSkillRange();
    }
    public void CancelThrow()
    {
        ResetSkillRange();
        IsThrowingMode = false;
        Controllable = true;
        itemToThrow= null;
        throwingItem = null;
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
                if (AvailableRange.Contains(coord))
                {
                    FlipSprite(coord);
                    ThrowItem(coord);
                    ResetSkillRange();
                }
                else CancelThrow();
            }
            else if (IsSkillMode)
            {
                if (AvailableRange.Contains(coord))
                {
                    UseCurrentSkill(coord);
                    ResetSkillRange();
                }
                else CancelSkill();
            }
            return;
        }
        if (coord == UnitData.coord)
        {
            Interact();
            return;
        }

        if (dm.fogMap.GetElementAt(coord).IsObserved)
        {
            if (dm.map.GetElementAt(coord).TileData.tileType == TileType.Floor)
            {
                bool avoidTrap = true;
                if (Coordinate.InRange(coord, UnitData.coord, 1.5f)) avoidTrap = false;
                if (dm.fogMap.GetElementAt(coord).IsOn)
                {
                    if (FindPath(coord, avoidTrap)) FollowPath();
                }
                else
                {
                    if (dm.map.GetElementAt(coord).unit != null)
                    {
                        BasicAttack.SetRange(this, dm, false);
                        if (AvailableRange.Contains(coord))
                        {
                            StartCoroutine(BasicAttack.Skill(this, dm, coord));
                            Controllable = false;
                        }
                        else ResetSkillRange();
                    }
                    else if (UnitData.coord.IsTargetInRange(coord, 1) && !dm.map.GetElementAt(coord).IsReachableTile(false) && (dm.map.GetElementAt(coord).GetTargetable() != null))
                    {
                        StartCoroutine(BasicAttack.Skill(this, dm, coord));
                        Controllable = false;
                    }
                    else
                    {
                        if (FindPath(coord, avoidTrap)) FollowPath();
                    }
                }
            }
        }
        else
        {
            List<Coordinate> surroundings = GlobalMethods.RangeByStep(coord, 1);
            for (int i = 0; i < surroundings.Count; i++)
            {
                if (dm.fogMap.GetElementAt(surroundings[i]).IsObserved)
                {
                    if (FindPath(coord, true)) FollowPath();
                    break;
                }
            }
        }
    }

    public void TileTargeted(Coordinate coord)
    {
        if (IsThrowingMode || IsSkillMode)
        {
            for (int i = 0; i < AvailableRange.Count; i++)
            {
                if (AvailableRange[i] == coord)
                {
                    Targeting = coord;
                    dm.targetMark.transform.position = Targeting.ToVector2();
                }
            }
        }
    }

    public void MoveTargeting(Directions direction)
    {
        if (AvailableRange.Contains(Targeting + new Coordinate(direction)))
        {
            Targeting += new Coordinate(direction);
            dm.targetMark.transform.position = Targeting.ToVector2();
        }
        else
        {
            Coordinate moved = Targeting + new Coordinate(direction);
            Coordinate next = Targeting;
            float dist = Mathf.Infinity;
            for(int i=0; i<AvailableRange.Count; i++)
            {
                if (AvailableRange[i] == Targeting) continue;
                float a = Coordinate.Distance(AvailableRange[i], moved);
                if (a < dist)
                {
                    dist = a;
                    next = AvailableRange[i];
                }
            }
            Targeting = next;
            dm.targetMark.transform.position = Targeting.ToVector2();
        }
    }

    public void StartSkill(int index)
    {
        if (UnitData.currentSkills[index] == null) return;

        CurrentSkill = GameManager.Instance.GetSkillBase(UnitData.currentSkills[index]);
        if (CurrentSkill.IsUseable(this))
        {
            CurrentSkillIndex = index;
            IsSkillMode = true;
            Controllable = false;
            if (CurrentSkill.NeedTarget)
            {
                CurrentSkill.SetRange(this, dm, true);
                if (AvailableRange.Count > 0)
                {
                    if (!AvailableRange.Contains(Targeting)) Targeting = AvailableRange[0];

                    if (AvailableRange.Count == 1)
                    {
                        ResetSkillRange();
                        UseCurrentSkill(Targeting);
                    }
                    else if (AvailableRange.Count > 1)
                    {
                        dm.targetMark.SetActive(true);
                        dm.targetMark.transform.position = Targeting.ToVector2();
                    }
                }
            }
            else UseCurrentSkill();
        }
        else CurrentSkill = null;
    }
    public void UseCurrentSkill(Coordinate coord)
    {
        if (CurrentSkillIndex >= 0 && CurrentSkillIndex <= 5)
        {
            UnitData.currentSkills[CurrentSkillIndex] = null;
            UnitData.skillRechargeLeft[CurrentSkillIndex] = 21;
        }
        IsSkillMode = false;
        StartCoroutine(CurrentSkill.Skill(this, dm, coord));
    }
    public void UseCurrentSkill()
    {
        if (CurrentSkillIndex >= 0 && CurrentSkillIndex <= 5)
        {
            UnitData.currentSkills[CurrentSkillIndex] = null;
            UnitData.skillRechargeLeft[CurrentSkillIndex] = 11;
        }
        IsSkillMode = false;
        StartCoroutine(CurrentSkill.Skill(this, dm, UnitData.coord));
    }
    public void StartBasicAttack()
    {
        IsSkillMode = true;
        Controllable = false;
        BasicAttack.SetRange(this, dm, true);
        CurrentSkill = BasicAttack;
        CurrentSkillIndex = -1;
        if (AvailableRange.Count > 0)
        {
            if(!AvailableRange.Contains(Targeting)) Targeting = AvailableRange[0];
            if (AvailableRange.Count == 1)
            {
                ResetSkillRange();
                UseCurrentSkill(Targeting);
            }
            else
            {
                dm.targetMark.SetActive(true);
                dm.targetMark.transform.position = Targeting.ToVector2();
            }
        }
    }
    public override void EndSkill(decimal turnSpent)
    {
        base.EndSkill(turnSpent);
        ResetSkillRange();
        CurrentSkill = null;
    }
    public void CancelSkill()
    {
        ResetSkillRange();
        IsSkillMode = false;
        Controllable = true;
        CurrentSkill = null;
    }
    public void SkillOnCurrentTargeting()
    {
        UseCurrentSkill(Targeting);
    }

    public void EquipEquipment(int inventoryIndex, int equippedIndex)
    {
        EquipmentData equip = UnitData.equipInventory[inventoryIndex];
        UnitData.equipped[equippedIndex] = equip;
        UnitData.equipInventory.RemoveAt(inventoryIndex);
        UnitData.ApplyEquipStats(equip);
        EndTurn(3);
    }

    public void ExchangeEquipment(int inventoryIndex, int equippedIndex)
    {
        (UnitData.equipInventory[inventoryIndex], UnitData.equipped[equippedIndex]) = (UnitData.equipped[equippedIndex], UnitData.equipInventory[inventoryIndex]);
        UnitData.RemoveEquipStats(UnitData.equipInventory[inventoryIndex]);
        UnitData.ApplyEquipStats(UnitData.equipped[equippedIndex]);
        EndTurn(3);
    }

    public bool UnequipEquipment(int index)
    {
        if (UnitData.equipInventory.Count < UnitData.maxEquip)
        {
            EquipmentData equip = UnitData.equipped[index];
            UnitData.equipInventory.Add(equip);
            UnitData.equipped[index] = null;
            UnitData.RemoveEquipStats(equip);
            EndTurn(3);
            return true;
        }
        else return false;
    }

    public bool IncreaseAbilityLevel(int index)
    {
        List<AbilityData> abilities = UnitData.abilities.Values.ToList();
        if ((abilities[index].Level < 3) && (UnitData.abilityPoint >= 1))
        {
            UnitData.abilityPoint--;
            abilityDirector.RemoveAbility(abilities[index]);
            abilities[index].IncreaseLevel();
            abilityDirector.ApplyAbility(abilities[index]);
            return true;
        }
        else return false;
    }

}
