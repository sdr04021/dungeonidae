using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[JsonObject(IsReference = true)]
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

    [JsonIgnore] public Unit Owner { get; private set; }
    public Coordinate coord = new(0, 0);
    public float turnIndicator = 0;
    public int unitListIndex = 0;
    public UnitData chaseTarget;
    public Coordinate chaseTargetRecentCoord;

    public string unitName;
    public Team team;
    public int level;
    public int exp;
    public int maxExp;
    public int[] expTable;
    public int[] expReward;
    public UnitStat maxHp;
    [JsonProperty] private int _hp;
    [JsonIgnore]
    public int Hp {
        get => _hp;
        set
        {
            _hp = value >= 0 ? value <= maxHp.Total()? value : maxHp.Total() : 0;
            OnHpValueChanged?.Invoke();
        }
    }

    public UnitStat hpRegen;
    public UnitStat maxMp;
    [JsonProperty] private int _mp;
    [JsonIgnore]
    public int Mp
    {
        get => _mp;
        set
        {
            _mp = value >= 0 ? value <= maxMp.Total() ? value : maxMp.Total() : 0;
            OnMpValueChanged?.Invoke();
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
    [JsonProperty] private int _hunger;
    [JsonIgnore]
    public int Hunger
    {
        get => _hunger;
        set
        {
            _hunger = value >= 0 ? value <= maxHunger.Total() ? value : maxHunger.Total() : 0;
        }
    }

    int maxHpGrowth;
    int maxMpGrowth;
    int atkGrowth;
    int mAtkGrowth;
    int defGrowth;
    int mDefGrowth;

    public Dictionary<string,AbilityData> abilities = new();
    public int abilityPoint = 0;

    public SkillData[] Skills = new SkillData[5];
    public List<SkillData> learnedSkills = new();

    public List<BuffData> Buffs = new();

    public int maxEquip = 20;
    public List<EquipmentData> equipInventory = new();
    public int maxMisc = 20;
    public List<MiscData> miscInventory = new();

    public EquipmentData[] equipped = new EquipmentData[9];

    public UnitData(UnitBase unitBase)
    {
        if (unitBase == null) return;
        unitName = unitBase.key;
        team = unitBase.team;
        level = unitBase.level;
        maxExp = unitBase.expTable[0];
        exp = 0;
        expTable = unitBase.expTable;
        expReward = unitBase.expReward;

        maxHp = new(unitBase.maxHp);
        Hp = maxHp.original;
        hpRegen = new(unitBase.hpRegen);
        maxMp = new(unitBase.maxMp);
        Mp = maxMp.original;
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

    public void SetOwner(Unit unit)
    {
        Owner = unit;
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
                throw new System.NotImplementedException();
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
                throw new System.NotImplementedException();
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
            default: throw new System.NotImplementedException();
        }
    }

    public void AddAbility(AbilityData ability)
    {
        abilities.Add(ability.Key, ability);
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
    public void InvokeSkillChanged()
    {
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

    public bool AddEquipment(EquipmentData equip)
    {
        if (equipInventory.Count >= maxEquip)
            return false;
        else
        {
            equipInventory.Add(equip);
            return true;
        }
    }

    public bool AddMisc(MiscData misc)
    {
        for (int i = miscInventory.Count - 1; i >= 0; i--)
        {
            if ((miscInventory[i].Key == misc.Key) && (misc.Amount < misc.MaxStack))
            {
                if (misc.Amount <= (miscInventory[i].AmountLeft))
                {
                    miscInventory[i].AddAmount(misc.Amount);
                    return true;
                }
                else
                {
                    misc.AddAmount(-miscInventory[i].AmountLeft);
                    miscInventory[i].AddAmount(miscInventory[i].AmountLeft);
                    break;
                }
            }
        }

        if (miscInventory.Count >= maxMisc)
            return false;
        else
        {
            miscInventory.Add(misc);
            return true;
        }
    }

    public void RemoveOneMisc(int index)
    {
        miscInventory[index].AddAmount(-1);
        if (miscInventory[index].Amount <= 0)
            miscInventory.RemoveAt(index);
    }
}
