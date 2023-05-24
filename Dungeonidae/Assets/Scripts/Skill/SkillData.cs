using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public class SkillData
{
    [JsonProperty]
    public string Key { get; private set; }

    [JsonProperty]
    public SkillType Type { get; private set; }

    [JsonProperty]
    public int[] EffectValues { get; private set; }

    [JsonProperty]
    public int[] MutableValueIndex { get; private set; }

    public int currentCoolDown = 0;

    [JsonProperty]
    public bool NeedTarget { get; private set; }

    public SkillData(SkillBase skill)
    {
        if (skill == null) return;
        Key = skill.key;
        Type = skill.type;
        EffectValues = skill.effectValues;
        MutableValueIndex = skill.mutableValueIndex;
        NeedTarget = skill.needTarget;
    }

    public Sprite GetSprite()
    {
        return GameManager.Instance.GetSprite(SpriteAssetType.Skill, Key);
    }
}
