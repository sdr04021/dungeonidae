using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackData
{
    public Unit Attacker { get; private set; }

    public AttackType Type { get; private set; }
    public int Damage { get; private set; }

    public AttackData(Unit attacker, AttackType type, int damage)
    {
        Attacker = attacker;
        Type = type;
        Damage = damage;
    }
}
