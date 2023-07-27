using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Fog : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public bool IsOn { get; private set; } = true;
    public bool IsObserved { get; private set; } = false;
    Color dark = new(1, 1, 1, 0.6f);
    DungeonManager dm;
    int x, y;
    //int identifier = 15;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(DungeonManager dm, int x, int y)
    {
        this.dm = dm;
        this.x = x; this.y = y;
        transform.position = new Vector2(x, y);
        if (IsObserved)
            spriteRenderer.color = dark;
        if (IsOn)
            spriteRenderer.enabled = true;
        spriteRenderer.sortingOrder = spriteRenderer.sortingOrder = 1000 - (10 * (y - 1)) + (int)LayerOrder.Fog;
    }


    public void Clear()
    {
        if (!IsObserved)
        {
            spriteRenderer.color = dark;
            IsObserved = true;
            GameManager.Instance.saveData.GetCurrentDungeonData().observedFog.Add(new Coordinate(x, y));
        }
        IsOn = false;
        gameObject.SetActive(false);
        //spriteRenderer.sortingOrder = 0;
        //spriteRenderer.enabled = false;
        //SendNeighborSignal();
    }
    public void Cover()
    {
        IsOn = true;
        gameObject.SetActive(true);
        //spriteRenderer.sortingOrder = 10;
        //SendNeighborSignal();
        //SetSprite();
        //spriteRenderer.enabled = true;
    }
    public void LoadObserved()
    {
        spriteRenderer.color = dark;
        IsObserved = true;
    }

    public void ResetFog()
    {
        spriteRenderer.color = Color.black;
        IsObserved = false;
        Cover();
    }

    /*
    public void UpdateSprite()
    {
        if (!gameObject.activeSelf) return;

        int identifier = 0;
        if ((x == 0) || dm.fogMap.GetElementAt(x - 1, y).gameObject.activeSelf)
            identifier += 1;
        if ((y == 0) || dm.fogMap.GetElementAt(x, y - 1).gameObject.activeSelf)
            identifier += 2;
        if ((x == dm.fogMap.arrSize.x - 1) || dm.fogMap.GetElementAt(x + 1, y).gameObject.activeSelf)
            identifier += 4;
        if ((y == dm.fogMap.arrSize.y - 1) || dm.fogMap.GetElementAt(x, y + 1).gameObject.activeSelf)
            identifier += 8;
        if (this.identifier != identifier)
        {
            spriteRenderer.sprite = GameManager.Instance.fogSprites[identifier];
            this.identifier = identifier;
        }
    }
    */
}
