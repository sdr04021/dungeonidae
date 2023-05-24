using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BuffData
{
    [field: SerializeField]
    public string Key { get; private set; }

    [field: SerializeField]
    public BuffType Type { get; private set; }

    [field: SerializeField]
    public Sprite Sprite { get; private set; }

    [field: SerializeField]
    public List<int> EffectValues { get; private set; }

    [field: SerializeField]
    public int MaxDuration { get; private set; }    
    public int durationLeft = 0;

    public BuffData(BuffBase buff, int maxDuration)
    {
        Key = buff.key;
        Type = buff.buffType;
        Sprite = buff.sprite;
        EffectValues = buff.effectValues;
        MaxDuration = maxDuration;
        durationLeft = maxDuration;
    }
    public BuffData(BuffBase buff, SkillData skill)
    {
        Key = buff.key;
        Type = buff.buffType;
        Sprite = buff.sprite;
        EffectValues = new();
        for (int i = 1; i < skill.EffectValues.Length - 1; i++)
        {
            EffectValues.Add(skill.EffectValues[i]);
        }
        MaxDuration = EffectValues[0];
    }
    public BuffData(BuffBase buff, MiscData misc)
    {
        Key = buff.key;
        Type = buff.buffType;
        Sprite = buff.sprite;
        EffectValues = GameManager.Instance.GetMiscBase(misc.Key).EffectValues.ToList();
        MaxDuration = EffectValues[0];
    }
}
