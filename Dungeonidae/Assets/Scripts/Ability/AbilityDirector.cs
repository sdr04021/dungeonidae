using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDirector
{
    Unit owner;

    public AbilityDirector(Unit owner)
    {
        this.owner = owner;
    }

    public void ApplyAbility(AbilityData ability)
    {
        if (ability.Level == 0) return;

        switch (ability.Key)
        {
            case "HP_INCREASE":
                owner.UnitData.SetStatValue(ability.Key, StatType.MaxHp, StatValueType.Percent, ability.EffectValues[ability.Level - 1], true);
                owner.UpdateHpBar();
                break;
            case "BERSERK":
                owner.UnitData.OnHpValueChange += new UnitData.EventHandler(Berserk);
                Berserk();
                break;
        }
    }

    public void RemoveAbility(AbilityData ability)
    {
        if (ability.Level == 0) return;

        switch (ability.Key)
        {
            case "HP_INCREASE":
                owner.UnitData.RemoveStatValue(ability.Key, StatType.MaxHp, StatValueType.Percent);
                owner.UpdateHpBar();
                break;
            case "BERSERK":
                owner.UnitData.OnHpValueChange -= new UnitData.EventHandler(Berserk);
                owner.UnitData.RemoveStatValue(ability.Key, StatType.Atk, StatValueType.Percent);
                break;
        }
    }

    void Berserk()
    {
        AbilityData ability = owner.UnitData.abilities["BERSERK"];
        float lostHpPercent = ((owner.UnitData.maxHp.Total() - owner.UnitData.Hp) / (float)owner.UnitData.maxHp.Total()) * 100;
        owner.UnitData.SetStatValue(ability.Key, StatType.Atk, StatValueType.Percent, (int)(lostHpPercent / ability.EffectValues[ability.Level - 1]), true);
    }
}
