using System;
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
            case "POTION_HEAL":
                owner.RecoverHp(GameManager.Instance.GetMiscBase(misc.Key).EffectValues[0]);
                return true;
            case "SCROLL_TELEPORT":
                owner.Teleportation();
                return true;
            default: 
                throw new NotImplementedException();
        }
    }
}
