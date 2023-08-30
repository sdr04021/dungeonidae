using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EquipmentData : ItemData
{
    [JsonIgnore]
    EquipmentBase baseData;
    [JsonIgnore]
    public EquipmentBase BaseData
    {
        get
        {
            if (baseData == null) baseData = GameManager.Instance.GetEquipmentBase(Key);
            return baseData;
        }
    }

    [JsonProperty]
    public EquipmentType EquipmentType { get; private set; }

    [JsonProperty]
    public List<EquipmentStat> Stats { get; private set; } = new();

    [JsonProperty]
    public int Enchant { get; private set; }

    [JsonProperty]
    public List<EquipmentStat> Potentials { get; private set; } = new();

    [JsonProperty]
    public int PotentialExp { get; private set; } = 0;

    [JsonProperty]
    public List<int> EquipmentAblitiyBonus { get; private set; } = new();

    public EquipmentData() {}

    public EquipmentData(string key) : base (key)
    {
        baseData = GameManager.Instance.GetEquipmentBase(key);

        EquipmentType = baseData.equipmentType;

        for(int i=0; i < baseData.stats.Length; i++)
        {
            EquipmentStat stat = new()
            {
                statType = baseData.stats[i].statType,
                statUnit = baseData.stats[i].statUnit,
                val = baseData.stats[i].val,
                bonus = baseData.stats[i].bonus
            };
            if (i == 0)
            {
                stat.val += (int)(stat.val * (1 / ((Random.value - 1.05f) * -20f) - 0.047619f) * 0.5f);
            }

            if (stat.val != 0)
                Stats.Add(stat);
        }

        if (baseData.abilities?.Length > 0)
        {
            for(int i=0; i< baseData.abilities[0].vals.Count; i++)
            {
                EquipmentAblitiyBonus.Add(0);
            }
        }

        if (baseData.equipmentType == EquipmentType.Artifact)
        {
            PotentialExp = 30;
        }
    }

    public void GainPotentialExp()
    {
        if (PotentialExp < 30)
        {
            PotentialExp++;
            if (PotentialExp % 10 == 0)
            {
                AddNewPotential();
            }
        }
    }
    public void AddNewPotential()
    {
        EquipmentStat eStat = new();
      
        int pick = Random.Range(0, 1000);
        switch (pick)
        {
            case <100:
                eStat.val = 1;
                break;
            case < 300:
                eStat.val = 2;
                break;
            case < 700:
                eStat.val = 3;
                break;
            case < 900:
                eStat.val = 4;
                break;
            case < 1000:
                eStat.val = 5;
                break;
        }

        pick = Random.Range(0, 1000);
        if ((EquipmentType == EquipmentType.Weapon) || (EquipmentType == EquipmentType.Sub))
        {
            if (pick < 700)
            {
                switch (pick % 10)
                {
                    case 0:
                        eStat.statType = StatType.Atk;
                        break;
                    case 1:
                        eStat.statType = StatType.MAtk;
                        break;
                    case 2:
                        eStat.statType = StatType.Pen;
                        break;
                    case 3:
                        eStat.statType = StatType.MPen;
                        break;
                    case 4:
                        eStat.statType = StatType.Proficiency;
                        break;
                    case 5:
                        eStat.statType = StatType.Acc;
                        break;
                    case 6:
                        eStat.statType = StatType.Cri;
                        break;
                    case 7:
                        eStat.statType = StatType.Atk;
                        break;
                    case 8:
                        eStat.statType = StatType.LifeSteal;
                        break;
                    case 9:
                        eStat.statType = StatType.ManaSteal;
                        break;
                }
            }
            else if (pick < 990)
            {
                switch (pick % 10)
                {
                    case 0:
                        eStat.statType = StatType.MaxHp;
                        break;
                    case 1:
                        eStat.statType = StatType.MaxMp;
                        break;
                    case 2:
                        eStat.statType = StatType.Def;
                        break;
                    case 3:
                        eStat.statType = StatType.MDef;
                        break;
                    case 4:
                        eStat.statType = StatType.Eva;
                        break;
                    case 5:
                        eStat.statType = StatType.HpRegen;
                        break;
                    case 6:
                        eStat.statType = StatType.MpRegen;
                        break;
                    case 7:
                        eStat.statType = StatType.CriDmg;
                        break;
                    case 8:
                        eStat.statType = StatType.Instinct;
                        break;
                    case 9:
                        eStat.statType = StatType.MaxHunger;
                        break;
                }
            }
            else
            {
                switch (pick % 5)
                {
                    case 0:
                        eStat.statType = StatType.DmgIncrease;
                        break;
                    case 1:
                        eStat.statType = StatType.CoolSpeed;
                        break;
                    case 2:
                        eStat.statType = StatType.Stealth;
                        break;
                    case 3:
                        eStat.statType = StatType.Speed;
                        break;
                    case 4:
                        eStat.statType = StatType.Aspd;
                        break;
                }
            }
        }
        else
        {
            if (pick < 700)
            {
                switch (pick % 10)
                {
                    case 0:
                        eStat.statType = StatType.MaxHp;
                        break;
                    case 1:
                        eStat.statType = StatType.MaxMp;
                        break;
                    case 2:
                        eStat.statType = StatType.Def;
                        break;
                    case 3:
                        eStat.statType = StatType.MDef;
                        break;
                    case 4:
                        eStat.statType = StatType.Eva;
                        break;
                    case 5:
                        eStat.statType = StatType.HpRegen;
                        break;
                    case 6:
                        eStat.statType = StatType.MpRegen;
                        break;
                    case 7:
                        eStat.statType = StatType.Resist;
                        break;
                    case 8:
                        eStat.statType = StatType.Instinct;
                        break;
                    case 9:
                        eStat.statType = StatType.MaxHunger;
                        break;
                }
            }
            else if (pick < 990)
            {
                switch (pick % 10)
                {
                    case 0:
                        eStat.statType = StatType.Atk;
                        break;
                    case 1:
                        eStat.statType = StatType.MAtk;
                        break;
                    case 2:
                        eStat.statType = StatType.CriDmg;
                        break;
                    case 3:
                        eStat.statType = StatType.Instinct;
                        break;
                    case 4:
                        eStat.statType = StatType.Proficiency;
                        break;
                    case 5:
                        eStat.statType = StatType.Acc;
                        break;
                    case 6:
                        eStat.statType = StatType.Cri;
                        break;
                    case 7:
                        eStat.statType = StatType.Atk;
                        break;
                    case 8:
                        eStat.statType = StatType.LifeSteal;
                        break;
                    case 9:
                        eStat.statType = StatType.ManaSteal;
                        break;
                }
            }
            else
            {
                switch (pick % 5)
                {
                    case 0:
                        eStat.statType = StatType.DmgReduction;
                        break;
                    case 1:
                        eStat.statType = StatType.CoolSpeed;
                        break;
                    case 2:
                        eStat.statType = StatType.MPen;
                        break;
                    case 3:
                        eStat.statType = StatType.Pen;
                        break;
                    case 4:
                        eStat.statType = StatType.Speed;
                        break;
                }
            }
        }

        if (Constants.PercentPointStats.Contains(eStat.statType))
            eStat.statUnit = StatUnit.Value;
        else eStat.statUnit = StatUnit.Percent;
        
        Potentials.Add(eStat);
        EquipmentData[] equipped = GameManager.Instance.saveData.playerData.equipped;
        for(int i=0; i<equipped.Length; i++)
        {
            if (equipped[i] == this)
            {
                GameManager.Instance.saveData.playerData.RemoveEquipStats(this);
                GameManager.Instance.saveData.playerData.ApplyEquipStats(this);
                break;
            }
        }
    }

    public void EnchantEquipment()
    {
        Enchant++;
        if (Stats.Count > 0) Stats[0].bonus += (int)(1 + Stats[0].val * 0.25m);
        EquipmentBase equipBase = GameManager.Instance.GetEquipmentBase(Key);
        if (equipBase.abilities?.Length > 0)
        {
            for (int i = 0; i < equipBase.abilities[0].vals.Count; i++)
            {
                EquipmentAblitiyBonus[i] += equipBase.abilities[0].increments[i];
            }
        }

        EquipmentData[] equipped = GameManager.Instance.saveData.playerData.equipped;
        for (int i = 0; i < equipped.Length; i++)
        {
            if (equipped[i] == this)
            {
                GameManager.Instance.saveData.playerData.RemoveEquipStats(this);
                GameManager.Instance.saveData.playerData.ApplyEquipStats(this);
                break;
            }
        }
    }

    public override Sprite GetSprite()
    {
        return GameManager.Instance.GetSprite(SpriteAssetType.Equipment, Key);
    }
}
