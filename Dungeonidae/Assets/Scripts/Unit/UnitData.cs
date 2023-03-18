using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData
{
    string unitName;
    public string UnitName { get { return unitName; } }
    Team team;
    public Team Team { get { return team; } }

    int level;
    int exp;
    int maxExp;

    public UnitStat maxHp;
    int hp;
    public int Hp { 
        get => hp;
        set
        {
            hp = value >= 0 ? value <= maxHp.Total()? value : maxHp.Total() : 0;
        }
    }
    int hpRegen;
    int maxMp;
    int mp;
    int mpRegen;

    public UnitStat atk;
    public UnitStat mAtk;
    public UnitStat atkRange;

    int pen;
    int acc;
    public UnitStat aspd;
    int cri;
    int criDmg;

    public UnitStat def;
    int eva;
    int block;

    int dmgIncrease;
    int dmgReduction;

    public UnitStat speed;
    public UnitStat sight;

    int maxHunger;
    int hunger;

    public UnitData(BaseStats baseStats)
    {
        unitName = baseStats.UnitName;
        team = baseStats.Team;
        maxHp = new(baseStats.MaxHp);
        hp = maxHp.Original;
        atk = new(baseStats.Atk);
        atkRange = new(baseStats.AtkRange);
        aspd = new(baseStats.Aspd);
        def = new(baseStats.Def);
        speed = new(baseStats.Speed);
        sight = new(baseStats.Sight);
    }
}
