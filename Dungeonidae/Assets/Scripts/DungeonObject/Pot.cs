using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : DungeonObject
{   
    public override void TargetedInteraction(Unit unit)
    {
        dm.RemoveDungeonObjectData(DungeonObjectData);
        dm.map.GetElementAt(DungeonObjectData.coord).dungeonObjects.Remove(this);
        dm.InstantiateDungeonObject("HEART_OF_HEALING",DungeonObjectData.coord);
        SpriteRenderer.DOFade(0, 1).OnComplete(() => {
            Destroy(gameObject); 
        });
    }
}
