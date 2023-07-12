using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackData
{
    public Unit Attacker { get; private set; }
    public int AttackDamage { get; private set; }
    public int MagicAttackDamage { get; private set; }
    public int FixedDamage { get; private set; }
    public bool IsCritical { get; private set; }

    public AttackData(Unit attacker, int attackDamage, int magicAttackDamage, int fixedDamage, bool isCritical = false)
    {
        Attacker = attacker;
        IsCritical = isCritical;
        AttackDamage = (int)(attackDamage * (Random.Range(attacker.UnitData.proficiency.Total() - 1, 100) + 1) * 0.01f);
        MagicAttackDamage = (int)(magicAttackDamage * (Random.Range(attacker.UnitData.proficiency.Total() - 1, 100) + 1) * 0.01f);
        if (IsCritical || ((Random.Range(0, 100) + 1) <= attacker.UnitData.cri.Total()))
        {
            IsCritical = true;
            AttackDamage = (int)(AttackDamage * (attacker.UnitData.criDmg.Total() * 0.01f));
            MagicAttackDamage = (int)(MagicAttackDamage * (attacker.UnitData.criDmg.Total() * 0.01f));
        }
        AttackDamage += (int)(AttackDamage * (attacker.UnitData.dmgIncrease.Total() * 0.01f));
        MagicAttackDamage += (int)(MagicAttackDamage * (attacker.UnitData.dmgIncrease.Total() * 0.01f));

        FixedDamage = fixedDamage;
    }
}
