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
            case "SPRINT":
                owner.UnitData.SetStatValue(buff.Key, StatType.Speed, StatValueType.Temporary, buff.EffectValues[1]);
                break;
        }
    }

    public void RemoveBuff(BuffData buff)
    {
        owner.UnitData.RemoveBuff(buff);
        switch (buff.Key)
        {
            case "SPRINT":
                owner.UnitData.RemoveStatValue(buff.Key, StatType.Speed, StatValueType.Temporary);
                break;
        }
    }
}