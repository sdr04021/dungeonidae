using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameManager : MonoBehaviour
{
    public SaveData saveData;
    [field:SerializeField] public StringData StringData { get; private set; }

    public DungeonManager dungeonManager;
    public Player warriorPrefab;
    public Monster testMob1Prefab;
    public Monster giantRatPrefab;
    public Fog fogPrefab;
    public Tile tilePrefab;
    public List<Sprite> tileSprites;
    public List<Sprite> pencilTiles1 = new List<Sprite>();
    public Sprite[] fogSprites;
    public DamageText damageTextPrefab;
    public ItemObject itemObjectPrefab;
    public AbilityBase[] testAbility;
    public SkillBase[] testSkill;
    [SerializeField] BuffBase[] buffBases;
    public Dictionary<string, BuffBase> buffBaseDict = new();

    [SerializeField] Sprite blankImage;
    readonly Dictionary<SpriteAssetType, Dictionary<string, AsyncOperationHandle<Sprite>>> spriteHandles = new()
    {
        {SpriteAssetType.Ability, new Dictionary<string, AsyncOperationHandle<Sprite>>()},
        {SpriteAssetType.Equipment, new Dictionary<string, AsyncOperationHandle<Sprite>>()},
        {SpriteAssetType.Misc, new Dictionary<string, AsyncOperationHandle<Sprite>>()},
        {SpriteAssetType.Skill, new Dictionary<string, AsyncOperationHandle<Sprite>>()}
    };
    readonly Dictionary<string, AsyncOperationHandle<MiscBase>> miscBaseHandles = new();
    readonly Dictionary<string, AsyncOperationHandle<EquipmentBase>> equipmentBaseHandles = new();
    readonly Dictionary<PrefabAssetType, Dictionary<string, AsyncOperationHandle<GameObject>>> prefabHandles = new()
    {
        {PrefabAssetType.DungeonObject, new Dictionary<string, AsyncOperationHandle<GameObject>>()},
        {PrefabAssetType.Monster, new Dictionary<string, AsyncOperationHandle<GameObject>>()},
    };

    private static GameManager instance = null;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            for(int i=0; i<buffBases.Length; i++)
                buffBaseDict.Add(buffBases[i].key, buffBases[i]);

            saveData = new SaveData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static GameManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public void SaveGame()
    {
        SaveData.Save(saveData);
    }
    public void LoadGame()
    {
        saveData = SaveData.Load();
        if (saveData != null)
        {
            saveData.SetRand();
            saveData.UpdateFloorSeeds();
            saveData.SetMonstersLayout();
            dungeonManager.LoadFloor();
        }
        else
        {
            saveData = new SaveData();
            saveData.SetSeed();
            saveData.SetRand();
            saveData.SetMonstersLayout();
            dungeonManager.StartFirstFloor();
        }
    }

    public Sprite GetSprite(SpriteAssetType spriteAssetType, string key)
    {
        if (spriteHandles[spriteAssetType].ContainsKey(key))
            return spriteHandles[spriteAssetType][key].Result;
        else
        {
            spriteHandles[spriteAssetType].Add(key, Addressables.LoadAssetAsync<Sprite>("Assets/Sprites/" + spriteAssetType.ToString() + " Sprites/" + key + ".png"));
            spriteHandles[spriteAssetType][key].WaitForCompletion();
            if (spriteHandles[spriteAssetType][key].Status == AsyncOperationStatus.Succeeded)
                return spriteHandles[spriteAssetType][key].Result;
            else return blankImage;
        }
    }
    public MiscBase GetMiscBase(string key)
    {
        if (miscBaseHandles.ContainsKey(key))
            return miscBaseHandles[key].Result;
        else
        {
            miscBaseHandles.Add(key, Addressables.LoadAssetAsync<MiscBase>("Assets/Scriptable Objects/Misc/" + key + ".asset"));
            miscBaseHandles[key].WaitForCompletion();
            if (miscBaseHandles[key].Status == AsyncOperationStatus.Succeeded)
                return miscBaseHandles[key].Result;
            else return null;
        }
    }
    public EquipmentBase GetEquipmentBase(string key)
    {
        if (equipmentBaseHandles.ContainsKey(key))
            return equipmentBaseHandles[key].Result;
        else
        {
            equipmentBaseHandles.Add(key, Addressables.LoadAssetAsync<EquipmentBase>("Assets/Scriptable Objects/Equip/" + key + ".asset"));
            equipmentBaseHandles[key].WaitForCompletion();
            if (equipmentBaseHandles[key].Status == AsyncOperationStatus.Succeeded)
                return equipmentBaseHandles[key].Result;
            else return null;
        }
    }

    public GameObject GetPrefab(PrefabAssetType prefabAssetType, string key)
    {
        if (prefabHandles[prefabAssetType].ContainsKey(key))
            return prefabHandles[prefabAssetType][key].Result;
        else
        {
            prefabHandles[prefabAssetType].Add(key, Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/" + prefabAssetType.ToString() + "/" + key + ".prefab"));
            prefabHandles[prefabAssetType][key].WaitForCompletion();
            if (prefabHandles[prefabAssetType][key].Status == AsyncOperationStatus.Succeeded)
                return prefabHandles[prefabAssetType][key].Result;
            else return null;
        }
    }

    public void ReleaseAllAssets()
    {
        foreach(KeyValuePair<SpriteAssetType,Dictionary<string, AsyncOperationHandle<Sprite>>> pair in spriteHandles){
            foreach(KeyValuePair<string, AsyncOperationHandle<Sprite>> pair2 in pair.Value)
            {
                Addressables.Release(pair2.Value);
                pair.Value.Remove(pair2.Key);
            }
        }
        foreach (KeyValuePair<string, AsyncOperationHandle<MiscBase>> pair in miscBaseHandles)
        {
            Addressables.Release(pair.Value);
            miscBaseHandles.Remove(pair.Key);
        }
        foreach (KeyValuePair<string, AsyncOperationHandle<EquipmentBase>> pair in equipmentBaseHandles)
        {
            Addressables.Release(pair.Value);
            equipmentBaseHandles.Remove(pair.Key);
        }
        foreach (KeyValuePair<PrefabAssetType, Dictionary<string, AsyncOperationHandle<GameObject>>> pair in prefabHandles)
        {
            foreach (KeyValuePair<string, AsyncOperationHandle<GameObject>> pair2 in pair.Value)
            {
                Addressables.Release(pair2.Value);
                pair.Value.Remove(pair2.Key);
            }
        }
    }
}
