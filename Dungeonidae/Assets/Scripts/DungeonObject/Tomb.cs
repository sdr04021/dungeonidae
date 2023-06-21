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

    public override void Interact(Unit unit)
    {
        groundRenderer.sprite = diggedGroundSprite;
        if (Random.Range(0, 2) == 0)
        {
            List<string> list = GameManager.Instance.StringData.EquipTier[0].forClass[0].list;
            string tempKey = list[Random.Range(0, list.Count)];
            EquipmentBase tempBase = dm.GetEquipmentBase(tempKey);
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
            Monster wraith = dm.InstantiateMonster("WRAITH", DungeonObjectData.coord);
            wraith.EffectAnimator.SetTrigger("WRAITH_APPEAR");
        }
        IsInteractable = false;
        DungeonObjectData.isActivated = true;
    }
}
