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
    int maxHp;
    public int MaxHp { get => maxHp; }

    [SerializeField]
    int atk;
    public int Atk { get => atk; }

    [SerializeField]
    int atkRange;
    public int AtkRange { get => atkRange; }

    [SerializeField]
    int aspd;
    public int Aspd { get => aspd; }

    [SerializeField]
    int def;
    public int Def { get => def; }

    [SerializeField]
    int speed;
    public int Speed { get => speed; }

    [SerializeField]
    int sight;
    public int Sight { get => sight; }
}
