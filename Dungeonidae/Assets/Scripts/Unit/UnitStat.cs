using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UnitStat
{
    public struct StatValue
    {
        public StatValueType type;
        public float value;
    }

    public int original = 0;
    public Dictionary<string, int> additional;
    public Dictionary<string, int> temporary;
    public Dictionary<string, int> percent;
    public int minLimit = 0;

    public UnitStat()
    {
        additional ??= new();
        temporary ??= new();
        percent ??= new();
    }

    public UnitStat(int original)
    {
        this.original = original;
        additional = new();
        temporary = new();
        percent = new();
        minLimit = 0;
    }
    public UnitStat(int original, int minLimit)
    {
        this.original = original;
        additional = new();
        temporary = new();
        percent = new();
        this.minLimit = minLimit;
    }

    public int Total()
    {
        int addi = 0;
        int temp = 0;
        decimal mult = 100;
        
        List<int> values = additional.Values.ToList();
        for (int i = 0; i < values.Count; i++)
            addi += values[i];
        values = temporary.Values.ToList();
        for (int i = 0; i < values.Count; i++)
            temp += values[i];
        values = percent.Values.ToList();
        for(int i=0; i<values.Count; i++)
        {
            mult += values[i];
        }
        mult *= 0.01m;
        if (mult < 0) mult = 0;

        return Mathf.Max(minLimit, (int)((original + addi) * mult) + temp);
    }
}
