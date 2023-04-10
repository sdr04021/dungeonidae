using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableLoader
{
    public async void LoadSkillIcon(AsyncOperationHandle<Sprite> loadHandle, string key)
    {
        loadHandle = Addressables.LoadAssetAsync<Sprite>("Assets/Sprites/Skill Icons/" + key + ".png");
        await loadHandle.Task;
    }
}
