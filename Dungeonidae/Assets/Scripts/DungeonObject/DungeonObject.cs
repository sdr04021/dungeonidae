using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonObject : MonoBehaviour
{
    protected DungeonManager dm;
    [field: SerializeField] public bool IsPassable { get; protected set; } = false;
    [SerializeField] protected bool isTargetable = false;
    [field: SerializeField] public bool IsBlockSight { get; protected set; } = false;
    [field: SerializeField] public bool IsInteractable { get; protected set; } = false;
    [field: SerializeField] public bool IsActivatesByThrownItem { get; protected set; } = false;
    [field: SerializeField] public bool IsInteractsWithCollision { get; protected set; } = false;
    [field: SerializeField] public DungeonObjectDurability Durability { get; protected set; } = DungeonObjectDurability.Unbreakable;
    [field: SerializeField] public DungeonObjectData DungeonObjectData { get; protected set; }

    [field:SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }

    private void Awake()
    {
        if (SpriteRenderer == null) SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void Init(string key, DungeonManager dm, Coordinate coord)
    {
        this.dm = dm;
        DungeonObjectData = new();
        DungeonObjectData.coord = coord;
        transform.position = coord.ToVector3(0);
        DungeonObjectData.Init(this, key);
        dm.map.GetElementAt(coord).dungeonObjects.Add(this);
        SpriteRenderer.sortingOrder = 1000 - (10 * coord.y) + (int)LayerOrder.DungeonObject;
    }

    public virtual void Load(DungeonManager dm, DungeonObjectData dungeonObjectData)
    {
        this.dm = dm;
        DungeonObjectData = dungeonObjectData;
        dungeonObjectData.SetOwner(this);
        transform.position = DungeonObjectData.coord.ToVector3(0);
        dm.map.GetElementAt(DungeonObjectData.coord).dungeonObjects.Add(this);
        SpriteRenderer.sortingOrder = 1000 - (10 * DungeonObjectData.coord.y) + (int)LayerOrder.DungeonObject;
    }

    public virtual void Activate(Unit unit)
    {

    }

    public virtual bool IsTargetable()
    {
        return isTargetable;
    }
}
