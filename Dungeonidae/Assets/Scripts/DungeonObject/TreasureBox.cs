using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox : DungeonObject
{
    SpriteRenderer spriteRenderer;
    [SerializeField] Sprite openedSprite;
    [SerializeField] ItemType itemType;
    int seed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSeed(int seed)
    {
        this.seed = seed;
    }

    public override void Interact()
    {
        UnitData playerData = GameManager.Instance.saveData.playerData;
        for(int i=0; i<playerData.miscInventory.Count; i++)
        {
            if (playerData.miscInventory[i].Key=="BOX_KEY")
            {
                spriteRenderer.sprite = openedSprite;
            }
        }
    }
}
