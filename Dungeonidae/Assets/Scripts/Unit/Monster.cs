using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Monster : Unit
{
    [field:SerializeField] public bool IsAggressive { get; private set; }
    [field:SerializeField] public bool IsForgetting { get; private set; }
    [field: SerializeField] public bool IsRunAway { get; private set; }

    protected override void DecideBehavior()
    {
        base.DecideBehavior();

        if (UnitData.chaseTarget == null)
            LookAround();

        if (UnitData.chaseTarget == null)
            RandomStep();
        else
        {
            if (UnitsInSight.Contains(UnitData.chaseTarget.Owner))
            {
                UnitData.chaseTargetRecentCoord = UnitData.chaseTarget.coord;
                if (UnitData.coord.IsTargetInRange(UnitData.chaseTarget.coord, UnitData.atkRange.Total()))
                {
                    Controllable = false;
                    BasicAttack.StartSkill(UnitData.chaseTarget.coord);
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
                    else if (!FindPath(UnitData.chaseTargetRecentCoord))
                    {
                        //RandomStep();
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
        UpdateSightArea();
        CheckNewInSight();
        dm.map.GetElementAt(UnitData.coord.x, UnitData.coord.y).unit = this;
        if (UnitData.chaseTarget == null)
            LookAround();
        else if(UnitsInSight.Contains(UnitData.chaseTarget.Owner))
            UnitData.chaseTargetRecentCoord = UnitData.chaseTarget.coord;

        EndTurn(100m / UnitData.speed.Total());
    }

    public void LookAround()
    {
        CheckNewInSight();
        for(int i=0; i<UnitsInSight.Count; i++)
        {
            if (IsAggressive&&IsHostileUnit(UnitsInSight[i]) && !UnitData.hostileTargets.Contains(UnitsInSight[i].UnitData))
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

    public override void UpdateSightArea()
    {
        bool checkSight = false;

        int sight = UnitData.sight.Total();
        int left = Mathf.Max(2, UnitData.coord.x - sight);
        int right = Mathf.Min(dm.map.arrSize.x - 2, UnitData.coord.x + sight);
        int bottom = Mathf.Max(2, UnitData.coord.y - sight);
        int top = Mathf.Min(dm.map.arrSize.y - 2, UnitData.coord.y + sight);

        for(int i=left; i<=right; i++)
        {
            for(int j=bottom; j<=top; j++)
            {
                if(dm.map.GetElementAt(i,j).unit!=null && IsHostileUnit(dm.map.GetElementAt(i, j).unit))
                {
                    checkSight = true;
                    break;
                }
            }
        }

        if (checkSight)
            base.UpdateSightArea();
    }

    public override void GetDamage(AttackData attackData)
    {
        base.GetDamage(attackData);

        UnitData temp = UnitData.chaseTarget;
        CheckNewInSight();
        if (UnitsInSight.Contains(attackData.Attacker))
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

        if (Random.Range(0, 9) == 0)
        {
            List<string> list = GameManager.Instance.StringData.EquipTier[0].forClass[0].list;
            string tempKey = list[Random.Range(0, list.Count)];
            EquipmentBase tempBase = dm.GetEquipmentBase(tempKey);
            if (tempBase != null)
            {
                ItemObject itemTemp = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
                itemTemp.Init(dm, new Coordinate((Vector2)transform.position), new EquipmentData(tempBase));
                GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(itemTemp.Data);
                itemTemp.Bounce();
                dm.GetTileByCoordinate(itemTemp.Coord).items.Push(itemTemp);
            }
        }
        if (Random.Range(0, 2) == 0)
        {
            int pick = Random.Range(0, GameManager.Instance.StringData.MiscItems.Count);
            ItemObject item = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
            item.Init(dm, new Coordinate((Vector2)transform.position), new MiscData(GameManager.Instance.GetMiscBase(GameManager.Instance.StringData.MiscItems[pick]), 1));
            GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(item.Data);
            item.Bounce();
            dm.GetTileByCoordinate(item.Coord).items.Push(item);
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
