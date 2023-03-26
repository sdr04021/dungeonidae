using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemObject : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;
    public ItemData data { get; private set; }

    DungeonManager dm;
    public Coordinate Coord { get; private set; }

    Sequence dropSequence;

    public void Init(DungeonManager dungeonManager, Coordinate c, ItemData item)
    {
        dm = dungeonManager;
        Coord = c;
        data = item;
        spriteRenderer.sprite = data.MySprite;
        //Vector2 spriteSize = new Vector2(myItem.MySprite.texture.width, myItem.MySprite.texture.height);
        Vector2 spriteSize = new Vector2(data.MySprite.textureRect.size.x, data.MySprite.textureRect.size.y);
        transform.localScale = new Vector2(60 / spriteSize.x, 60 / spriteSize.y);
    }

    public void Bounce()
    {
        dropSequence = transform.DOJump(new Vector3(Coord.x, Coord.y - 0.2f, transform.position.z), 0.4f, 1, 0.5f);
    }
    public void Drop()
    {
        transform.DOMove(new Vector3(Coord.x, Coord.y - 0.2f, transform.position.z), 0.2f);
    }

    private void OnDestroy()
    {
        dropSequence?.Kill();
    }
}
