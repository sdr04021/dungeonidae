using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonObject : MonoBehaviour
{
    protected DungeonManager dm;
    [field: SerializeField] public bool IsPassable { get; private set; } = false;
    [field: SerializeField] public bool IsTargetable { get; private set; } = false;

    [field: SerializeField] public bool IsBlockSight { get; private set; } = false;

    [field: SerializeField] public bool IsAttackable { get; private set; } = false;
    [field: SerializeField] public bool IsInteractable { get; private set; } = false;

    public DungeonObjectData DungeonObjectData { get; private set; }

    public SpriteRenderer SpriteRenderer { get; private set; }

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(DungeonManager dm, Coordinate coord)
    {
        this.dm = dm;
        DungeonObjectData = new();
        DungeonObjectData.coord = coord;
        transform.position = coord.ToVector3(0);
        DungeonObjectData.Init(this, GetType());
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
}
