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

    Sprite _sprite;
    [JsonIgnore]
    public Sprite Sprite
    {
        get
        {
            if (_sprite == null)
            {
                LoadAbilityIcon();
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

    void LoadAbilityIcon()
    {
        loadHandle = Addressables.LoadAssetAsync<Sprite>("Assets/Sprites/Ability Icons/" + Key + ".png");
        loadHandle.WaitForCompletion();
        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            Sprite = loadHandle.Result;
    }

    ~AbilityData()
    {
        Addressables.Release(loadHandle);
    }
}
