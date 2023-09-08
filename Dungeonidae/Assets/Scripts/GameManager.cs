using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Localization;

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
    [field: SerializeField] public AnimationEffect AnimationEffectPrefab { get; private set; }

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
        {PrefabAssetType.ParticleEffect, new Dictionary<string, AsyncOperationHandle<GameObject>>()}
    };
    readonly Dictionary<string, AsyncOperationHandle<BuffBase>> buffBaseHandles = new();
    readonly Dictionary<string, AsyncOperationHandle<SkillBase>> skillBaseHandles = new();
    readonly Dictionary<string, AsyncOperationHandle<AbilityBase>> abilityBaseHandles = new();


    readonly LocalizedStringTable tableStatusText = new("Status Text");
    readonly LocalizedStringTable tableEquipment = new("Equipment Text");
    readonly LocalizedStringTable tableMiscItem = new("Misc Item Text");
    readonly LocalizedStringTable tableAbility = new("Ability");
    readonly LocalizedStringTable tableSkill = new("Skill Text");


    private static GameManager instance = null;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

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

    public BuffBase GetBuffBase(string key)
    {
        if (buffBaseHandles.ContainsKey(key))
            return buffBaseHandles[key].Result;
        else
        {
            buffBaseHandles.Add(key, Addressables.LoadAssetAsync<BuffBase>("Assets/Scriptable Objects/Buff/" + key + ".asset"));
            buffBaseHandles[key].WaitForCompletion();
            if (buffBaseHandles[key].Status == AsyncOperationStatus.Succeeded)
                return buffBaseHandles[key].Result;
            else return null;
        }
    }
    public SkillBase GetSkillBase(string key)
    {
        if (skillBaseHandles.ContainsKey(key))
            return skillBaseHandles[key].Result;
        else
        {
            skillBaseHandles.Add(key, Addressables.LoadAssetAsync<SkillBase>("Assets/Scriptable Objects/Skill/" + key + ".asset"));
            skillBaseHandles[key].WaitForCompletion();
            if (skillBaseHandles[key].Status == AsyncOperationStatus.Succeeded)
                return skillBaseHandles[key].Result;
            else return null;
        }
    }
    public AbilityBase GetAbilityBase(string key)
    {
        if (abilityBaseHandles.ContainsKey(key))
            return abilityBaseHandles[key].Result;
        else
        {
            abilityBaseHandles.Add(key, Addressables.LoadAssetAsync<AbilityBase>("Assets/Scriptable Objects/Ability/" + key + ".asset"));
            abilityBaseHandles[key].WaitForCompletion();
            if (abilityBaseHandles[key].Status == AsyncOperationStatus.Succeeded)
                return abilityBaseHandles[key].Result;
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
        foreach (KeyValuePair<string, AsyncOperationHandle<BuffBase>> pair in buffBaseHandles)
        {
            Addressables.Release(pair.Value);
            buffBaseHandles.Remove(pair.Key);
        }
        foreach (KeyValuePair<string, AsyncOperationHandle<SkillBase>> pair in skillBaseHandles)
        {
            Addressables.Release(pair.Value);
            skillBaseHandles.Remove(pair.Key);
        }
        foreach (KeyValuePair<string, AsyncOperationHandle<AbilityBase>> pair in abilityBaseHandles)
        {
            Addressables.Release(pair.Value);
            abilityBaseHandles.Remove(pair.Key);
        }
    }

    public string StatTypeToString(StatType stat)
    {
        return stat switch
        {
            StatType.Atk => tableStatusText.GetTable().GetEntry("ATTACK").GetLocalizedString(),
            StatType.MAtk => tableStatusText.GetTable().GetEntry("MAGIC_ATTACK").GetLocalizedString(),
            StatType.AtkRange => tableStatusText.GetTable().GetEntry("ATTACK_RANGE").GetLocalizedString(),
            StatType.Pen => tableStatusText.GetTable().GetEntry("ARMOR_PENETRATION").GetLocalizedString(),
            StatType.MPen => tableStatusText.GetTable().GetEntry("MAGIC_PENETRATION").GetLocalizedString(),
            StatType.Acc => tableStatusText.GetTable().GetEntry("ACCURACY").GetLocalizedString(),
            StatType.Aspd => tableStatusText.GetTable().GetEntry("ATTACK_SPEED").GetLocalizedString(),
            StatType.Cri => tableStatusText.GetTable().GetEntry("CRITICAL_RATE").GetLocalizedString(),
            StatType.CriDmg => tableStatusText.GetTable().GetEntry("CRITICAL_DAMAGE").GetLocalizedString(),
            StatType.Proficiency => tableStatusText.GetTable().GetEntry("PROFICIENCY").GetLocalizedString(),
            StatType.LifeSteal => tableStatusText.GetTable().GetEntry("LIFE_STEAL").GetLocalizedString(),
            StatType.ManaSteal => tableStatusText.GetTable().GetEntry("MANA_STEAL").GetLocalizedString(),
            StatType.DmgIncrease => tableStatusText.GetTable().GetEntry("DAMAGE_INCREASE").GetLocalizedString(),
            StatType.MaxHp => "HP",
            StatType.MaxMp => "MP",
            StatType.MaxHunger => tableStatusText.GetTable().GetEntry("HUNGER").GetLocalizedString(),
            StatType.Def => tableStatusText.GetTable().GetEntry("DEFENSE").GetLocalizedString(),
            StatType.MDef => tableStatusText.GetTable().GetEntry("MAGIC_DEFENSE").GetLocalizedString(),
            StatType.Eva => tableStatusText.GetTable().GetEntry("EVASION").GetLocalizedString(),
            StatType.Tolerance => tableStatusText.GetTable().GetEntry("TOLERANCE").GetLocalizedString(),
            StatType.Resist => tableStatusText.GetTable().GetEntry("RESIST").GetLocalizedString(),
            StatType.DmgReduction => tableStatusText.GetTable().GetEntry("DAMAGE_REDUCTION").GetLocalizedString(),
            StatType.Sight => tableStatusText.GetTable().GetEntry("SIGHT").GetLocalizedString(),
            StatType.Instinct => tableStatusText.GetTable().GetEntry("INSTINCT").GetLocalizedString(),
            StatType.SearchRange => tableStatusText.GetTable().GetEntry("SEARCH_RANGE").GetLocalizedString(),
            StatType.HpRegen => tableStatusText.GetTable().GetEntry("HEALTH_REGENERATION").GetLocalizedString(),
            StatType.MpRegen => tableStatusText.GetTable().GetEntry("MANA_REGENERATION").GetLocalizedString(),
            StatType.Speed => tableStatusText.GetTable().GetEntry("MOVE_SPEED").GetLocalizedString(),
            StatType.Stealth => tableStatusText.GetTable().GetEntry("STEALTH").GetLocalizedString(),
            _ => null,
        };
    }

    public string GetEquipmentName(string key)
    {
        return tableEquipment.GetTable().GetEntry(key + "_NAME").GetLocalizedString();
    }
    public string GetEquipmentAbilitiy(string key)
    {
        return tableEquipment.GetTable().GetEntry("ABILITY_" + key).GetLocalizedString();
    }
    public string GetEquipmentAbilitiy(string key, List<int> values)
    {
        return tableEquipment.GetTable().GetEntry("ABILITY_" + key).GetLocalizedString(values);
    }
    public string GetItemName(string key)
    {
        return tableMiscItem.GetTable().GetEntry(key + "_NAME").GetLocalizedString();
    }

    public string GetItemDescription(string key, int[] values)
    {
        if (values.Length > 0)
            return tableMiscItem.GetTable().GetEntry(key + "_DESC").GetLocalizedString(values);
        else
            return tableMiscItem.GetTable().GetEntry(key + "_DESC").GetLocalizedString();
    }

    public string GetAbilityName(string key)
    {
        return tableAbility.GetTable().GetEntry(key + "_NAME").GetLocalizedString();
    }
    public string GetAbilityDescription(string key)
    {
        return tableAbility.GetTable().GetEntry(key + "_DESC").GetLocalizedString();
    }
    public string GetAbilityEffect(string key, int[] values)
    {
        return tableAbility.GetTable().GetEntry(key + "_EFFECT").GetLocalizedString(values);
    }
    public string GetSkillName(string key)
    {
        return tableSkill.GetTable().GetEntry(key + "_NAME").GetLocalizedString();
    }
    public string GetSkillDescription(string key, int[] values)
    {
        return tableSkill.GetTable().GetEntry(key + "_DESC").GetLocalizedString(values);
    }
}
