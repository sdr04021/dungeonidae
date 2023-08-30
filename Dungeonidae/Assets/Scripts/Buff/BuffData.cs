using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuffData
{
    [field: SerializeField] public string Key { get; private set; }
    [JsonIgnore] BuffBase baseData;
    [JsonIgnore]
    public BuffBase BaseData
    {
        get
        {
            if (baseData == null) baseData = GameManager.Instance.GetBuffBase(Key);
            return baseData;
        }
    }

    [field: SerializeField] public int MaxDuration { get; private set; }    
    public int durationLeft = 0;
    public int stack = 0;
    [JsonIgnore] public GameObject effectAnimation;

    public BuffData() { }
    public BuffData(string key, int maxDuration)
    {       
        Key = key;
        baseData = GameManager.Instance.GetBuffBase(key);
        MaxDuration = maxDuration;
        durationLeft = maxDuration;
    }
}
