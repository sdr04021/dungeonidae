using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitData
{
    public delegate void EventHandler();
    public event EventHandler OnHpValueChanged;
    public event EventHandler OnMpValueChanged;
    public event EventHandler OnExpValueChanged;
    public event EventHandler OnSkillChanged;
    public event EventHandler OnSkillCurrentCooldownChanged;
    public event EventHandler OnLevelChanged;
    public event EventHandler OnBuffListChanged;
    public event EventHandler OnBuffDurationChanged;

    [SerializeField] string unitName;
    public string UnitName { get => unitName; }

    [SerializeField] Team team;
    public Team Team { get => team; }

    [SerializeField] int level;
    public int Level { get => level; }

    [SerializeField] int exp;
    public int Exp { get => exp; }

    [SerializeField] int maxExp;
    public int MaxExp { get => maxExp; }

    [SerializeField] int[] expTable;
    public int[] ExpTable { get => expTable; }

    [SerializeField] int[] expReward;
    public int[] ExpReward { get => expReward; }

    [SerializeField] UnitStat maxHp;
    public UnitStat MaxHp { get => maxHp; }

    [SerializeField] int hp;
    public int Hp { 
        get => hp;
        set
        {
            hp = value >= 0 ? value <= maxHp.Total()? value : maxHp.Total() : 0;
            OnHpValueChanged?.Invoke();
        }
    }

    [SerializeField] UnitStat hpRegen;
    public UnitStat HpRegen { get => hpRegen; }

    [SerializeField] public UnitStat maxMp;
    public UnitStat MaxMp { get => maxMp; }

    [SerializeField] int mp;
    public int Mp
    {
        get => mp;
        set
        {
            mp = value >= 0 ? value <= maxMp.Total() ? value : maxMp.Total() : 0;
            OnMpValueChanged?.Invoke();
        }
    }

    [SerializeField] UnitStat mpRegen;
    public UnitStat MpRegen { get => mpRegen; }

    [SerializeField] UnitStat atk;
    public UnitStat Atk { get => atk; }

    [SerializeField] UnitStat mAtk;
    public UnitStat MAtk { get => mAtk; }

    [SerializeField] UnitStat atkRange;
    public UnitStat AtkRange { get => atkRange; }

    [SerializeField] UnitStat pen;
    public UnitStat Pen { get => pen;}

    [SerializeField] UnitStat mPen;
    public UnitStat MPen { get => mPen; }

    [SerializeField] UnitStat acc;
    public UnitStat Acc { get => acc; }

    [SerializeField] UnitStat aspd;
    public UnitStat Aspd { get => aspd; }

    [SerializeField] UnitStat cri;
    public UnitStat Cri { get => cri; }

    [SerializeField] UnitStat criDmg;
    public UnitStat CriDmg { get => criDmg; }

    [SerializeField] UnitStat proficiency;
    public UnitStat Proficiency { get => proficiency; }

    [SerializeField] UnitStat lifeSteal;
    public UnitStat LifeSteal { get => lifeSteal; }

    [SerializeField] UnitStat manaSteal;
    public UnitStat ManaSteal { get => manaSteal; }

    [SerializeField] UnitStat def;
    public UnitStat Def { get => def; }

    [SerializeField] UnitStat mDef;
    public UnitStat MDef { get => mDef; }

    [SerializeField] UnitStat eva;
    public UnitStat Eva { get => eva; }

    [SerializeField] UnitStat block;
    public UnitStat Block { get => block; } 

    [SerializeField] UnitStat resist;
    public UnitStat Resist { get => resist; }

    [SerializeField] UnitStat dmgIncrease;
    public UnitStat DmgIncrease { get => dmgIncrease;}

    [SerializeField] UnitStat dmgReduction;
    public UnitStat DmgReduction { get => dmgReduction;}

    [SerializeField] UnitStat speed;
    public UnitStat Speed { get => speed; }

    [SerializeField] UnitStat sight;
    public UnitStat Sight {  get => sight; }

    [SerializeField] UnitStat instinct;
    public UnitStat Instinct { get => instinct; }

    [SerializeField] UnitStat searchRange;
    public UnitStat SearchRange { get => searchRange; }

    [SerializeField] UnitStat maxHunger;
    public UnitStat MaxHunger { get => maxHunger; }

    [SerializeField] int hunger;
    public int Hunger
    {
        get => hunger;
        set
        {
            hunger = value >= 0 ? value <= maxHunger.Total() ? value : maxHunger.Total() : 0;
        }
    }

    int maxHpGrowth;
    int maxMpGrowth;
    int atkGrowth;
    int mAtkGrowth;
    int defGrowth;
    int mDefGrowth;

    [field:SerializeField] public SkillData[] Skills { get; private set; } = new SkillData[5];
    public List<SkillData> learnedSkills = new();

    [field:SerializeField] public List<BuffData> Buffs { get; private set; } = new();

    public UnitData(UnitBase unitBase)
    {
        unitName = unitBase.key;
        team = unitBase.team;
        level = unitBase.level;
        maxExp = unitBase.expTable[0];
        exp = 0;
        expTable = unitBase.expTable;
        expReward = unitBase.expReward;

        maxHp = new(unitBase.maxHp);
        hp = maxHp.original;
        hpRegen = new(unitBase.hpRegen);
        maxMp = new(unitBase.maxMp);
        mp = maxMp.original;
        mpRegen = new(unitBase.mpRegen);
        atk = new(unitBase.atk);
        mAtk = new(unitBase.mAtk);
        atkRange = new(unitBase.atkRange);
        pen = new(unitBase.pen);
        mPen = new(unitBase.mPen);
        acc = new(unitBase.acc);
        aspd = new(unitBase.aspd);
        cri = new(unitBase.cri);
        criDmg = new(unitBase.criDmg);
        proficiency = new(unitBase.proficiency);
        lifeSteal = new(unitBase.lifeSteal);
        manaSteal  = new(unitBase.manaSteal);
        def = new(unitBase.def);
        mDef = new(unitBase.mDef);
        eva = new(unitBase.eva);
        block = new(unitBase.block);
        resist = new(unitBase.resist);
        dmgIncrease = new(unitBase.dmgIncrease);
        dmgReduction = new(unitBase.dmgReduction);
        speed = new(unitBase.speed);
        sight = new(unitBase.sight);
        instinct = new(unitBase.instinct);
        searchRange = new(unitBase.searchRange);
        maxHunger = new(unitBase.maxHunger);

        maxHpGrowth = unitBase.maxHpGrowth;
        maxMpGrowth = unitBase.maxMpGrowth;
        atkGrowth = unitBase.atkGrowth;
        mAtkGrowth = unitBase.mAtkGrowth;
        defGrowth = unitBase.mDefGrowth;
        mDefGrowth = unitBase.mDefGrowth;
    }

    public void IncreaseExpValue(int amount)
    {
        exp += amount;
        while (exp >= maxExp)
        {
            exp -= maxExp;
            IncreaseLevelValue();
        }
        OnExpValueChanged?.Invoke();
    }
    public void IncreaseLevelValue()
    {
        level++;
        GrowStats();
        OnLevelChanged?.Invoke();
    }
    void GrowStats()
    {
        maxHp.original += maxHpGrowth;
        maxMp.original += maxMpGrowth;
        atk.original += atkGrowth;
        mAtk.original += mAtkGrowth;
        def.original += defGrowth;
        mDef.original += mDefGrowth;
    }

    public void SetStatValue(string key, StatType statType, StatValueType statValueType, int amount)
    {
        switch (statValueType)
        {
            case StatValueType.Original:
                throw new NotImplementedException();
            case StatValueType.Additional:
                if (StatTypeToUnitStat(statType).additional.ContainsKey(key))
                {
                    StatTypeToUnitStat(statType).additional[key] = amount;
                }
                else
                    StatTypeToUnitStat(statType).additional.Add(key, amount);
                break;
            case StatValueType.Temporary:
                if (StatTypeToUnitStat(statType).temporary.ContainsKey(key))
                {
                    StatTypeToUnitStat(statType).temporary[key] = amount;
                }
                else
                    StatTypeToUnitStat(statType).temporary.Add(key, amount);
                break;
            case StatValueType.Percent:
                if (StatTypeToUnitStat(statType).percent.ContainsKey(key))
                {
                    StatTypeToUnitStat(statType).percent[key] = amount;
                }
                else
                    StatTypeToUnitStat(statType).percent.Add(key, amount);
                break;
        }
        InvokeStatChangedEvent(statType);
    }
    public void RemoveStatValue(string key, StatType statType, StatValueType statValueType)
    {
        switch (statValueType)
        {
            case StatValueType.Original:
                throw new NotImplementedException();
            case StatValueType.Additional:
                if (StatTypeToUnitStat(statType).additional.ContainsKey(key))
                {
                    StatTypeToUnitStat(statType).additional.Remove(key);
                }
                break;
            case StatValueType.Temporary:
                if (StatTypeToUnitStat(statType).temporary.ContainsKey(key))
                {
                    StatTypeToUnitStat(statType).temporary.Remove(key);
                }
                break;
            case StatValueType.Percent:
                if (StatTypeToUnitStat(statType).percent.ContainsKey(key))
                {
                    StatTypeToUnitStat(statType).percent.Remove(key);
                }
                break;
        }
        InvokeStatChangedEvent(statType);
    }

    public void InvokeStatChangedEvent(StatType statType)
    {
        switch (statType)
        {
            case StatType.MaxHp:
                OnHpValueChanged.Invoke();
                break;
            case StatType.MaxMp:
                OnHpValueChanged.Invoke();
                break;
        }
    }

    public void ApplyEquipStats(EquipmentData equip)
    {
        foreach(var pair in equip.Stats)
        {
            SetStatValue(equip.Key, pair.Key, StatValueType.Additional, pair.Value);
        }
    }
    public void RemoveEquipStats(EquipmentData equip)
    {
        foreach (var pair in equip.Stats)
        {
            RemoveStatValue(equip.Key, pair.Key, StatValueType.Additional);
        }
    }

    private ref UnitStat StatTypeToUnitStat(StatType statType)
    {
        switch (statType)
        {
            case StatType.Atk: return ref atk;
            case StatType.MAtk: return ref mAtk;
            case StatType.AtkRange: return ref atkRange;
            case StatType.Pen: return ref pen;
            case StatType.MPen: return ref mPen;
            case StatType.Acc: return ref acc;
            case StatType.Aspd: return ref aspd;
            case StatType.Cri: return ref cri;
            case StatType.CriDmg: return ref criDmg;
            case StatType.Proficiency: return ref proficiency;
            case StatType.LifeSteal: return ref lifeSteal;
            case StatType.ManaSteal: return ref manaSteal;
            case StatType.DmgIncrease: return ref dmgIncrease;
            case StatType.MaxHp: return ref maxHp;
            case StatType.MaxMp: return ref maxMp;
            case StatType.MaxHunger: return ref maxHunger;
            case StatType.Def: return ref def;
            case StatType.MDef: return ref mDef;
            case StatType.Eva: return ref eva;
            case StatType.Block: return ref block;
            case StatType.Resist: return ref resist;
            case StatType.DmgReduction: return ref dmgReduction;
            case StatType.Sight: return ref sight;
            case StatType.Instinct: return ref instinct;
            case StatType.SearchRange: return ref searchRange;
            case StatType.HpRegen: return ref hpRegen;
            case StatType.MpRegen: return ref mpRegen;
            case StatType.Speed: return ref speed;
            default: throw new NotImplementedException();
        }
    }

    public void AddSkill(SkillData skill, int index)
    {
        Skills[index] = skill;
        learnedSkills.Add(skill);
        OnSkillChanged?.Invoke();
    }
    public void SwapSkillSlots(int a, int b)
    {
        (Skills[a], Skills[b]) = (Skills[b], Skills[a]);
        OnSkillChanged?.Invoke();
    }
    public void ReduceSkillCurrentCooldowns(int turn)
    {
        for(int i=0; i<Skills.Length; i++)
        {
            if ((Skills[i] != null) && (Skills[i].currentCoolDown >= 1))
            {
                Skills[i].currentCoolDown -= turn;
            }
        }
        OnSkillCurrentCooldownChanged?.Invoke();
    }

    public void AddBuff(BuffData buff)
    {
        for (int i = 0; i < Buffs.Count; i++)
        {
            if (Buffs[i].Key == buff.Key)
            {
                Buffs[i].durationLeft = buff.MaxDuration + 1;
                return;
            }
        }

        Buffs.Add(buff);
        OnBuffListChanged?.Invoke();
    }
    public void RemoveBuff(BuffData buff)
    {
        Buffs.Remove(buff);
        OnBuffListChanged?.Invoke();
    }
    public void UpdateBuffDurations(int turnSpent)
    {
        for(int i=0; i<Buffs.Count; i++)
        {
            Buffs[i].durationLeft -= turnSpent;
            if (Buffs[i].durationLeft <= 0)
            {
                Buffs[i].durationLeft = 0;
            }
        }
        OnBuffDurationChanged?.Invoke();
    }
}
