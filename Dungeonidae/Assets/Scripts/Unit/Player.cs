using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Player : Unit
{
    protected override void Start()
    {
        base.Start();
    }

    public override void Init(DungeonManager dungeonManager, Coordinate c)
    {
        base.Init(dungeonManager, c);;
    }

    public override void StartTurn()
    {
        base.StartTurn();
    }

    protected override void EndBehavior()
    {
        base.EndBehavior();
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
}
