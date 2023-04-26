using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GrowthValue
{
    public StatType statType;
    public float amount; 
}

[CreateAssetMenu(fileName = "UnitBase", menuName = "Scriptable Object/UnitBase")]
public class UnitBase : ScriptableObject
{
    [field:SerializeField] public string Key { get; private set; }
    [field: SerializeField] public Team Team { get; private set; }
    [field: SerializeField] public int MaxExp { get; private set; } = 10;
    [field: SerializeField] public float ExpCoefficient { get; private set; } = 1;
    [field: SerializeField] public int MaxHp { get; private set; } = 10;
    [field: SerializeField] public int HpRegen { get; private set; } = 0;
    [field: SerializeField] public int MaxMp { get; private set; } = 0;
    [field: SerializeField] public int MpRegen { get; private set; } = 0;
    [field: SerializeField] public int MaxHunger { get; private set; } = 100;
    [field: SerializeField] public int Atk { get; private set; } = 0;
    [field: SerializeField] public int MAtk { get; private set; } = 0;
    [field: SerializeField] public int AtkRange { get; private set; } = 1;
    [field: SerializeField] public int Pen { get; private set; } = 0;
    [field: SerializeField] public int MPen { get; private set; } = 0;
    [field: SerializeField] public int Acc { get; private set; } = 0;
    [field: SerializeField] public int Aspd { get; private set; } = 100;
    [field: SerializeField] public int Cri { get; private set; } = 0;
    [field: SerializeField] public int CriDmg { get; private set; } = 150;
    [field: SerializeField] public int Proficiency { get; private set; } = 0;
    [field: SerializeField] public int LifeSteal { get; private set; } = 0;
    [field: SerializeField] public int ManaSteal { get; private set; } = 0;
    [field: SerializeField] public int Def { get; private set; } = 0;
    [field: SerializeField] public int MDef { get; private set; } = 0;
    [field: SerializeField] public int Eva { get; private set; } = 0;
    [field: SerializeField] public int CoolSpeed { get; private set; } = 100;
    [field: SerializeField] public int Resist { get; private set; } = 0;
    [field: SerializeField] public int DmgIncrease { get; private set; } = 0;
    [field: SerializeField] public int DmgReduction { get; private set; } = 0;
    [field: SerializeField] public int Speed { get; private set; } = 100;
    [field: SerializeField] public int Sight { get; private set; } = 5;
    [field: SerializeField] public int Instinct { get; private set; } = 10;
    [field: SerializeField] public int SearchRange { get; private set; } = 1;
    [field: SerializeField] public int Stealth { get; private set; } = 0;

    [field: SerializeField] public List<GrowthValue> GrowthTable { get; private set; }
}
