using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillBase", menuName = "Scriptable Object/SkillBase")]
public class SkillBase : ScriptableObject
{
    [field: SerializeField]
    public string Key { get; private set; }

    [field: SerializeField]
    public SkillType Type { get; private set; }

    [field: SerializeField]
    public Sprite Sprite { get; private set; }

    [field: SerializeField]
    public int[] EffectValues { get; private set; }

    [field: SerializeField]
    public int[] MutableValueIndex { get; private set; }
}
