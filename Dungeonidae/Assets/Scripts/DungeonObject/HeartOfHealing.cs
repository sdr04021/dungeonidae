using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartOfHealing : DungeonObject
{
    public override void Interact(Unit unit)
    {
        if (unit.UnitData.team == Team.Player)
        {
            unit.RecoverHp((int)(unit.UnitData.maxHp.Total() * 0.3f));
            dm.map.GetElementAt(DungeonObjectData.coord).dungeonObjects.Remove(this);
            dm.RemoveDungeonObjectData(DungeonObjectData);
            Destroy(gameObject);
        }
    }
}
