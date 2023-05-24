using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Constants
{
    public const int Step = 1;
    public static Color Transparent = new(1, 1, 1, 0);
    public static readonly WaitForSeconds ZeroPointZeroOne = new(0.01f);
    public static readonly WaitForSeconds ZeroPointZeroTwo = new(0.02f);
    public static readonly WaitForSeconds ZeroPointOne = new(0.1f);
    public static readonly WaitForSeconds ZeroPointFive = new(0.5f);
    public static HashSet<StatType> PercentPointStats = new() { StatType.Pen, StatType.MPen, StatType.Proficiency,StatType.Cri,StatType.CriDmg,
            StatType.Aspd,StatType.LifeSteal,StatType.ManaSteal,StatType.Resist,StatType.CoolSpeed,StatType.Speed,StatType.DmgIncrease,StatType.DmgReduction };
}