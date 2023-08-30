using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDirector
{
    Unit owner;

    public BuffDirector(Unit owner)
    {
        this.owner = owner;
    }

    public void ApplyBuff(BuffData buff)
    {
        buff.durationLeft = buff.MaxDuration + 1;
        owner.UnitData.AddBuff(buff);
        int[] effectValues;
        switch (buff.Key)
        {
            case "MUCUS":
                effectValues = buff.BaseData.EffectValues.ToArray();
                owner.UnitData.SetStatValue(buff.Key, StatType.Speed, StatValueType.Temporary, -effectValues[0], true);
                break;
            case "POTION_HASTE":
                effectValues = GameManager.Instance.GetMiscBase(buff.Key).EffectValues;
                owner.UnitData.SetStatValue(buff.Key, StatType.Speed, StatValueType.Temporary, effectValues[1], true);
                break;
            case "SPRINT":
                effectValues = GameManager.Instance.GetSkillBase(buff.Key).EffectValues;
                owner.UnitData.SetStatValue(buff.Key, StatType.Speed, StatValueType.Temporary, effectValues[0], true);
                break;
            case "STUN":
                owner.AddParticleEffect(buff.Key, GameManager.Instance.GetPrefab(PrefabAssetType.ParticleEffect, buff.Key));
                break;
        }
    }

    public void RemoveBuff(BuffData buff)
    {
        owner.UnitData.RemoveBuff(buff);
        switch (buff.Key)
        {
            case "MUCUS":
                owner.UnitData.RemoveStatValue(buff.Key, StatType.Speed, StatValueType.Temporary);
                break;
            case "POTION_HASTE":
                owner.UnitData.RemoveStatValue(buff.Key, StatType.Speed, StatValueType.Temporary);
                break;
            case "SPRINT":
                owner.UnitData.RemoveStatValue(buff.Key, StatType.Speed, StatValueType.Temporary);
                break;
            case "STUN":
                break;
        }
    }
}
