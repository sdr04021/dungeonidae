using System.Collections;
using System.Collections.Generic;

public struct UnitStat
{
    public int original;
    public int additional;
    public int temporary;
    public int percent;

    public UnitStat(int original)
    {
        this.original = original;
        additional = 0;
        temporary = 0;
        percent = 0;
    }

    public int Total()
    {
        decimal mult = (100 + percent) / 100m;
        if (mult < 0) mult = 0;
        return (int)((original + additional) * mult) + temporary;
    }
}
