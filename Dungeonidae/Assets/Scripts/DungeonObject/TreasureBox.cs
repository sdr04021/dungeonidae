using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox : DungeonObject
{
    [SerializeField] Sprite openedSprite;
    [SerializeField] ItemType itemType;
    [SerializeField] string KeyKey;

    public override void Load(DungeonManager dm, DungeonObjectData dungeonObjectData)
    {
        base.Load(dm, dungeonObjectData);
        if (dungeonObjectData.isActivated)
        {
            SpriteRenderer.sprite = openedSprite;
            IsInteractable = false;
        }
    }

    public override void Interact()
    {
        UnitData playerData = GameManager.Instance.saveData.playerData;
        if (playerData.RemoveOneMisc(KeyKey))
        {
            SpriteRenderer.sprite = openedSprite;
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
            IsInteractable = false;
            DungeonObjectData.isActivated = true;
        }
    }
}
