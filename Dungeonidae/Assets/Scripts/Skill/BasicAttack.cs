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
                if (dm.IsValidIndexForMap(i, j) && (!dm.fogMap.GetElementAt(i, j).IsOn))
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
                        else if (owner.UnitData.coord.IsTargetInRange(tile.Coord,1) && tile.HasTargetable())
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
        owner.FlipSprite(coord);
        if (dm.map.GetElementAt(coord).unit != null)
        {
            if (owner.UnitData.AdditionalEffects.ContainsKey("MISSILE") && !Coordinate.InRange(owner.UnitData.coord, coord, owner.UnitData.AdditionalEffects["MISSILE"][0] + 0.5f))
            {
                owner.StartCoroutine(Missile(coord));
            }
            else
            {
                owner.MyAnimator.SetBool("Attack", true);
                owner.StartCoroutine(SkillEffect(coord));
            }
        }
        else
        {
            owner.MyAnimator.SetBool("Attack", true);
            owner.StartCoroutine(InteractionAnimation(dm.map.GetElementAt(coord).GetTargetable()));
        }
    }

    protected override IEnumerator SkillEffect(Coordinate coord)
    {
        Unit target = dm.GetTileByCoordinate(coord).unit;
        owner.isAnimationFinished = false;
        float animationLength = owner.MyAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength * 0.4f);
        target.GetDamage(MakeAttackData(target));
        owner.UnitData.equipped[1]?.GainPotentialExp();
        owner.EndSkill(100m / owner.UnitData.aspd.Total());
        while ((owner.MyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) || !target.isHitFinished)
        {
            yield return Constants.ZeroPointZeroOne;
        }
        owner.MyAnimator.SetBool("Attack", false);
        owner.isAnimationFinished = true;
    }

    IEnumerator Missile(Coordinate coord)
    {
        Unit target = dm.GetTileByCoordinate(coord).unit;
        owner.isAnimationFinished = false;
        Projectile projectile = GameObject.Instantiate(owner.BasicProjectilePrefab, owner.transform.position, Quaternion.identity);
        projectile.Init(coord.ToVector3(0));
        while (projectile != null)
        {
            yield return Constants.ZeroPointZeroOne;
        }
        target.GetDamage(MakeAttackData(target));
        owner.EndSkill(100m / owner.UnitData.aspd.Total());
        owner.isAnimationFinished = true;
    }

    IEnumerator InteractionAnimation(DungeonObject target)
    {
        owner.isAnimationFinished = false;
        float animationLength = owner.MyAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength * 0.4f);
        target.TargetedInteraction(owner);
        owner.EndSkill(1);
        while (owner.MyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return Constants.ZeroPointZeroOne;
        }
        owner.MyAnimator.SetBool("Attack", false);
        owner.isAnimationFinished = true;
    }

    public override void ShowTargetArea(Coordinate coord)
    {
        for (int i = 0; i < targetArea.Count; i++)
        {
            dm.GetTileByCoordinate(targetArea[i]).targetMark.gameObject.SetActive(false);
        }
        targetArea = SetTargetArea(coord);
        for (int i = 0; i < targetArea.Count; i++)
        {
            dm.GetTileByCoordinate(targetArea[i]).targetMark.gameObject.SetActive(true);
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
        for (int i = 0; i < AvailableTilesInRange.Count; i++)
        {
            if (dm.map.GetElementAt(AvailableTilesInRange[i]).unit!=null)
                return AvailableTilesInRange[i];
        }
        return AvailableTilesInRange[0];
    }

    public override AttackData MakeAttackData(Unit target)
    {
        AttackData ad = new(owner, owner.UnitData.atk.Total(), 0, 0);
        return ad;
    }
}
