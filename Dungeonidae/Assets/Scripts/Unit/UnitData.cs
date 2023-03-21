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
    public int Level { get { return level; } }
    int exp;
    public int Exp { get { return exp; } }
    int maxExp;
    public int MaxExp { get { return maxExp; } }

    public UnitStat maxHp;
    int hp;
    public int Hp { 
        get => hp;
        set
        {
            hp = value >= 0 ? value <= maxHp.Total()? value : maxHp.Total() : 0;
        }
    }
    public UnitStat hpRegen;
    public UnitStat maxMp;
    int mp;
    public int Mp
    {
        get => mp;
        set
        {
            mp = value >= 0 ? value <= maxMp.Total() ? value : maxMp.Total() : 0;
        }
    }
    public UnitStat mpRegen;

    public UnitStat atk;
    public UnitStat mAtk;
    public UnitStat atkRange;

    public UnitStat pen;
    public UnitStat mPen;
    public UnitStat acc;
    public UnitStat aspd;
    public UnitStat cri;
    public UnitStat criDmg;
    public UnitStat proficiency;
    public UnitStat lifeSteal;
    public UnitStat manaSteal;

    public UnitStat def;
    public UnitStat mDef;
    public UnitStat eva;
    public UnitStat block;
    public UnitStat resist;

    public UnitStat dmgIncrease;
    public UnitStat dmgReduction;

    public UnitStat speed;
    public UnitStat sight;
    public UnitStat instinct;
    public UnitStat searchRange;

    public UnitStat maxHunger;
    int hunger;
    public int Hunger
    {
        get => hunger;
        set
        {
            hunger = value >= 0 ? value <= maxHunger.Total() ? value : maxHunger.Total() : 0;
        }
    }

    public UnitData(BaseStats baseStats)
    {
        unitName = baseStats.UnitName;
        team = baseStats.Team;
        level = baseStats.Level;
        maxExp = baseStats.MaxExp;
        exp = 0;
        maxHp = new(baseStats.MaxHp);
        hp = maxHp.Original;
        hpRegen = new(baseStats.HpRegen);
        maxMp = new(baseStats.MaxMp);
        mp = maxMp.Original;
        mpRegen = new(baseStats.MpRegen);
        atk = new(baseStats.Atk);
        mAtk = new(baseStats.MAtk);
        atkRange = new(baseStats.AtkRange);
        pen = new(baseStats.Pen);
        mPen = new(baseStats.MPen);
        acc = new(baseStats.Acc);
        aspd = new(baseStats.Aspd);
        cri = new(baseStats.Cri);
        criDmg = new(baseStats.CriDmg);
        proficiency = new(baseStats.Proficiency);
        lifeSteal = new(baseStats.LifeSteal);
        manaSteal  = new(baseStats.ManaSteal);
        def = new(baseStats.Def);
        mDef = new(baseStats.MDef);
        eva = new(baseStats.Eva);
        block = new(baseStats.Block);
        resist = new(baseStats.Resist);
        dmgIncrease = new(baseStats.DmgIncrease);
        dmgReduction = new(baseStats.DmgReduction);
        speed = new(baseStats.Speed);
        sight = new(baseStats.Sight);
        instinct = new(baseStats.Instinct);
        searchRange = new(baseStats.SearchRange);
        maxHunger = new(baseStats.MaxHunger);
    }
}
