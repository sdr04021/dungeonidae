using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField]
    Sprite sprite;
    public Sprite Sprite { get => sprite; }

    [SerializeField]
    int index;
    public int Index { get => index; }

    [SerializeField]
    int price;
    public int Price { get => price; }

    [SerializeField] 
    string discription;
    public string Discription { get => discription; }
}
