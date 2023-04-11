using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Fog : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public FogData FogData { get; private set; }
    Color dark = new(1, 1, 1, 0.8f);
    DungeonManager dm;
    int x, y;
    bool[] isNeighborOn = { true, true, true, true };

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(DungeonManager dm, FogData fogData, int x, int y)
    {
        this.dm = dm;
        FogData = fogData;
        this.x = x; this.y = y;
        transform.position = new Vector2(x, y);
        if (fogData.IsObserved)
            spriteRenderer.color = dark;
        if(!fogData.IsOn)
            spriteRenderer.enabled = false;
    }


    public void Clear()
    {
        if (!FogData.IsObserved)
        {
            spriteRenderer.color = dark;
            FogData.IsObserved = true;
            GameManager.Instance.saveData.GetCurrentDungeonData().observedFog.Add(new Coordinate(x, y));
        }
        FogData.IsOn = false;
        spriteRenderer.enabled = false;
        SendNeighborSignal();
    }
    public void Cover()
    {
        FogData.IsOn = true;
        SendNeighborSignal();
        SetSprite();
        spriteRenderer.enabled = true;
    }

    void SendNeighborSignal()
    {
        if (y + 1 < dm.fogMap.arrSize.y)
            dm.fogMap.GetElementAt(x, y + 1).SetSprite();
        if (x + 1 < dm.fogMap.arrSize.x)
            dm.fogMap.GetElementAt(x + 1, y).SetSprite();
        if (y - 1 >= 0)
            dm.fogMap.GetElementAt(x, y - 1).SetSprite();
        if (x - 1 >= 0)
            dm.fogMap.GetElementAt(x - 1, y).SetSprite();
    }

    void UpdateNeighborState()
    {
        if (y + 1 < dm.fogMap.arrSize.y)
        {
            if (dm.fogMap.GetElementAt(x, y + 1).FogData.IsOn != FogData.IsOn)
                isNeighborOn[0] = false;
            else 
                isNeighborOn[0] = dm.fogMap.GetElementAt(x, y + 1).FogData.IsOn;
        }
        if (x + 1 < dm.fogMap.arrSize.x)
        {
            if (dm.fogMap.GetElementAt(x + 1, y).FogData.IsOn != FogData.IsOn)
                isNeighborOn[1] = false;
            else
                isNeighborOn[1] = dm.fogMap.GetElementAt(x + 1, y).FogData.IsOn;
        }
        if (y - 1 >= 0)
        {
            if (dm.fogMap.GetElementAt(x, y - 1).FogData.IsOn != FogData.IsOn)
                isNeighborOn[2] = false;
            else
                isNeighborOn[2] = dm.fogMap.GetElementAt(x, y - 1).FogData.IsOn;
        }
        if (x - 1 >= 0)
        {
            if (dm.fogMap.GetElementAt(x - 1, y).FogData.IsOn != FogData.IsOn)
                isNeighborOn[3] = false;
            else
                isNeighborOn[3] = dm.fogMap.GetElementAt(x - 1, y).FogData.IsOn;
        }
        /*
                 if (y + 1 < dm.FogMap.GetLength(1))
            isNeighborOn[0] = dm.FogMap[x, y + 1].IsOn;
        if (x + 1 < dm.FogMap.GetLength(0))
            isNeighborOn[1] = dm.FogMap[x + 1, y].IsOn;
        if (y - 1 >= 0)
            isNeighborOn[2] = dm.FogMap[x, y - 1].IsOn;
        if (x - 1 >= 0)
            isNeighborOn[3] = dm.FogMap[x - 1, y].IsOn;
        */
    }

    public void SetSprite()
    {
        UpdateNeighborState();

        int identifier = 0;
        if (isNeighborOn[0]) identifier += 1;
        if (isNeighborOn[1]) identifier += 2;
        if (isNeighborOn[2]) identifier += 4;
        if (isNeighborOn[3]) identifier += 8;

        transform.rotation = Quaternion.Euler(0, 0, 0);
        switch (identifier)
        {
            case 0:
                spriteRenderer.sprite = GameManager.Instance.fogSprites[0];
                break;
            case 1:
                spriteRenderer.sprite = GameManager.Instance.fogSprites[1];
                transform.rotation = Quaternion.Euler(0, 0, 270);
                break;
            case 2:
                spriteRenderer.sprite = GameManager.Instance.fogSprites[1];
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case 3:
                spriteRenderer.sprite = GameManager.Instance.fogSprites[2];
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case 4:
                spriteRenderer.sprite = GameManager.Instance.fogSprites[1];
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case 5:
                spriteRenderer.sprite = GameManager.Instance.fogSprites[3];
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case 6:
                spriteRenderer.sprite = GameManager.Instance.fogSprites[2];
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case 7:
                spriteRenderer.sprite = GameManager.Instance.fogSprites[4];
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case 8:
                spriteRenderer.sprite = GameManager.Instance.fogSprites[1];
                break;
            case 9:
                spriteRenderer.sprite = GameManager.Instance.fogSprites[2];
                transform.rotation = Quaternion.Euler(0, 0, 270);
                break;
            case 10:
                spriteRenderer.sprite = GameManager.Instance.fogSprites[3];
                break;
            case 11:
                spriteRenderer.sprite = GameManager.Instance.fogSprites[4];
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case 12:
                spriteRenderer.sprite = GameManager.Instance.fogSprites[2];
                break;
            case 13:
                spriteRenderer.sprite = GameManager.Instance.fogSprites[4];
                transform.rotation = Quaternion.Euler(0, 0, 270);
                break;
            case 14:
                spriteRenderer.sprite = GameManager.Instance.fogSprites[4];
                break;
            case 15:
                spriteRenderer.sprite = GameManager.Instance.fogSprites[5];
                break;
        }

    }
}
