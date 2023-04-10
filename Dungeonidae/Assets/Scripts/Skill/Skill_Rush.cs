using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Skill_Rush : Skill
{
    public Skill_Rush(Unit owner, DungeonManager dm, SkillData skillData) : base(owner, dm, skillData)
    {
    }

    public override void Prepare()
    {
        if (owner is Player)
            SetRange(true);
        else
            SetRange(false);
    }

    public override bool IsUsable()
    {
        if ((owner.UnitData.Mp >= SkillData.EffectValues[0]) && (SkillData.currentCoolDown == 0))
            return true;
        else return false;
    }

    public override void SetRange(bool showRange)
    {
        this.showRange = showRange;
        RaycastHit2D[] hit;
        for (int i = owner.UnitData.coord.x - SkillData.EffectValues[1]; i <= owner.UnitData.coord.x + SkillData.EffectValues[1]; i++)
        {
            for (int j = owner.UnitData.coord.y - SkillData.EffectValues[1]; j <= owner.UnitData.coord.y + SkillData.EffectValues[1]; j++)
            {
                if (dm.IsValidIndexForMap(i, j) && (!dm.FogMap[i, j].FogData.IsOn))
                {
                    Vector2 dir = new Vector2(i, j) - (Vector2)owner.transform.position;
                    hit = Physics2D.RaycastAll(owner.transform.position, dir, dir.magnitude, LayerMask.GetMask("Tile"));

                    for (int k = 0; k < hit.Length; k++)
                    {
                        if (k == hit.Length - 1)
                        {
                            Tile tile = dm.Map[i, j];
                            if ((tile.TileData.tileType == TileType.Floor) && (tile.Coord != owner.UnitData.coord))
                            {
                                if ((tile.IsReachableTile()))
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
                        else if (dm.Map[(int)hit[k].transform.position.x, (int)hit[k].transform.position.y].TileData.tileType == TileType.Wall)
                            break;
                    }
                }
            }
        }
    }

    public override void StartSkill(Coordinate coord)
    {
        ResetTilesInRange();
        owner.UnitData.Mp -= SkillData.EffectValues[0];
        //owner.EffectAnimator.SetTrigger(SkillData.Key);
        //owner.MyAnimator.SetBool("Attack", true);
        dm.GetTileByCoordinate(owner.UnitData.coord).unit = null;
        owner.transform.DOMove(coord.ToVector3(0), 0.1f * Coordinate.Distance(owner.UnitData.coord, coord)).OnComplete(Finsh);
    }

    void Finsh()
    {
        owner.UpdateCoordinateFromTransform();

        SkillData.currentCoolDown = SkillData.EffectValues[2] + 1;
        owner.EndSkill(1);
    }

    protected override IEnumerator SkillEffect(Coordinate coord)
    {
        throw new System.NotImplementedException();
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

    public override AttackData MakeAttackData(Unit target)
    {
        throw new System.NotImplementedException();
    }

    public override Coordinate? SelectTargetAutomatically()
    {
        return null;
    }
}
