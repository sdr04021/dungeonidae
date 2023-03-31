using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillData
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

    public int coolDown = 0;

    public SkillData(SkillBase skill)
    {
        Key = skill.Key;
        Type = skill.Type;
        Sprite = skill.Sprite;
        EffectValues = skill.EffectValues;
        MutableValueIndex = skill.MutableValueIndex;
    }
}
