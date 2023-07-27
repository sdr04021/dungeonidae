using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartOfHealing : DungeonObject
{
    public override void Init(string key, DungeonManager dm, Coordinate coord)
    {
        base.Init(key, dm, coord);
        transform.DOMoveY(transform.position.y + 0.1f, 0.6f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    public override void Activate(Unit unit)
    {
        if (unit.UnitData.team == Team.Player)
        {
            if (unit.MySpriteRenderer.enabled)
                Instantiate(GameManager.Instance.GetPrefab(PrefabAssetType.ParticleEffect, "HEALING"), unit.transform);
            unit.RecoverHp((int)(unit.UnitData.maxHp.Total() * 0.3f));
            dm.map.GetElementAt(DungeonObjectData.coord).dungeonObjects.Remove(this);
            dm.RemoveDungeonObjectData(DungeonObjectData);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
