using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableSpriteLoader : MonoBehaviour
{
    private string address = "Assets/Sprites/Skill Icons/SPRINT.png";

    AssetReference newSprite;

    public void StartLoad()
    {
        Addressables.LoadAssetAsync<Sprite>(address).Completed += SpriteLoaded;
    }

    void SpriteLoaded(AsyncOperationHandle<Sprite> obj)
    {
        if(obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log(obj.Result.name.ToString());
        }
    }
}
