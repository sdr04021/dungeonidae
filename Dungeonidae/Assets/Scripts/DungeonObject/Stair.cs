using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stair : DungeonObject
{
    [SerializeField] int moveAmount = 1;

    public override void Init(string key, DungeonManager dm, Coordinate coord)
    {
        base.Init(key, dm, coord);
        SpriteRenderer.sortingOrder += (-(int)LayerOrder.DungeonObject + (int)LayerOrder.Stair);
    }

    public override void Interact(Unit unit)
    {
        //base.Interaction();
        dm.MoveFloor(moveAmount);
    }
}
