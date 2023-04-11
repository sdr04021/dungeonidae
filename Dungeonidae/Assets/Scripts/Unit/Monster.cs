using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.GraphicsBuffer;

public class Monster : Unit
{

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        
    }

    public override void StartTurn()
    {
        base.StartTurn();

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
                UnitData.chaseTarget = null;
                if (!FindPath(UnitData.chaseTargetRecentCoord))
                {
                    RandomStep();
                }
                else FollowPath();
            }
        }
        //controllable = false;
        //dungeonManager.EndTurn();
    }

    public override void GetDamage(AttackData attackData)
    {
        base.GetDamage(attackData);
        UnitData.chaseTarget = attackData.Attacker.UnitData;
    }

    protected override void StartDie()
    {
        base.StartDie();

        //ItemObject item = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
        //item.Init(dm, new Coordinate((Vector2)transform.position), new Item(GameManager.Instance.testItem));
        //dm.GetTileByCoordinate(item.Coord).items.Push(item);

        dm.Player.IncreaseExp(UnitData.expReward[0]);

        ItemObject item = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
        item.Init(dm, new Coordinate((Vector2)transform.position), new EquipmentData(GameManager.Instance.testEquip));
        GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(new ItemDataContainer(item.data));
        item.Bounce();
        dm.GetTileByCoordinate(item.Coord).items.Push(item);
        item = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
        item.Init(dm, new Coordinate((Vector2)transform.position), new MiscData(GameManager.Instance.testItem, 1));
        GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(new ItemDataContainer(item.data));
        item.Bounce();
        dm.GetTileByCoordinate(item.Coord).items.Push(item);
    }
}
