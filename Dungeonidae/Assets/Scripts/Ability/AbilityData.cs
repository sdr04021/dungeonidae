using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityData
{
    [SerializeField]
    string key;
    public string Key { get => key; }

    [SerializeField]
    Sprite sprite;
    public Sprite Sprite { get => sprite; }

    [SerializeField]
    int[] effectValues;
    public int[] EffectValues { get => effectValues; }

    int level = 0;
    public int Level { get => level; }

    public AbilityData(AbilityBase ability)
    {
        key = ability.Key;
        sprite = ability.Sprite;
        effectValues = ability.EffectValues;
    }

    public void IncreaseLevel()
    {
        if (level < 3) level++;
    }
}
