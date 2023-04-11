using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : Skill
{
    public BasicAttack(Unit owner, DungeonManager dm, SkillData skillData) : base(owner, dm, skillData) { }

    public override void Prepare()
    {
        if (owner is Player)
            SetRange(true);
        else SetRange(false);
    }

    public override bool IsUsable()
    {
        if (owner.UnitData.aspd.Total() > 0)
            return true;
        else return false;
    }

    public override void SetRange(bool showRange)
    {
        this.showRange = showRange;
        int range = owner.UnitData.atkRange.Total();
        for (int i = owner.UnitData.coord.x - range; i <= owner.UnitData.coord.x + range; i++)
        {
            for (int j = owner.UnitData.coord.y - range; j <= owner.UnitData.coord.y + range; j++)
            {
                if (dm.IsValidIndexForMap(i, j) && (!dm.fogMap.GetElementAt(i,j).FogData.IsOn))
                {
                    Tile tile = dm.map.GetElementAt(i, j);
                    if ((tile.TileData.tileType == TileType.Floor) && (tile.Coord != owner.UnitData.coord))
                    {
                        if ((tile.unit != null) && (owner.IsHostileUnit(tile.unit)))
                        {
                            if (showRange)
                                tile.SetAvailable();
                            AvailableTilesInRange.Add(tile.Coord);
                        }
                        else
                        {
                            if (showRange)
                                tile.SetUnavailable();
                            UnAvailableTilesInRange.Add(tile.Coord);
                        }
                    }
                }
            }
        }
    }

    public override void StartSkill(Coordinate coord)
    {
        ResetTilesInRange();
        owner.MyAnimator.SetBool("Attack", true);
        owner.StartCoroutine(SkillEffect(coord));
    }

    protected override IEnumerator SkillEffect(Coordinate coord)
    {
        Unit target = dm.GetTileByCoordinate(coord).unit;
        float animationLength = owner.MyAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength * 0.5f);
        target.GetDamage(MakeAttackData(target));
        while (!owner.isAnimationFinished || !target.isAnimationFinished)
        {
            yield return Constants.ZeroPointOne;
        }
        owner.isAnimationFinished = false;
        target.isAnimationFinished = false;
        owner.EndSkill(100f / owner.UnitData.aspd.Total());
    }

    public override void ShowTargetArea(Coordinate coord)
    {
        for (int i = 0; i < targetArea.Count; i++)
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

        if (AvailableTilesInRange.Contains(coord))
        {
            targetArea.Add(coord);
        }

        return targetArea;
    }

    public override Coordinate? SelectTargetAutomatically()
    {
        return AvailableTilesInRange[0];
    }

    public override AttackData MakeAttackData(Unit target)
    {
        AttackData ad = new(owner, owner.UnitData.atk.Total(), 0, 0);
        return ad;
    }
}
