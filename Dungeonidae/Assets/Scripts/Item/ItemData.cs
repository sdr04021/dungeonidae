using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData
{
    string key;
    public string Key { get => key; }

    Sprite mySprite;
    public Sprite MySprite { get => mySprite; }

    int price;
    public int Price { get => price; }

    public ItemData(ItemBase item)
    {
        key = item.Key;
        mySprite = item.Sprite;
        price = item.Price;
    }
}
