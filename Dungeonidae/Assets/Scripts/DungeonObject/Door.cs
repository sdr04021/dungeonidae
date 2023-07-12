using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : DungeonObject
{
    [SerializeField] SpriteRenderer door;
    [SerializeField] SpriteRenderer backgroundBottom;

    public override void Init(string key, DungeonManager dm, Coordinate coord)
    {
        base.Init(key, dm, coord);
        if(backgroundBottom!=null)
            backgroundBottom.sortingOrder = SpriteRenderer.sortingOrder - (int)LayerOrder.DungeonObject + (int)LayerOrder.TopWall;
        door.sortingOrder = SpriteRenderer.sortingOrder - (int)LayerOrder.DungeonObject + (int)LayerOrder.Unit;
    }

    public override void Load(DungeonManager dm, DungeonObjectData dungeonObjectData)
    {
        base.Load(dm, dungeonObjectData);
        if (backgroundBottom != null)
            backgroundBottom.sortingOrder = SpriteRenderer.sortingOrder - (int)LayerOrder.DungeonObject + (int)LayerOrder.TopWall;
        door.sortingOrder = SpriteRenderer.sortingOrder - (int)LayerOrder.DungeonObject + (int)LayerOrder.Unit;
        if (dungeonObjectData.isActivated)
        {
            IsPassable = true;
            door.gameObject.SetActive(false);
            IsBlockSight = false;
            IsInteractsWithThrownItem = false;
        }
    }

    public override void Activate(Unit unit)
    {
        if (IsPassable)
        {
            IsPassable = false;
            door.gameObject.SetActive(true);
            IsBlockSight = true;
            IsInteractsWithThrownItem = true;
            DungeonObjectData.isActivated = false;
            dm.UpdateSightAreaNearThis(DungeonObjectData.coord);
            dm.UpdateUnitRenderers();
        }
        else
        {
            IsPassable = true;
            door.gameObject.SetActive(false);
            IsBlockSight = false;
            IsInteractsWithThrownItem = false;
            DungeonObjectData.isActivated = true;
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
