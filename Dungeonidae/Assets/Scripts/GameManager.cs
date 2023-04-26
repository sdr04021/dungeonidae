using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Stair stairDownPrefab;
    public Stair stairUpPrefab;
    public MiscBase testItem;
    public EquipmentBase testEquip;
    public AbilityBase[] testAbility;
    public SkillBase[] testSkill;
    [SerializeField] BuffBase[] buffBases;
    public Dictionary<string, BuffBase> buffBaseDict = new();

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
}
