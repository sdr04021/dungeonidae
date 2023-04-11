using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : Unit
{
    public bool IsThrowingMode { get; private set; } = false;
    Tuple<ItemType, ItemSlotType, int> itemToThrow;
    ItemObject throwingItem;
    public List<Coordinate> throwableRange = new();

    public bool IsSkillMode { get; private set; } = false;
    public bool IsBasicAttackMode { get; private set; } = false; 

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
        base.Init(dungeonManager, c);

        UnitData.abilityPoint += 9;
        for (int i = 0; i < GameManager.Instance.testAbility.Length; i++)
        {
            UnitData.AddAbility(new AbilityData(GameManager.Instance.testAbility[i]));
        }
        UnitData.AddSkill(new SkillData(GameManager.Instance.testSkill[0]), 0);
        UnitData.AddSkill(new SkillData(GameManager.Instance.testSkill[1]), 3);
        UnitData.AddSkill(new SkillData(GameManager.Instance.testSkill[2]), 4);
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
            dm.fogMap.GetElementAt(tilesInSight[i].x, tilesInSight[i].y).Cover();
        }

        base.UpdateSightArea();

        for (int i = 0; i < tilesInSight.Count; i++)
        {
            //map[clearFog[i].x, clearFog[i].y].isObserved = true;
            dm.fogMap.GetElementAt(tilesInSight[i].x, tilesInSight[i].y).Clear();
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
        Tile tile = dm.GetTileByCoordinate(UnitData.coord);
        if (tile.items.Count > 0)
        {
            ItemObject itemObj = tile.items.Peek();
            if (itemObj.data is EquipmentData equip)
            {
                if (UnitData.AddEquipment(equip))
                {
                    tile.items.Pop();
                    GameManager.Instance.saveData.GetCurrentDungeonData().RemoveFieldItem(itemObj.data);
                    itemObj.Loot();
                    EndTurn(1);
                }
                else
                {
                    //인벤토리공간이없음
                }
            }
            else if (itemObj.data is MiscData misc)
            {
                int before = misc.Amount;
                if (UnitData.AddMisc(misc))
                {
                    tile.items.Pop();
                    GameManager.Instance.saveData.GetCurrentDungeonData().RemoveFieldItem(itemObj.data);
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
        GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(new ItemDataContainer(item.data));
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
        GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(new ItemDataContainer(item.data));
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

        for(int i=0; i<tilesInSight.Count; i++)
        {
            Tile tile = dm.GetTileByCoordinate(tilesInSight[i]);
            if ((tile.Coord != UnitData.coord) && (tile.TileData.tileType == TileType.Floor))
            {
                throwableRange.Add(tile.Coord);
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
                GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(new ItemDataContainer(throwingItem.data));
                UnitData.equipInventory.RemoveAt(itemToThrow.Item3);
            }
            else if (itemToThrow.Item2 == ItemSlotType.Equipped)
            {
                throwingItem.Init(dm, to, UnitData.equipped[itemToThrow.Item3]);
                GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(new ItemDataContainer(throwingItem.data));
                UnitData.RemoveEquipStats(UnitData.equipped[itemToThrow.Item3]);
                UnitData.equipped[itemToThrow.Item3] = null;
            }

        }
        else if (itemToThrow.Item1 == ItemType.Misc)
        {
            throwingItem.Init(dm, to, new MiscData(GameManager.Instance.testItem, 1));
            GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(new ItemDataContainer(throwingItem.data));
            UnitData.RemoveOneMisc(itemToThrow.Item3);
        }
        dm.GetTileByCoordinate(to).items.Push(throwingItem);
        throwingItem.transform.DOMove(to.ToVector3(0), Coordinate.Distance(to, UnitData.coord) * 0.08f).OnComplete(EndThrowing);
    }
    void EndThrowing()
    {
        throwingItem.Drop();
        EndTurn(1);
    }
    void ResetThrowableRange()
    {
        for (int i = 0; i < throwableRange.Count; i++)
        {
            dm.GetTileByCoordinate(throwableRange[i]).TurnOffRangeIndicator();
        }
        throwableRange.Clear();
    }
    public void CancelThrow()
    {
        ResetThrowableRange();
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
                if (throwableRange.Contains(coord))
                {
                    FlipSprite(coord);
                    ThrowItem(coord);
                    ResetThrowableRange();
                }
                else CancelThrow();
            }
            else if (IsSkillMode)
            {
                if (skill.AvailableTilesInRange.Contains(coord))
                {
                    IsSkillMode = false;
                    FlipSprite(coord);
                    skill.StartSkill(coord);
                }
                else CancelSkill();
            }
            else if (IsBasicAttackMode)
            {
                if (BasicAttack.AvailableTilesInRange.Contains(coord))
                {
                    IsBasicAttackMode = false;
                    FlipSprite(coord);
                    BasicAttack.StartSkill(coord);
                }
                else CancelBasicAttack();
            }
            return;
        }

        if (coord == UnitData.coord)
        {
            if (dm.map.GetElementAt(coord).items.Count > 0)
                LootItem();
            else return;
        }

        FlipSprite(coord);

        if (dm.map.GetElementAt(coord.x, coord.y).TileData.tileType == TileType.Floor)
        {
            if (dm.map.GetElementAt(coord.x, coord.y).unit == null)
            {
                if (dm.fogMap.GetElementAt(coord.x, coord.y).FogData.IsObserved)
                {
                    if (FindPath(coord))
                        FollowPath();
                }
            }
            else
            {
                //StartBasicAttack(map[coord.x, coord.y].unit);
                BasicAttack.SetRange(false);
                if (BasicAttack.AvailableTilesInRange.Contains(coord))
                {
                    Controllable = false;
                    BasicAttack.StartSkill(coord);
                }
                else BasicAttack.ResetTilesInRange();
            }
        }
    }

    public void TileTargeted(Coordinate coord)
    {
        if (IsThrowingMode)
        {
            for (int i = 0; i < throwableRange.Count; i++)
            {
                if (throwableRange[i] == coord)
                {
                    dm.GetTileByCoordinate(coord).targetMark.SetActive(true);
                }
                else dm.GetTileByCoordinate(throwableRange[i]).targetMark.SetActive(false);
            }
        }
        else if (IsSkillMode)
        {
           skill.ShowTargetArea(coord);
        }
        else if(IsBasicAttackMode)
        {
            BasicAttack.ShowTargetArea(coord);
        }

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

    public void PrepareSkill(int index)
    {
        if ((UnitData.Skills[index] != null) && (skill == null))
        {
            MatchSkillData(UnitData.Skills[index]);
            if (skill.IsUsable())
            {
                skill.Prepare();
                Controllable = false;
                if (skill.SkillData.NeedTarget)
                    IsSkillMode = true;
            }
            else
            {
                skill = null;
            }
        }
    }

    public void CancelSkill()
    {
        skill.ResetTilesInRange();
        IsSkillMode = false;
        Controllable = true;
        skill = null;
    }
    public void AutoSkill()
    {
        if(skill.AvailableTilesInRange.Count > 0)
        {
            Coordinate? c = skill.SelectTargetAutomatically();
            if(c!= null)
            {
                IsSkillMode = false;
                FlipSprite((Coordinate)c);
                skill.StartSkill((Coordinate)c);
            }
        }
    }

    public void PrepareBasicAttack()
    {
        if (BasicAttack.IsUsable())
        {
            BasicAttack.Prepare();
            Controllable = false;
            IsBasicAttackMode = true;
        }
    }
    public void CancelBasicAttack()
    {
        BasicAttack.ResetTilesInRange();
        IsBasicAttackMode = false;
        Controllable = true;
    }
    public void AutoBasicAttack()
    {
        Coordinate? c = BasicAttack.SelectTargetAutomatically();
        IsBasicAttackMode = false;
        FlipSprite((Coordinate)c);
        BasicAttack.StartSkill((Coordinate)c);
    }

    public void MatchSkillData(SkillData skillData)
    {
        skill = skillData.Key switch
        {
            "POWER_STRIKE" => new SkillPowerStrike(this, dm, skillData),
            "RUSH" => new Skill_Rush(this, dm, skillData),
            "SPRINT" => new Skill_Buff(this, dm, skillData),
            _ => null,
        };
    }
}
