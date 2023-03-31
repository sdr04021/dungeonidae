using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPowerStrike : Skill
{
    List<Coordinate> targetArea = new();

    public SkillPowerStrike(Unit owner, DungeonManager dm, SkillData skillData) : base(owner, dm, skillData)
    {
    }

    public override void Prepare()
    {
        SetRange();
    }

    public override bool IsUsable()
    {
        if ((owner.UnitData.Mp >= skillData.EffectValues[0]) && (skillData.coolDown == 0))
            return true;
        else return false;
    }

    public override void SetRange()
    {
        for (int i = owner.Coord.x - 1; i <= owner.Coord.x + 1; i++)
        {
            for (int j = owner.Coord.y - 1; j <= owner.Coord.y + 1; j++)
            {
                if (dm.IsValidIndexForMap(i, j) && (!dm.FogMap[i, j].IsOn))
                {
                    Tile tile = dm.map[i, j];
                    if ((tile.type == TileType.Floor) && (tile.Coord != owner.Coord))
                    {
                        if ((tile.unit != null) && (owner.IsHostileUnit(tile.unit)))
                        {
                            if (owner is Player)
                                tile.SetAvailable();
                            availableTilesInRange.Add(tile.Coord);
                        }
                        else
                        {
                            if (owner is Player)
                                tile.SetUnavailable();
                            unAvailableTilesInRange.Add(tile.Coord);
                        }
                    }
                }
            }
        }
    }
    
    public override void ResetTilesInRange()
    {
        if(owner is Player)
        {
            for (int i = 0; i < availableTilesInRange.Count; i++)
            {
                dm.GetTileByCoordinate(availableTilesInRange[i]).TurnOffRangeIndicator();
            }
            for (int i = 0; i < unAvailableTilesInRange.Count; i++)
            {
                dm.GetTileByCoordinate(unAvailableTilesInRange[i]).TurnOffRangeIndicator();
            }
        }

        availableTilesInRange.Clear();
        unAvailableTilesInRange.Clear();
    }

    public override void StartSkill(Coordinate coord)
    {
        ResetTilesInRange();
        owner.UnitData.Mp -= skillData.EffectValues[0];
        owner.EffectAnimator.SetTrigger(skillData.Key);
        owner.MyAnimator.SetBool("Attack", true);
        owner.StartCoroutine(SkillEffect(dm.GetTileByCoordinate(coord).unit));
    }

    protected override IEnumerator SkillEffect(Unit target)
    {
        float animationLength = owner.MyAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength * 0.5f);
        target.GetDamage(MakeAttackData(target));
        while (!owner.isAnimationFinished || !target.isAnimationFinished)
        {
            yield return Constants.ZeroPointOne;
        }
        owner.isAnimationFinished = false;
        target.isAnimationFinished = false;
        skillData.coolDown = skillData.EffectValues[2] + 1;
        owner.EndSkill(1);
    }

    public override void ShowTargetArea(Coordinate coord)
    {
        for(int i = 0; i<targetArea.Count; i++)
        {
            dm.GetTileByCoordinate(targetArea[i]).targetMark.SetActive(false);
        }
        targetArea = SetTargetArea(coord);
        for (int i = 0; i < targetArea.Count; i++)
        {
            dm.GetTileByCoordinate(targetArea[i]).targetMark.SetActive(true);
        }
    }

    public override List<Coordinate> SetTargetArea(Coordinate coord)
    {
        List<Coordinate> targetArea = new();

        if (availableTilesInRange.Contains(coord))
        {
            targetArea.Add(coord);
        }

        return targetArea;
    }

    public override AttackData MakeAttackData(Unit target)
    {
        AttackData ad = new(owner, (int)(owner.UnitData.Atk.Total() * (skillData.EffectValues[1]/100f)), 0);
        return ad;
    }
}
