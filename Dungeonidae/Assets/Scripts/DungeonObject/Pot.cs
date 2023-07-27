using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : DungeonObject
{
    public override void Activate(Unit unit)
    {
        dm.RemoveDungeonObjectData(DungeonObjectData);
        dm.map.GetElementAt(DungeonObjectData.coord).dungeonObjects.Remove(this);

        int pick = Random.Range(0, 5);
        if (pick == 0)
            dm.InstantiateDungeonObject("HEART_OF_HEALING", DungeonObjectData.coord);
        SpriteRenderer.DOColor(Color.gray, 0.1f).OnComplete(() =>
        {
            Instantiate(GameManager.Instance.GetPrefab(PrefabAssetType.ParticleEffect, "POT"), transform.position, Quaternion.identity);
            Destroy(gameObject);
        });
        /*
    SpriteRenderer.DOFade(0, 1).OnComplete(() => {
        Destroy(gameObject); 
    });
        */
    }
}
