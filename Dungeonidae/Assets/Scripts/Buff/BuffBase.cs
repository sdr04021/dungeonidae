using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffBase", menuName = "Scriptable Object/BuffBase")]
public class BuffBase : ScriptableObject
{
    [field: SerializeField] public string Key { get; private set; }
    [field: SerializeField] public BuffType BuffType { get; private set; }
    [field: SerializeField] public bool IsStackable { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public List<int> EffectValues { get; private set; }
}
