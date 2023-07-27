using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : DungeonObject
{
    public override void Init(string key, DungeonManager dm, Coordinate coord)
    {
        base.Init(key, dm, coord);
        SpriteRenderer.sortingOrder = 1;
        if (DungeonObjectData.isHidden)
        {
            SpriteRenderer.enabled = false;
        }
    }
    public override void Load(DungeonManager dm, DungeonObjectData dungeonObjectData)
    {
        base.Load(dm, dungeonObjectData);
        SpriteRenderer.sortingOrder = 1;
        if (DungeonObjectData.isHidden)
        {
            SpriteRenderer.enabled = false;
        }
    }

    public override void Activate(Unit unit)
    {
        if (DungeonObjectData.isHidden)
        {
            DungeonObjectData.isHidden = false;
            SpriteRenderer.enabled = true;
        }
        TrapAttack(unit);
    }

    protected virtual void TrapAttack(Unit unit)
    {
        int damage = 10 + GameManager.Instance.saveData.currentFloor * 2;
        unit.GetDamage(damage, 0, 0);
    }
}
