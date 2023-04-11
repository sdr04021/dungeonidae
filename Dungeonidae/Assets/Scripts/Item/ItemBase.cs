using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [field: SerializeField]
    public string Key { get; private set; }

    [field: SerializeField]
    public int Price { get; private set; }
}
