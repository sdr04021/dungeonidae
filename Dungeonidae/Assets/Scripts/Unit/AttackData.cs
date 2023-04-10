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
        AttackDamage = (int)(attackDamage * (Random.Range(attacker.UnitData.proficiency.Total() - 1, 100) + 1) * 0.01f);
        MagicAttackDamage = (int)(magicAttackDamage * (Random.Range(attacker.UnitData.proficiency.Total() - 1, 100) + 1) * 0.01f);
        FixedDamage = fixedDamage;
    }
}
