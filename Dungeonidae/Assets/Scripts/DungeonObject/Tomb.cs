using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tomb : DungeonObject
{
    [SerializeField] SpriteRenderer groundRenderer;
    [SerializeField] Sprite diggedGroundSprite;
    int seed;

    public override void Init(string key, DungeonManager dm, Coordinate coord)
    {
        base.Init(key, dm, coord);
        groundRenderer.sortingOrder = SpriteRenderer.sortingOrder;
    }

    public override void Load(DungeonManager dm, DungeonObjectData dungeonObjectData)
    {
        base.Load(dm, dungeonObjectData);
        groundRenderer.sortingOrder = SpriteRenderer.sortingOrder;
        if (dungeonObjectData.isActivated)
        {
            groundRenderer.sprite = diggedGroundSprite;
            IsInteractable = false;
        }
    }

    public void SetSeed(int seed)
    {
        this.seed = seed;
    }

    public override void Activate(Unit unit)
    {
        groundRenderer.sprite = diggedGroundSprite;
        if (Random.Range(0, 2) == 0)
        {
            List<string> list = GameManager.Instance.StringData.Artifacts;
            string tempKey = list[Random.Range(0, list.Count)];
            EquipmentBase tempBase = GameManager.Instance.GetEquipmentBase(tempKey);
            if (tempBase != null)
            {
                ItemObject itemTemp = Instantiate(GameManager.Instance.itemObjectPrefab, transform.position, Quaternion.identity);
                itemTemp.Init(dm, new Coordinate((Vector2)transform.position), new EquipmentData(tempBase));
                GameManager.Instance.saveData.GetCurrentDungeonData().fieldItemList.Add(itemTemp.Data);
                itemTemp.Bounce();
                dm.GetTileByCoordinate(itemTemp.Coord).items.Push(itemTemp);
            }
        }
        else
        {
            Monster wraith = dm.InstantiateMonster("WRAITH", DungeonObjectData.coord, GameManager.Instance.saveData.currentFloor);
            wraith.EffectAnimator.SetTrigger("WRAITH_APPEAR");
        }
        IsInteractable = false;
        DungeonObjectData.isActivated = true;
    }
}
