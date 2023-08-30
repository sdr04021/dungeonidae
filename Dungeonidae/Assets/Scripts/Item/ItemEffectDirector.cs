using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectDirector
{
    Unit owner;

    public ItemEffectDirector(Unit owner) 
    { 
        this.owner = owner;
    }

    public bool ItemEffect(MiscData misc)
    {
        switch (misc.Key)
        {
            case "POTION_HASTE":
                owner.BuffDirector.ApplyBuff(new BuffData(misc.Key, GameManager.Instance.GetMiscBase(misc.Key).EffectValues[0]));
                return true;
            case "POTION_HEAL":
                owner.RecoverHp(GameManager.Instance.GetMiscBase(misc.Key).EffectValues[0]);
                return true;
            case "SCROLL_TELEPORT":
                owner.Teleportation();
                return true;
            default: 
                throw new System.NotImplementedException();
        }
    }
    public bool ThrownEffect(Unit target, MiscData misc)
    {
        switch (misc.Key)
        {
            case "POTION_HASTE":
                target.BuffDirector.ApplyBuff(new BuffData(misc.Key, GameManager.Instance.GetMiscBase(misc.Key).EffectValues[0]));
                return true;
            case "POTION_HEAL":
                target.RecoverHp(GameManager.Instance.GetMiscBase(misc.Key).EffectValues[0]);
                return true;
            case "SMALL_STONE":
                AttackData attackData = new(owner, 10, 0, 0);
                target.GetDamage(attackData);
                if (Random.value <= 0.1f)
                    return true;
                else return false;
            default:
                return false;
        }
    }
}
