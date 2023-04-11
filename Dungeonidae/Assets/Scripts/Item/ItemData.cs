using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public class ItemData
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
                LoadItemIcon();
            }
            return _sprite;
        }
        protected set
        {
            _sprite = value;
        }
    }

    [JsonProperty]
    public int Price { get; private set; }

    protected AsyncOperationHandle<Sprite> loadHandle;

    public Coordinate coord;

    public ItemData() { }

    public ItemData(ItemBase item)
    {
        Key = item.Key;
        Price = item.Price;
    }

    protected virtual void LoadItemIcon()
    {

    }

    ~ItemData()
    {
        Addressables.Release(loadHandle);
    }
}
