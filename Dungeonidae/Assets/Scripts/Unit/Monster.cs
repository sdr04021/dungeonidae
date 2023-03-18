using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                if (Coord.IsTargetInRange(chaseTarget.Coord, Data.atkRange.Total()))
                {
                    StartBasicAttack(chaseTarget);
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
}
