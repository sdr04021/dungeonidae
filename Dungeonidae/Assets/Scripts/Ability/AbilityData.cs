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
    public int Level { get; private set; }

    [JsonIgnore]
    AbilityBase baseData;
    [JsonIgnore]
    public AbilityBase BaseData
    {
        get
        {
            if (baseData == null) baseData = GameManager.Instance.GetAbilityBase(Key);
            return baseData;
        }
    }

    public AbilityData() { }
    public AbilityData(AbilityBase ability)
    {
        Key = ability.Key;
        baseData = ability;
    }
    public void IncreaseLevel()
    {
        if (Level < 3) Level++;
    }
}
