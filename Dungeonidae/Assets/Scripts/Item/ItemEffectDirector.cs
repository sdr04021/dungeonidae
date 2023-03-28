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
            case "RED_POTION":
                owner.RecoverHp(misc.EffectValues[0]);
                return true;
            default: 
                throw new NotImplementedException();
        }
    }
}
