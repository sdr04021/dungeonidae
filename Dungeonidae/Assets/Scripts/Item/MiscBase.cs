using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MiscBase", menuName = "Scriptable Object/MiscBase")]
public class MiscBase : ItemBase
{
    [SerializeField]
    int[] effectValues;
    public int[] EffectValues { get => effectValues; }

    [SerializeField]
    int maxStack = 1;
    public int MaxStack { get => maxStack; }
}
