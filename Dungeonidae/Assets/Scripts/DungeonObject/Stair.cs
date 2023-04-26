using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stair : DungeonObject
{
    [SerializeField] int moveAmount = 1;

    public override void Interact()
    {
        //base.Interaction();
        dm.MoveFloor(moveAmount);
    }
}
