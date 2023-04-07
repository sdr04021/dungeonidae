using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitBase", menuName = "Scriptable Object/UnitBase")]
public class UnitBase : ScriptableObject
{
    public string key;
    public Team team;
    public int level;
    public int[] expTable;
    public int[] expReward;

    [Header("Stats")]
    public int maxHp;
    public int hpRegen;
    public int maxMp;
    public int mpRegen;
    public int maxHunger;
    public int atk;
    public int mAtk;
    public int atkRange;
    public int pen;
    public int mPen;
    public int acc;
    public int aspd;
    public int cri;
    public int criDmg;
    public int proficiency;
    public int lifeSteal;
    public int manaSteal;
    public int def;
    public int mDef;
    public int eva;
    public int block;
    public int resist;
    public int dmgIncrease;
    public int dmgReduction;
    public int speed;
    public int sight;
    public int instinct;
    public int searchRange;

    [Header("Growth Table")]
    public int maxHpGrowth;
    public int maxMpGrowth;
    public int atkGrowth;
    public int mAtkGrowth;
    public int defGrowth;
    public int mDefGrowth;
}
