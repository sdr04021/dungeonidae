using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Base Stats", menuName = "Scriptable Object/Base Stats")]
public class BaseStats : ScriptableObject
{   
    [SerializeField]
    string unitName;
    public string UnitName { get => unitName; }

    [SerializeField]
    Team team;
    public Team Team { get => team; }

    [SerializeField]
    int level;
    public int Level { get => level; }

    [SerializeField]
    int maxExp;
    public int MaxExp { get => maxExp; }

    [SerializeField]
    int maxHp;
    public int MaxHp { get => maxHp; }

    [SerializeField]
    int hpRegen;
    public int HpRegen { get => hpRegen; }

    [SerializeField]
    int maxMp;
    public int MaxMp { get => maxMp; }

    [SerializeField]
    int mpRegen;
    public int MpRegen { get => mpRegen; }

    [SerializeField]
    int atk;
    public int Atk { get => atk; }

    [SerializeField]
    int mAtk;
    public int MAtk { get => mAtk; }

    [SerializeField]
    int atkRange;
    public int AtkRange { get => atkRange; }

    [SerializeField]
    int pen;
    public int Pen { get => pen; }

    [SerializeField]
    int mPen;
    public int MPen { get => mPen; }

    [SerializeField]
    int acc;
    public int Acc { get => acc; }

    [SerializeField]
    int aspd;
    public int Aspd { get => aspd; }

    [SerializeField]
    int cri;
    public int Cri { get => cri; }

    [SerializeField]
    int criDmg;
    public int CriDmg { get => criDmg; }

    [SerializeField]
    int proficiency;
    public int Proficiency { get => proficiency; }

    [SerializeField]
    int lifeSteal;
    public int LifeSteal { get => lifeSteal; }

    [SerializeField]
    int manaSteal;
    public int ManaSteal { get => manaSteal; }

    [SerializeField]
    int def;
    public int Def { get => def; }

    [SerializeField]
    int mDef;
    public int MDef { get => mDef; }

    [SerializeField]
    int eva;
    public int Eva { get => eva; }

    [SerializeField]
    int block;
    public int Block { get => block; }

    [SerializeField]
    int resist;
    public int Resist { get => resist; }

    [SerializeField]
    int dmgIncrease;
    public int DmgIncrease { get => dmgIncrease; }

    [SerializeField]
    int dmgReduction;
    public int DmgReduction { get => dmgReduction; }

    [SerializeField]
    int speed;
    public int Speed { get => speed; }

    [SerializeField]
    int sight;
    public int Sight { get => sight; }

    [SerializeField]
    int instinct;
    public int Instinct { get => instinct; }

    [SerializeField]
    int searchRange;
    public int SearchRange { get => searchRange; }

    [SerializeField]
    int maxHunger;
    public int MaxHunger { get => maxHunger; }
}
