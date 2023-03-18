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
    public Sprite[] fogSprites;
    public DamageText damageTextPrefab;

    private static GameManager instance = null;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
