using System.Collections;
using System.Collections.Generic;

public class DamageManager
{
    public void CalculateDamage(Unit attacker, Unit defender)
    {
        int amount = attacker.Data.atk.Total() - defender.Data.def.Total();
        if (amount < 0) amount = 0;
        //defender.GetDamage(amount);
    }
}
