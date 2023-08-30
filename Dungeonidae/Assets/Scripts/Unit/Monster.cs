using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Monster : Unit
{
    [field:SerializeField] public bool IsAggressive { get; private set; }
    [field:SerializeField] public bool IsForgetting { get; private set; }
    [field:SerializeField] public bool IsRunAway { get; private set; }
    [field: SerializeField] public List<string> MustDropItem { get; private set; } = new();
    [field: SerializeField] public List<EquipmentType> LikelyDropItem { get; private set; } = new();
    [field: SerializeField] public List<SkillBase> Skills { get; private set; } = new();

    int forgetCounter = 0;

    protected override void DecideBehavior()
    {
        base.DecideBehavior();

        if (forgetCounter == 10)
        {
            UnitData.chaseTarget = null;
            forgetCounter = 0;
        }

        if (UnitData.chaseTarget == null || UnitData.chaseTarget.Owner.IsDead)
        {
            UnitData.chaseTarget = null;
            LookAround();
        }

        if (UnitData.chaseTarget == null)
            RandomStep();
        else
        {
            if (UnitsInSight.Contains(UnitData.chaseTarget.Owner))
            {
                UnitData.chaseTargetRecentCoord = UnitData.chaseTarget.coord;

                SkillBase skill = BasicAttack;
                for(int i=0; i<Skills.Count; i++)
                {
                    if (Skills[i].IsUseable(this))
                    {
                        skill = Skills[i];
                        break;
                    }
                }
                skill.SetRange(this, dm, false);
                if (AvailableRange.Contains(UnitData.chaseTarget.coord))
                {
                    StartCoroutine(skill.Skill(this, dm, UnitData.chaseTarget.coord));
                    ResetSkillRange();
                }
                else
                {
                    Directions dir = FollowTarget(UnitData.chaseTarget.coord);
                    if (dir == Directions.NONE)
                        RandomStep();
                    else Move(dir);
                }
            }
            else
            {
                if ((UnitData.chaseTarget.Owner == null) || (UnitData.chaseTarget.Owner.IsDead))
                {
                    UnitData.chaseTarget = null;
                    DecideBehavior();
                }
                else
                {
                    if (UnitData.coord == UnitData.chaseTargetRecentCoord)
                    {
                        ShowBubble("?");
                        UnitData.chaseTarget = null;
                        DecideBehavior();
                    }
                    else if (!FindPath(UnitData.chaseTargetRecentCoord, true))
                    {
                        //RandomStep();
                        forgetCounter++;
                        EndTurn(1);
                    }
                    else
                    {
                        Move(path.Pop());
                    }

                }
            }
        }
        //controllable = false;
        //dungeonManager.EndTurn();
    }

    /*
    public override void UpdateSightArea()
    {
        bool needToCheckSightArea = false;

        int northBound = Mathf.Min(UnitData.coord.y + UnitData.sight.Total(), dm.map.arrSize.x - 1);
        int southBound = Mathf.Max(UnitData.coord.y - UnitData.sight.Total(), 0);
        int eastBound = Mathf.Min(UnitData.coord.x + UnitData.sight.Total(), dm.map.arrSize.y - 1);
        int westBound = Mathf.Max(UnitData.coord.x - UnitData.sight.Total(), 0);

        for (int i = westBound; i <= eastBound; i++)
        {
            for (int j = southBound; j <= northBound; j++)
            {
                if ((dm.map.GetElementAt(i, j).unit!=null)&&(IsHostileUnit(dm.map.GetElementAt(i, j).unit)))
                {
                    needToCheckSightArea = true;
                    break;
                }
            }
        }

        if (!needToCheckSightArea) return;

        base.UpdateSightArea();
    }
    */

    protected override void EndMove()
    {
        CheckSightArea();
        dm.map.GetElementAt(UnitData.coord.x, UnitData.coord.y).unit = this;
        if (UnitData.chaseTarget == null)
            LookAround();
        else if(UnitsInSight.Contains(UnitData.chaseTarget.Owner))
            UnitData.chaseTargetRecentCoord = UnitData.chaseTarget.coord;

        EndTurn(100m / UnitData.speed.Total());
    }

    public void LookAround()
    {
        for (int i=0; i<UnitsInSight.Count; i++)
        {
            if (IsAggressive && IsHostileUnit(UnitsInSight[i]) && !UnitData.hostileTargets.Contains(UnitsInSight[i].UnitData))
            {
                UnitData.hostileTargets.Add(UnitsInSight[i].UnitData);
            }
            if (UnitData.hostileTargets.Contains(UnitsInSight[i].UnitData))
            {
                UnitData.chaseTarget = UnitsInSight[i].UnitData;
                UnitData.isChasingTarget = true;
                FlipSprite(UnitData.chaseTarget.coord);
                ShowBubble("!");
                return;
            }
        } 
    }


    public override void CheckSightArea()
    {
        int sight = UnitData.sight.Total();

        List<Coordinate> targets = new();
        DungeonData dungeonData = GameManager.Instance.saveData.GetCurrentDungeonData();
        for(int i=0; i<dungeonData.unitList.Count; i++)
        {
            if (IsHostileUnit(dungeonData.unitList[i].Owner) && Coordinate.InRange(dungeonData.unitList[i].coord, UnitData.coord, sight + 0.5f))
            {
                targets.Add(dungeonData.unitList[i].coord);
            }
        }

        if (targets.Count > 0)
        {
            int wallMapSize = sight * 2 + 1;
            NativeArray<bool> wallMap = new(wallMapSize * wallMapSize, Allocator.TempJob);
            Coordinate origin = new(UnitData.coord.x - sight, UnitData.coord.y - sight);
            for (int i = 0; i < wallMapSize; i++)
            {
                for (int j = 0; j < wallMapSize; j++)
                {
                    if (dm.IsValidIndexForMap(origin.x + i, origin.y + j) && dm.map.GetElementAt(origin.x + i, origin.y + j).IsBlockingSight())
                    {
                        wallMap[j + i * wallMapSize] = true;
                    }
                    else wallMap[j + i * wallMapSize] = false;
                }
            }
            NativeArray<Coordinate> end = new(targets.Count, Allocator.TempJob);
            NativeArray<bool> result = new(targets.Count, Allocator.TempJob);
            for (int i = 0; i < targets.Count; i++)
            {
                end[i] = targets[i] - origin;
            }

            SightCheckJob2 sightJob = new()
            {
                wallMap = wallMap,
                wallMapSize = wallMapSize,
                start = UnitData.coord - origin,
                end = end,
                result = result
            };

            JobHandle handle = sightJob.Schedule(targets.Count, 1);
            handle.Complete();

            for (int i = 0; i < result.Length; i++)
            {
                if (result[i])
                {
                    UnitsInSight.Add(dm.map.GetElementAt(end[i] + origin).unit);
                }
            }

            wallMap.Dispose();
            end.Dispose();
            result.Dispose();
        }
    }

    public override void GetDamage(AttackData attackData)
    {
        base.GetDamage(attackData);

        UnitData temp = UnitData.chaseTarget;
        CheckSightArea();
        if (attackData.Attacker!=null && UnitsInSight.Contains(attackData.Attacker))
        {
            if (!UnitData.hostileTargets.Contains(attackData.Attacker.UnitData)) UnitData.hostileTargets.Add(attackData.Attacker.UnitData);
            UnitData.chaseTarget = attackData.Attacker.UnitData;
            UnitData.isChasingTarget = true;
            if (temp != UnitData.chaseTarget) ShowBubble("!");
        }
    }

    protected override void StartDie()
    {
        base.StartDie();

        //ItemObject item = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
        //item.Init(dm, new Coordinate((Vector2)transform.position), new Item(GameManager.Instance.testItem));
        //dm.GetTileByCoordinate(item.Coord).items.Push(item);

        dm.Player.IncreaseExp((int)((UnitData.level + 1) * UnitBase.ExpCoefficient));

        /*
        if (Random.Range(0, 9) == 0)
        {
            List<string> list = GameManager.Instance.StringData.EquipTier[0].forClass[0].list;
            string tempKey = list[Random.Range(0, list.Count)];
            EquipmentBase tempBase = GameManager.Instance.GetEquipmentBase(tempKey);
            if (tempBase != null)
            {
                ItemObject itemTemp = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
                itemTemp.Init(dm, new Coordinate((Vector2)transform.position), new EquipmentData(tempBase));
                GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(itemTemp.Data);
                itemTemp.Bounce();
                dm.GetTileByCoordinate(itemTemp.Coord).items.Push(itemTemp);
            }
        }
        */
        /*
        if (Random.Range(0, 2) == 0)
        {
            int pick = Random.Range(0, GameManager.Instance.StringData.MiscItems.Count);
            ItemObject item = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
            item.Init(dm, new Coordinate((Vector2)transform.position), new MiscData(GameManager.Instance.GetMiscBase(GameManager.Instance.StringData.MiscItems[pick]), 1));
            GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(item.Data);
            item.Bounce();
            dm.GetTileByCoordinate(item.Coord).items.Push(item);
        }
        */

        if(Random.Range(0,5)==0)
            dm.InstantiateDungeonObject("HEART_OF_HEALING", UnitData.coord);
        for (int i=0; i<MustDropItem.Count; i++)
        {
            ItemObject itemTemp = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
            itemTemp.Init(dm, new Coordinate((Vector2)transform.position), new MiscData(MustDropItem[i], 1));
            GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(itemTemp.Data);
            itemTemp.Bounce();
            dm.GetTileByCoordinate(itemTemp.Coord).items.Push(itemTemp);
        }
        /*
        ItemObject testItem = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
        testItem.Init(dm, new Coordinate((Vector2)transform.position), new MiscData(GameManager.Instance.GetMiscBase("SCROLL_EQUIPMENT_ENCHANT"), 99));
        GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(testItem.Data);
        testItem.Bounce();
        dm.GetTileByCoordinate(testItem.Coord).items.Push(testItem);
        */
    }
}
