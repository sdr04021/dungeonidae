using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField]
    string key;
    public string Key { get => key; }

    [SerializeField]
    Sprite sprite;
    public Sprite Sprite { get => sprite; }

    [SerializeField]
    int price;
    public int Price { get => price; }
}
