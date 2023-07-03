using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemObject : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] SpriteRenderer shadow;
    public ItemData Data { get; private set; }
    public System.Type DataType { get; private set; } 

    DungeonManager dm;
    public Coordinate Coord { get; private set; }

    public void Init(DungeonManager dungeonManager, Coordinate c, ItemData item)
    {
        dm = dungeonManager;
        Coord = c;
        Data = item;
        Data.owner = this;
        item.coord = Coord;
        DataType = item.GetType();
        spriteRenderer.sprite = DataType == typeof(MiscData) ? GameManager.Instance.GetSprite(SpriteAssetType.Misc, item.Key) : GameManager.Instance.GetSprite(SpriteAssetType.Equipment, item.Key);
        //Vector2 spriteSize = new Vector2(data.Sprite.textureRect.size.x, data.Sprite.textureRect.size.y);
        //transform.localScale = new Vector2(60 / spriteSize.x, 60 / spriteSize.y);
        spriteRenderer.sortingOrder = 1000 - (10 * Coord.y) + (int)LayerOrder.ItemObject;
        shadow.sortingOrder = spriteRenderer.sortingOrder - 1;
    }

    public void Bounce()
    {
        transform.DOJump(new Vector3(Coord.x, Coord.y - 0.25f, transform.position.z), 0.4f, 1, 0.5f);
    }
    public void Drop()
    {
        transform.DOMove(new Vector3(Coord.x, Coord.y - 0.25f, transform.position.z), 0.2f);
    }
    public void Loot()
    {
        transform.DOMove(new Vector3(Coord.x, Coord.y, transform.position.z), 0.1f).OnComplete(() => { Destroy(gameObject); });
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
