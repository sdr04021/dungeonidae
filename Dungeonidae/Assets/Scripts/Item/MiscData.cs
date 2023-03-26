using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscData : ItemData
{
    int[] effectValues;
    public int[] EffectValues { get => effectValues; }    

    int maxStack;
    public int MaxStack { get => maxStack; }

    int amount;
    public int Amount { get => amount; }

    public int AmountLeft { get => (maxStack - Amount); }

    MiscBase miscBase;
    public MiscBase MiscBase { get => miscBase; }

    public MiscData(MiscBase misc, int amount) : base(misc)
    {
        miscBase = misc;
        effectValues = misc.EffectValues;
        maxStack = misc.MaxStack;
        this.amount = amount;
    }

    public void AddAmount(int amount)
    {
        this.amount += amount;
    }
}
