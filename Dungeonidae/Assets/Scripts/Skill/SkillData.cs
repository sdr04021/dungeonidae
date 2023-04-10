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

    Sprite _sprite;
    [JsonIgnore]
    public Sprite Sprite
    {
        get
        {
            if (_sprite == null)
            {
                LoadSkillIcon();
            }
            return _sprite;
        }
        private set
        {
            _sprite = value;
        }
    }

    [JsonProperty]
    public int[] EffectValues { get; private set; }

    [JsonProperty]
    public int[] MutableValueIndex { get; private set; }

    public int currentCoolDown = 0;

    [JsonProperty]
    public bool NeedTarget { get; private set; }

    AsyncOperationHandle<Sprite> loadHandle;

    public SkillData(SkillBase skill)
    {
        if (skill == null) return;
        Key = skill.key;
        Type = skill.type;
        //Sprite = skill.sprite;
        EffectValues = skill.effectValues;
        MutableValueIndex = skill.mutableValueIndex;
        NeedTarget = skill.needTarget;
    }

    void LoadSkillIcon()
    {
        loadHandle = Addressables.LoadAssetAsync<Sprite>("Assets/Sprites/Skill Icons/" + Key + ".png");
        loadHandle.WaitForCompletion();
        //await loadHandle.Task;
        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            Sprite = loadHandle.Result;
        //GameManager.Instance.saveData.playerData.InvokeSkillChanged();
    }

    ~SkillData()
    {
        Addressables.Release(loadHandle);
    }
}
