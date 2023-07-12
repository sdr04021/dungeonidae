using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryTower : Monster
{
    protected override void DecideBehavior()
    {
        if (!UnitData.AdditionalEffects.ContainsKey("MISSILE"))
            UnitData.AdditionalEffects.Add("MISSILE", new int[] { 0 });

        if (UnitData.chaseTarget == null)
            LookAround();

        if ((UnitData.chaseTarget != null) && UnitsInSight.Contains(UnitData.chaseTarget.Owner)
            && UnitData.coord.IsTargetInRange(UnitData.chaseTarget.coord, UnitData.atkRange.Total()))
        {

        }
        else EndTurn(1);
    }
}
