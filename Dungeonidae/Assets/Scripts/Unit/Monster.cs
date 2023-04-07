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

        if (chaseTarget == null)
            RandomStep();
        else
        {
            if (UnitsInSight.Contains(chaseTarget))
            {
                chaseTargetRecentCoord = chaseTarget.Coord;
                if (Coord.IsTargetInRange(chaseTarget.Coord, UnitData.AtkRange.Total()))
                {
                    Controllable = false;
                    BasicAttack.StartSkill(chaseTarget.Coord);
                }
                else
                {
                    Directions dir = FollowTarget(chaseTarget.Coord);
                    if (dir == Directions.NONE)
                        RandomStep();
                    else Move(dir);
                }
            }
            else
            {
                chaseTarget = null;
                if (!FindPath(chaseTargetRecentCoord))
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
        chaseTarget = attackData.Attacker;
    }

    protected override void StartDie()
    {
        base.StartDie();

        //ItemObject item = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
        //item.Init(dm, new Coordinate((Vector2)transform.position), new Item(GameManager.Instance.testItem));
        //dm.GetTileByCoordinate(item.Coord).items.Push(item);

        dm.Player.IncreaseExp(UnitData.ExpReward[0]);

        ItemObject item = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
        item.Init(dm, new Coordinate((Vector2)transform.position), new EquipmentData(GameManager.Instance.testEquip));
        item.Bounce();
        dm.GetTileByCoordinate(item.Coord).items.Push(item);
        item = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
        item.Init(dm, new Coordinate((Vector2)transform.position), new MiscData(GameManager.Instance.testItem, 1));
        item.Bounce();
        dm.GetTileByCoordinate(item.Coord).items.Push(item);
    }
}
