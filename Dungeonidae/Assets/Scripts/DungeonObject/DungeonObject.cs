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
    [field: SerializeField] public bool IsInteractsWithThrownItem { get; protected set; } = false;

    public DungeonObjectData DungeonObjectData { get; private set; }

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
        DungeonObjectData.Init(this, GetType(), key);
        dm.map.GetElementAt(coord).dungeonObjects.Add(this);
        SpriteRenderer.sortingOrder = 1000 - (10 * coord.y) + (int)LayerOrder.DungeonObject;
    }

    public void Load(DungeonManager dm, DungeonObjectData dungeonObjectData)
    {
        this.dm = dm;
        DungeonObjectData = dungeonObjectData;
        dungeonObjectData.SetOwner(this);
        transform.position = DungeonObjectData.coord.ToVector3(0);
    }

    public virtual void Interact()
    {

    }

    public virtual void TargetedInteraction()
    {

    }

    public virtual bool IsTargetable()
    {
        return isTargetable;
    }
}
