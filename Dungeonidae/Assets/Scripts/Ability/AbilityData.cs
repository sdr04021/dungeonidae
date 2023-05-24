using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public class AbilityData
{
    [JsonProperty]
    public string Key { get; private set; }

    [JsonProperty]
    public int[] EffectValues { get; private set; }

    [JsonProperty]
    public int Level { get; private set; }

    AsyncOperationHandle<Sprite> loadHandle;

    public AbilityData() { }

    public AbilityData(AbilityBase ability)
    {
        Key = ability.Key;
        EffectValues = ability.EffectValues;
    }
    public void IncreaseLevel()
    {
        if (Level < 3) Level++;
    }

    public Sprite GetSprite()
    {
       return GameManager.Instance.GetSprite(SpriteAssetType.Ability, Key);
    }

    ~AbilityData()
    {
        Addressables.Release(loadHandle);
    }
}
