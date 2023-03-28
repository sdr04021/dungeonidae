using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityBase", menuName = "Scriptable Object/AbilityBase")]
public class AbilityBase : ScriptableObject
{
    [field:SerializeField]
    public string Key {get; private set;}

    [field: SerializeField]
    public Sprite Sprite { get; private set; }

    [field: SerializeField]
    public int[] EffectValues { get; private set; }
}
