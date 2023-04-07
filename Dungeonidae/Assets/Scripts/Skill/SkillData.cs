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

    public int currentCoolDown = 0;

    [field: SerializeField]
    public bool NeedTarget { get; private set; }

    public SkillData(SkillBase skill)
    {
        Key = skill.key;
        Type = skill.type;
        Sprite = skill.sprite;
        EffectValues = skill.effectValues;
        MutableValueIndex = skill.mutableValueIndex;
        NeedTarget = skill.needTarget;
    }
}
