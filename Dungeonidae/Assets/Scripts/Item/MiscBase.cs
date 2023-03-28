using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MiscBase", menuName = "Scriptable Object/MiscBase")]
public class MiscBase : ItemBase
{
    [field: SerializeField]
    public int[] EffectValues { get; private set; }

    [field: SerializeField]
    public int MaxStack { get; private set; }
}
