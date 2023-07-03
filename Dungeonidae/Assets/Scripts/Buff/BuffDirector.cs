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
        switch (buff.Key)
        {
            case "POTION_HASTE":
                int[] effectValues = GameManager.Instance.GetMiscBase(buff.Key).EffectValues;
                owner.UnitData.SetStatValue(buff.Key, StatType.Speed, StatValueType.Temporary, effectValues[1], true);
                break;
            case "SPRINT":
                //owner.UnitData.SetStatValue(buff.Key, StatType.Speed, StatValueType.Temporary, buff.EffectValues[1], true);
                break;
        }
    }

    public void RemoveBuff(BuffData buff)
    {
        owner.UnitData.RemoveBuff(buff);
        switch (buff.Key)
        {
            case "POTION_HASTE":
                owner.UnitData.RemoveStatValue(buff.Key, StatType.Speed, StatValueType.Temporary);
                break;
            case "SPRINT":
                //owner.UnitData.RemoveStatValue(buff.Key, StatType.Speed, StatValueType.Temporary);
                break;
        }
    }
}
