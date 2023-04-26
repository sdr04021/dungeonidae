using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Unit
{
    [field:SerializeField] public bool IsAggressive { get; private set; }
    [field:SerializeField] public bool IsForgetting { get; private set; }
    [field: SerializeField] public bool IsRunAway { get; private set; }

    protected override void Start()
    {
        base.Start();
    }

    public override void StartTurn()
    {
        base.StartTurn();

        if(IsAggressive&&UnitData.chaseTarget==null)
            AggressiveStart();

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
                if (IsForgetting) UnitData.chaseTarget = null;
                if (!FindPath(UnitData.chaseTargetRecentCoord))
                {
                    RandomStep();
                }
                else
                {
                   Move(path.Pop());
                }
                if (UnitData.coord == UnitData.chaseTargetRecentCoord)
                    UnitData.chaseTarget = null;
            }
        }
        //controllable = false;
        //dungeonManager.EndTurn();
    }

    public void AggressiveStart()
    {
        for(int i=0; i<UnitsInSight.Count; i++)
        {
            if (IsHostileUnit(UnitsInSight[i]))
            {
                UnitData.chaseTarget = UnitsInSight[i].UnitData;
                        UnitData.isChasingTarget = true;
                return;
            }
        }
    }

    public override void GetDamage(AttackData attackData)
    {
        base.GetDamage(attackData);
        UnitData.chaseTarget = attackData.Attacker.UnitData;
        UnitData.isChasingTarget = true;
    }

    protected override void StartDie()
    {
        base.StartDie();

        //ItemObject item = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
        //item.Init(dm, new Coordinate((Vector2)transform.position), new Item(GameManager.Instance.testItem));
        //dm.GetTileByCoordinate(item.Coord).items.Push(item);

        dm.Player.IncreaseExp((int)((UnitData.level + 1) * UnitBase.ExpCoefficient));

        ItemObject item = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
        item.Init(dm, new Coordinate((Vector2)transform.position), new EquipmentData(GameManager.Instance.testEquip));
        GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(item.data);
        item.Bounce();
        dm.GetTileByCoordinate(item.Coord).items.Push(item);
        item = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
        item.Init(dm, new Coordinate((Vector2)transform.position), new MiscData(GameManager.Instance.testItem, 1));
        GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(item.data);
        item.Bounce();
        dm.GetTileByCoordinate(item.Coord).items.Push(item);
    }
}
