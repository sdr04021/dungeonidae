using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDirector
{
    Player player;

    public AbilityDirector(Player player)
    {
        this.player = player;
    }

    public void ApplyAbility(AbilityData ability)
    {
        if (ability.Level == 0) return;

        switch (ability.Key)
        {
            case "HP_INCREASE":
                player.UnitData.SetStatValue(ability.Key, StatType.HP, StatValueType.Percent, ability.EffectValues[ability.Level - 1]);
                player.UpdateHpBar();
                break;
            case "BERSERK":
                player.UnitData.OnHpValueChanged += new UnitData.EventHandler(Berserk);
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
                player.UnitData.RemoveStatValue(ability.Key, StatType.HP, StatValueType.Percent);
                player.UpdateHpBar();
                break;
            case "BERSERK":
                player.UnitData.OnHpValueChanged -= new UnitData.EventHandler(Berserk);
                player.UnitData.RemoveStatValue(ability.Key, StatType.Atk, StatValueType.Percent);
                break;
        }
    }

    void Berserk()
    {
        AbilityData ability = player.PlayerData.Abilities[player.PlayerData.AbilityNameToIndex["BERSERK"]];
        float lostHpPercent = ((player.UnitData.MaxHp.Total() - player.UnitData.Hp) / (float)player.UnitData.MaxHp.Total()) * 100;
        player.UnitData.SetStatValue(ability.Key, StatType.Atk, StatValueType.Percent, (int)(lostHpPercent / ability.EffectValues[ability.Level - 1]));
    }
}
