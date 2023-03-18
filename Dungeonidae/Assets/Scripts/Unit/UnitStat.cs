using System.Collections;
using System.Collections.Generic;

public struct UnitStat
{
    int original;
    public int Original { get { return original; } set { original = value; } }
    int passive;
    int additional;
    int percent;

    public UnitStat(int original)
    {
        this.original = original;
        passive = 0;
        additional = 0;
        percent = 0;
    }

    public int Total()
    {
        decimal mult = (100 + percent) / 100m;
        if (mult < 0) mult = 0;
        return (int)((original + passive) * mult) + additional;
    }
}
