using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
[JsonObject(IsReference = true)]
public class UnitData
{
    public delegate void EventHandler();
    public event EventHandler OnHpValueChange;
    public event EventHandler OnBarrierValueChange;
    public event EventHandler OnMpValueChange;
    public event EventHandler OnExpValueChange;
    public event EventHandler OnSkillChange;
    public event EventHandler OnSkillRechargeChange;
    public event EventHandler OnLevelChange;
    public event EventHandler OnBuffListChange;
    public event EventHandler OnBuffDurationChange;
    public event EventHandler OnBuffStackChange;
    public event EventHandler OnTurnChange;

    [JsonIgnore] public Unit Owner { get; private set; }
    [JsonProperty] public string Key { get; private set; } 
    public Coordinate coord = new(0, 0);
    [JsonProperty] private decimal _turnIndicator = 0;
    [JsonIgnore]
    public decimal TurnIndicator
    {
        get => _turnIndicator;
        set
        {
            _turnIndicator = value;
            OnTurnChange?.Invoke();
        }
    }
    [JsonProperty][System.NonSerialized] public List<UnitData> hostileTargets;
    public UnitData chaseTarget;
    public bool isChasingTarget = false;
    public Coordinate chaseTargetRecentCoord;

    [JsonIgnore] UnitBase unitBase;
    public Team team;
    public int level = 0;
    public int exp = 0;
    [JsonIgnore] public int maxExp;
    [JsonIgnore] public UnitStat maxHp;
    [JsonProperty] private int _hp = 0;
    [JsonIgnore]
    public int Hp {
        get => _hp;
        set
        {
            _hp = value >= 0 ? value <= maxHp.Total()? value : maxHp.Total() : 0;
            OnHpValueChange?.Invoke();
        }
    }
    [JsonIgnore] public UnitStat hpRegen;
    public int hpRegenCounter = 0;
    [JsonIgnore] public UnitStat maxMp;
    [JsonProperty] private int _mp = 0;
    [JsonIgnore]
    public int Mp
    {
        get => _mp;
        set
        {
            _mp = value >= 0 ? value <= maxMp.Total() ? value : maxMp.Total() : 0;
            OnMpValueChange?.Invoke();
        }
    }
    [JsonIgnore] public UnitStat mpRegen;
    [JsonProperty] private int _barrier = 0;
    [JsonIgnore]
    public int Barrier
    {
        get => _barrier;
        set
        {
            _barrier = value >= 0 ? value : 0;
            OnBarrierValueChange?.Invoke();
        }
    }
    public int mpRegenCounter = 0;
    [JsonIgnore] public UnitStat atk;
    [JsonIgnore] public UnitStat mAtk;
    [JsonIgnore] public UnitStat atkRange;
    [JsonIgnore] public UnitStat pen;
    [JsonIgnore] public UnitStat mPen;
    [JsonIgnore] public UnitStat acc;
    [JsonIgnore] public UnitStat aspd;
    [JsonIgnore] public UnitStat cri;
    [JsonIgnore] public UnitStat criDmg;
    [JsonIgnore] public UnitStat proficiency;
    [JsonIgnore] public UnitStat lifeSteal;
    [JsonIgnore] public UnitStat manaSteal;
    [JsonIgnore] public UnitStat def;
    [JsonIgnore] public UnitStat mDef;
    [JsonIgnore] public UnitStat eva;
    [JsonIgnore] public UnitStat tolerance;
    [JsonIgnore] public UnitStat resist;
    [JsonIgnore] public UnitStat dmgIncrease;
    [JsonIgnore] public UnitStat dmgReduction;
    [JsonIgnore] public UnitStat speed;
    [JsonIgnore] public UnitStat sight;
    [JsonIgnore] public UnitStat searchRange;
    [JsonIgnore] public UnitStat maxHunger;
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
    [JsonIgnore] public Dictionary<AdditionalEffectKey, int[]> AdditionalEffects { get; private set; } = new();

    public Dictionary<string,AbilityData> abilities = new();
    public int abilityPoint = 0;

    public string[] currentSkills = new string[5];
    public int[] skillRechargeLeft = { 0, 0, 0, 0, 0 }; 
    public List<string> acquiredSkills = new();

    public List<BuffData> Buffs = new();

    public int maxEquip = 20;
    public List<EquipmentData> equipInventory = new();
    public int maxMisc = 20;
    public List<MiscData> miscInventory = new();

    public EquipmentData[] equipped = new EquipmentData[9];

    public void Init(Unit unit)
    {
        Owner = unit;
        unitBase = unit.UnitBase;
        Key = unitBase.Key;

        hostileTargets = new();

        team = unitBase.Team;
        maxExp = unitBase.MaxExp;

        maxHp = new(unitBase.MaxHp);
        hpRegen = new(unitBase.HpRegen);
        maxMp = new(unitBase.MaxMp);
        mpRegen = new(unitBase.MpRegen);
        atk = new(unitBase.Atk);
        mAtk = new(unitBase.MAtk);
        atkRange = new(unitBase.AtkRange);
        pen = new(unitBase.Pen);
        mPen = new(unitBase.MPen);
        acc = new(unitBase.Acc);
        aspd = new(unitBase.Aspd);
        cri = new(unitBase.Cri);
        criDmg = new(unitBase.CriDmg);
        proficiency = new(unitBase.Proficiency);
        lifeSteal = new(unitBase.LifeSteal);
        manaSteal  = new(unitBase.ManaSteal);
        def = new(unitBase.Def);
        mDef = new(unitBase.MDef);
        eva = new(unitBase.Eva);
        tolerance = new(unitBase.Toleracne);
        resist = new(unitBase.Resist);
        dmgIncrease = new(unitBase.DmgIncrease);
        dmgReduction = new(unitBase.DmgReduction);
        speed = new(unitBase.Speed);
        sight = new(unitBase.Sight);
        searchRange = new(unitBase.SearchRange);
        maxHunger = new(unitBase.MaxHunger);

        if (Hp == 0)
        {
            Hp = maxHp.Total();
            Mp = maxMp.Total();
            Hunger = maxHunger.Total();
        }

        for(int i=0; i<level; i++)
        {
            IncreaseMaxExp(i);
            GrowStats(i);
        }
        for(int i=0; i<equipped.Length; i++)
        {
            if (equipped[i] != null)
                ApplyEquipStats(equipped[i]);
        }
        foreach(KeyValuePair<string, AbilityData> pair in abilities)
        {
            Owner.abilityDirector.ApplyAbility(pair.Value);
        }
        for(int i=0; i<Buffs.Count; i++)
        {
            Owner.BuffDirector.ApplyBuff(Buffs[i]);
        }
    }

    public void IncreaseExpValue(int amount)
    {
        exp += amount;
        while (exp >= maxExp)
        {
            exp -= maxExp;
            IncreaseLevelValue();
        }
        OnExpValueChange?.Invoke();
    }
    public void IncreaseLevelValue()
    {
        level++;
        IncreaseMaxExp(level);
        GrowStats(level);
        Hp = maxHp.Total();
        Mp = maxMp.Total();
        OnLevelChange?.Invoke();
    }
    void IncreaseMaxExp(int lv)
    {
        maxExp += (unitBase.MaxExp + lv * 5);
    }
    public void SetStartLevel(int lv)
    {
        for(int i=0; i < lv; i++)
        {
            IncreaseLevelValue();
        }
    }
    void GrowStats(int lv)
    {
        /*
        List<GrowthValue> growthTable = unitBase.GrowthTable;
        for(int i=0; i<growthTable.Count; i++)
        {
            int val = (int)(growthTable[i].amount * lv) - (int)(growthTable[i].amount * (lv - 1));
            StatTypeToUnitStat(growthTable[i].statType).original += val;
        }
        */
        float _maxHp = unitBase.MaxHp;
        float _maxMp = unitBase.MaxMp;
        float _atk = unitBase.Atk;
        float _def = unitBase.Def;
        float _mAtk = unitBase.MAtk;
        float _mDef = unitBase.MDef;
        for (int i=0; i<lv; i++)
        {
            _maxHp *= unitBase.GrowthRate;
            _maxMp *= unitBase.GrowthRate;
            _atk *= unitBase.GrowthRate;
            _def *= unitBase.GrowthRate;
            _mAtk *= unitBase.GrowthRate;
            _mDef *= unitBase.GrowthRate;
        }
        maxHp.original = (int)_maxHp;
        maxMp.original = (int)_maxMp;
        atk.original = (int)_atk;
        def.original = (int)_def;
        mAtk.original = (int)_mAtk;
        mDef.original = (int)_mDef;
    }

    public void SetStatValue(string key, StatType statType, StatValueType statValueType, int amount, bool overWrite)
    {
        switch (statValueType)
        {
            case StatValueType.Original:
                throw new System.NotImplementedException();
            case StatValueType.Additional:
                if (StatTypeToUnitStat(statType).additional.ContainsKey(key))
                {
                    if (overWrite) StatTypeToUnitStat(statType).additional[key] = amount;
                    else StatTypeToUnitStat(statType).additional[key] += amount;
                }
                else
                    StatTypeToUnitStat(statType).additional.Add(key, amount);
                break;
            case StatValueType.Temporary:
                if (StatTypeToUnitStat(statType).temporary.ContainsKey(key))
                {
                    if (overWrite) StatTypeToUnitStat(statType).temporary[key] = amount;
                    else StatTypeToUnitStat(statType).temporary[key] += amount;
                }
                else
                    StatTypeToUnitStat(statType).temporary.Add(key, amount);
                break;
            case StatValueType.Percent:
                if (StatTypeToUnitStat(statType).percent.ContainsKey(key))
                {
                    if(overWrite) StatTypeToUnitStat(statType).percent[key] = amount;
                    else StatTypeToUnitStat(statType).percent[key] = amount;
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
                OnHpValueChange.Invoke();
                break;
            case StatType.MaxMp:
                OnHpValueChange.Invoke();
                break;
        }
    }

    public void ApplyEquipStats(EquipmentData equip)
    {
        for(int i=0; i<equip.Stats.Count; i++)
        {
            if (equip.Stats[i].statUnit == StatUnit.Value)
                SetStatValue(equip.Key, equip.Stats[i].statType, StatValueType.Additional, equip.Stats[i].val + equip.Stats[i].bonus, false);
            else if (equip.Stats[i].statUnit == StatUnit.Percent)
                SetStatValue(equip.Key, equip.Stats[i].statType, StatValueType.Percent, equip.Stats[i].val + equip.Stats[i].bonus, false);
        }
        for (int i = 0; i < equip.Potentials.Count; i++)
        {
            if (equip.Potentials[i].statUnit == StatUnit.Value)
                SetStatValue(equip.Key, equip.Potentials[i].statType, StatValueType.Additional, equip.Potentials[i].val, false);
            else if (equip.Potentials[i].statUnit == StatUnit.Percent)
                SetStatValue(equip.Key, equip.Potentials[i].statType, StatValueType.Percent, equip.Potentials[i].val, false);
        }
        EquipmentBase equipBase = GameManager.Instance.GetEquipmentBase(equip.Key);
        if (equipBase.abilities?.Length > 0)
        {
            for(int i=0; i<equipBase.abilities.Length; i++)
            {
                if (equipBase.abilities[i].vals?.Count > 0)
                {
                    if (AdditionalEffects.ContainsKey(equipBase.abilities[i].key))
                        AdditionalEffects[equipBase.abilities[i].key] = GlobalMethods.AddArrays(AdditionalEffects[equipBase.abilities[i].key], equipBase.abilities[i].vals, equip.EquipmentAblitiyBonus).ToArray();
                    else AdditionalEffects.Add(equipBase.abilities[i].key, equipBase.abilities[i].vals.ToArray());
                }
                else
                {
                    if (AdditionalEffects.ContainsKey(equipBase.abilities[i].key))
                        AdditionalEffects[equipBase.abilities[i].key][0]++;
                    else AdditionalEffects.Add(equipBase.abilities[i].key, new int[] {1});
                }
            }
        }
    }
    public void RemoveEquipStats(EquipmentData equip)
    {
        for (int i = 0; i < equip.Stats.Count; i++)
        {
            if (equip.Stats[i].statUnit == StatUnit.Value)
                RemoveStatValue(equip.Key, equip.Stats[i].statType, StatValueType.Additional);
            else if (equip.Stats[i].statUnit == StatUnit.Percent)
                RemoveStatValue(equip.Key, equip.Stats[i].statType, StatValueType.Percent);
        }
        for (int i = 0; i < equip.Potentials.Count; i++)
        {
            if (equip.Potentials[i].statUnit == StatUnit.Value)
                RemoveStatValue(equip.Key, equip.Potentials[i].statType, StatValueType.Additional);
            else if (equip.Potentials[i].statUnit == StatUnit.Percent)
                RemoveStatValue(equip.Key, equip.Potentials[i].statType, StatValueType.Percent);
        }
        EquipmentBase equipBase = GameManager.Instance.GetEquipmentBase(equip.Key);
        if (equipBase.abilities?.Length > 0)
        {
            for (int i = 0; i < equipBase.abilities.Length; i++)
            {
                if (equipBase.abilities[i].vals?.Count > 0)
                    AdditionalEffects[equipBase.abilities[i].key] = GlobalMethods.SubstractArrays(AdditionalEffects[equipBase.abilities[i].key], equipBase.abilities[i].vals, equip.EquipmentAblitiyBonus).ToArray();
                else
                    AdditionalEffects[equipBase.abilities[i].key][0]--;
                if (AdditionalEffects[equipBase.abilities[i].key][0] == 0)
                    AdditionalEffects.Remove(equipBase.abilities[i].key);
            }
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
            case StatType.Tolerance: return ref tolerance;
            case StatType.Resist: return ref resist;
            case StatType.DmgReduction: return ref dmgReduction;
            case StatType.Sight: return ref sight;
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

    public void AddSkill(string key, int index)
    {
        currentSkills[index] = key;
        acquiredSkills.Add(key);
        OnSkillChange?.Invoke();
    }
    public void SwapSkillSlots(int a, int b)
    {
        (currentSkills[a], currentSkills[b]) = (currentSkills[b], currentSkills[a]);
        OnSkillChange?.Invoke();
    }
    public void InvokeSkillChanged()
    {
        OnSkillChange?.Invoke();
    }
    void UpdateSkillRechargeLeft(int turn)
    {
        for(int i=0; i<currentSkills.Length; i++)
        {
            if (currentSkills[i] ==null)
            {
                if (skillRechargeLeft[i] >= 1)
                {
                    skillRechargeLeft[i] -= turn;
                }
                if (skillRechargeLeft[i] <= 0 && acquiredSkills.Count > 0)
                {
                    skillRechargeLeft[i] = 0;
                    List<string> deck = new();
                    for (int j = 0; j < acquiredSkills.Count; j++)
                    {
                        int count = 0;
                        for (int k = 0; k < currentSkills.Length; k++)
                        {
                            if (currentSkills[k] == acquiredSkills[j]) count++;
                        }
                        if (count < 1) deck.Add(acquiredSkills[j]);
                    }
                    if (deck.Count > 0)
                        currentSkills[i] = deck[Random.Range(0, deck.Count)];
                }
                OnSkillChange?.Invoke();
            }
        }
        OnSkillRechargeChange?.Invoke();
    }

    public void AddBuff(BuffData buff)
    {
        for (int i = 0; i < Buffs.Count; i++)
        {
            if (Buffs[i].Key == buff.Key)
            {
                Buffs[i].durationLeft = buff.MaxDuration + 1;
                if (Buffs[i].BaseData.IsStackable)
                {
                    Buffs[i].stack++;
                    OnBuffStackChange?.Invoke();
                }
                return;
            }
        }

        Buffs.Add(buff);
        OnBuffListChange?.Invoke();
        if (buff.BaseData.IsStackable)
        {
            buff.stack = 1;
            OnBuffStackChange?.Invoke();
        }
    }
    public void RemoveBuff(BuffData buff)
    {
        Buffs.Remove(buff);
        Owner.RemoveParticleEffect(buff.Key);
        OnBuffListChange?.Invoke();
    }
    void UpdateBuffDurations(int turnSpent)
    {
        for(int i=0; i<Buffs.Count; i++)
        {
            Buffs[i].durationLeft -= turnSpent;
            if (Buffs[i].durationLeft <= 0)
            {
                Buffs[i].durationLeft = 0;
            }
        }
        OnBuffDurationChange?.Invoke();
    }
    public bool ContainsBuff(string key)
    {
        for (int i = 0; i < Buffs.Count; i++)
        {
            if (Buffs[i].Key == key)
            {
                return true;
            }
        }
        return false;
    }
    public bool ContainsBuffStack(string key, int requiredStack)
    {
        for (int i = 0; i < Buffs.Count; i++)
        {
            if (Buffs[i].Key == key && Buffs[i].stack >= requiredStack)
            {
                return true;
            }
        }
        return false;
    }
    public void ConsumeBuffStack(string key, int amount)
    {
        for (int i = 0; i < Buffs.Count; i++)
        {
            if (Buffs[i].Key == key)
            {
                Buffs[i].stack -= amount;
                if (Buffs[i].stack <= 0)
                {
                    Buffs.RemoveAt(i);
                    OnBuffListChange.Invoke();
                }
                else OnBuffStackChange.Invoke();
                break;
            }
        }
    }

    public void UpdateDurations(int turn)
    {
        UpdateBuffDurations(turn);
        UpdateSkillRechargeLeft(turn);
        if (Barrier > 0)
        {
            Barrier -= turn;
            Owner.UpdateHpBar();
        }
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
            if ((miscInventory[i].Key == misc.Key) && (misc.Amount < GameManager.Instance.GetMiscBase(misc.Key).MaxStack))
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
    public bool RemoveOneMisc(string key)
    {
        int index;
        for (int i = miscInventory.Count - 1; i >= 0; i--)
        {
            if (miscInventory[i].Key == key)
            {
                index = i;
                RemoveOneMisc(index);
                return true;
            }
        }
        return false;
    }
}
