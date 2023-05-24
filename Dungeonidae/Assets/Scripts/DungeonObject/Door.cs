using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : DungeonObject
{
    [SerializeField] SpriteRenderer door;

    public override void Init(string key, DungeonManager dm, Coordinate coord)
    {
        base.Init(key, dm, coord);
        SpriteRenderer.sortingOrder += (-(int)LayerOrder.DungeonObject + (int)LayerOrder.TopWall);
        door.sortingOrder = SpriteRenderer.sortingOrder;
        dm.map.GetElementAt(coord).sightBlocker.SetActive(true);
    }

    public override void TargetedInteraction()
    {
        if (IsPassable)
        {
            IsPassable = false;
            door.gameObject.SetActive(true);
            IsBlockSight = true;
            IsInteractsWithThrownItem = true;
            dm.map.GetElementAt(DungeonObjectData.coord).sightBlocker.SetActive(true);
            dm.UpdateSightAreaNearThis(DungeonObjectData.coord);
            dm.UpdateUnitRenderers();
        }
        else
        {
            IsPassable = true;
            door.gameObject.SetActive(false);
            IsBlockSight = false;
            IsInteractsWithThrownItem = false;
            dm.map.GetElementAt(DungeonObjectData.coord).sightBlocker.SetActive(false);
            dm.UpdateSightAreaNearThis(DungeonObjectData.coord);
            dm.UpdateUnitRenderers();
        }
    }

    public override bool IsTargetable()
    {
        if (dm.map.GetElementAt(DungeonObjectData.coord).items.Count == 0)
            return isTargetable;
        else return false;
    }
}
