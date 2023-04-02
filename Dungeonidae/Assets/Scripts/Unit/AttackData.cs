using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackData
{
    public Unit Attacker { get; private set; }
    public int AttackDamage { get; private set; }
    public int MagicAttackDamage { get; private set; }

    public int FixedDamage { get; private set; }    

    public AttackData(Unit attacker, int attackDamage, int magicAttackDamage, int fixedDamage)
    {
        Attacker = attacker;
        AttackDamage = attackDamage;
        MagicAttackDamage = magicAttackDamage;
        FixedDamage = fixedDamage;
    }
}
