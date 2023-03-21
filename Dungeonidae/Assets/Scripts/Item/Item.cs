using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item
{
    Sprite mySprite;
    public Sprite MySprite { get => mySprite; }

    int index;
    public int Index { get => index; }

    int price;
    public int Price { get => price; }

    string description;
    public string Description { get => description; }

    public Item(ItemData item)
    {
        mySprite = item.Sprite;
        index = item.Index;
        price = item.Price;
        description = item.Discription;
    }
}
