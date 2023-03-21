using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemObject : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;
    public Item myItem { get; private set; }

    Coordinate coord;

    public void Init(Coordinate c, Item item)
    {
        coord = c;
        myItem = item;
        spriteRenderer.sprite = myItem.MySprite;
        Vector2 spriteSize = new Vector2(myItem.MySprite.texture.width, myItem.MySprite.texture.height);
        transform.localScale = new Vector2(60 / spriteSize.x, 60 / spriteSize.y);
        Drop();
    }

    public void Drop()
    {
        transform.DOJump(new Vector3(coord.x, coord.y - 0.2f, transform.position.z), 0.4f, 1, 0.5f);
    }
}
