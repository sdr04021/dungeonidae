using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DungeonManager dungeonManager;
    public Player warriorPrefab;
    public Monster testMob1Prefab;
    public GameObject fogPrefab;
    public Tile tilePrefab;
    public List<Sprite> tileSprites;
    public List<Sprite> pencilTiles1 = new List<Sprite>();
    public Sprite[] fogSprites;
    public DamageText damageTextPrefab;
    public ItemObject itemObjectPrefab;
    public MiscBase testItem;
    public EquipmentBase testEquip;
    public AbilityBase[] testAbility;
    public SkillBase[] testSkill;
    [SerializeField] BuffBase[] buffBases;
    public Dictionary<string, BuffBase> buffBaseDict = new();

    public int temp = 0;

    private static GameManager instance = null;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            for(int i=0; i<buffBases.Length; i++)
                buffBaseDict.Add(buffBases[i].key, buffBases[i]);
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
}
