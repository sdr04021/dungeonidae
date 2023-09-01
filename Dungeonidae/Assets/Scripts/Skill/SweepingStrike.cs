using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SWEEPING_STRIKE", menuName = "Scriptable Object/Skills/SweepingStrike")]
public class SweepingStrike : SkillBase
{
    public override IEnumerator Skill(Unit owner, DungeonManager dm, Coordinate coord)
    {
        owner.activeMotions++;
        owner.UnitData.Mp -= Cost;
        owner.FlipSprite(coord);
        List<Coordinate> targetCoords = ApplyingTargets(owner, coord);
        List<Unit> targets = new();
        bool isRendered = false;
        for(int i=0; i<targetCoords.Count; i++)
        {
            Unit unit = dm.map.GetElementAt(targetCoords[i]).unit;
            if (unit != null)
            {
                targets.Add(unit);
                if(unit.MySpriteRenderer.enabled) isRendered = true;
            }
        }
        if (owner.MySpriteRenderer.enabled || isRendered)
        {
            GameObject effect = Instantiate(GameManager.Instance.GetPrefab(PrefabAssetType.ParticleEffect, "SWEEPING_STRIKE"), owner.transform.position, Quaternion.identity);
            Directions direction = (coord - owner.UnitData.coord).ToDirection();
            if (direction == Directions.E || direction == Directions.SE)
            {
                effect.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
            }
            else if (direction == Directions.S || direction == Directions.SW)
            {
                effect.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
            }
            else if (direction == Directions.W || direction == Directions.NW)
            {
                effect.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            }

            owner.MyAnimator.SetBool("Skill", true);
            yield return Constants.ZeroPointZeroOne;
            float animationLength = owner.MyAnimator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationLength * 0.5f);
            AttackData ad = MakeAttackData(owner);
            for (int i=0; i<targets.Count; i++)
            {
                targets[i].GetDamage(ad);
            }
            owner.EndSkill(1);
            while ((effect!=null) || targets[0].passiveMotions > 0)
            {
                yield return Constants.ZeroPointZeroTwo;
            }
            owner.MyAnimator.SetBool("Skill", false);
            owner.activeMotions--;
        }
        else
        {
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].GetDamage(MakeAttackData(owner));
            }
            owner.EndSkill(1);
        }
    }

    public override void SetRange(Unit owner, DungeonManager dm, bool showRangeIndicator)
    {
        HashSet<Directions> skillRange = new();
        Coordinate origin = owner.UnitData.coord;

        if (dm.map.GetElementAt(origin.MovedCoordinate(Directions.N, 1)).unit != null)
            skillRange.Add(Directions.N);
        if(dm.map.GetElementAt(origin.MovedCoordinate(Directions.NE, 1)).unit != null)
        {
            skillRange.Add(Directions.N);
            skillRange.Add(Directions.E);
        }
        if (dm.map.GetElementAt(origin.MovedCoordinate(Directions.E, 1)).unit != null)
            skillRange.Add(Directions.E);
        if (dm.map.GetElementAt(origin.MovedCoordinate(Directions.SE, 1)).unit != null)
        {
            skillRange.Add(Directions.S);
            skillRange.Add(Directions.E);
        }
        if (dm.map.GetElementAt(origin.MovedCoordinate(Directions.S, 1)).unit != null)
            skillRange.Add(Directions.S);
        if (dm.map.GetElementAt(origin.MovedCoordinate(Directions.SW, 1)).unit != null)
        {
            skillRange.Add(Directions.S);
            skillRange.Add(Directions.W);
        }
        if (dm.map.GetElementAt(origin.MovedCoordinate(Directions.W, 1)).unit != null)
            skillRange.Add(Directions.W);
        if (dm.map.GetElementAt(origin.MovedCoordinate(Directions.NW, 1)).unit != null)
        {
            skillRange.Add(Directions.N);
            skillRange.Add(Directions.W);
        }

        Tile tile = dm.map.GetElementAt(origin.MovedCoordinate(Directions.N, 1));
        if (skillRange.Contains(Directions.N))
        {
            owner.AvailableRange.Add(origin.MovedCoordinate(Directions.N, 1));
            if (showRangeIndicator) tile.SetAvailable();
        }
        else if (tile.TileData.tileType != TileType.Wall)
        {
            owner.UnavailableRange.Add(origin.MovedCoordinate(Directions.N, 1));
            if (showRangeIndicator) tile.SetUnavailable();
        }

        tile = dm.map.GetElementAt(origin.MovedCoordinate(Directions.E, 1));
        if (skillRange.Contains(Directions.E))
        {
            owner.AvailableRange.Add(origin.MovedCoordinate(Directions.E, 1));
            if (showRangeIndicator) tile.SetAvailable();
        }
        else if (tile.TileData.tileType != TileType.Wall)
        {
            owner.UnavailableRange.Add(origin.MovedCoordinate(Directions.E, 1));
            if (showRangeIndicator) tile.SetUnavailable();
        }

        tile = dm.map.GetElementAt(origin.MovedCoordinate(Directions.S, 1));
        if (skillRange.Contains(Directions.S))
        {
            owner.AvailableRange.Add(origin.MovedCoordinate(Directions.S, 1));
            if (showRangeIndicator) tile.SetAvailable();
        }
        else if (tile.TileData.tileType != TileType.Wall)
        {
            owner.UnavailableRange.Add(origin.MovedCoordinate(Directions.S, 1));
            if (showRangeIndicator) tile.SetUnavailable();
        }

        tile = dm.map.GetElementAt(origin.MovedCoordinate(Directions.W, 1));
        if (skillRange.Contains(Directions.W))
        {
            Debug.Log("W");
            owner.AvailableRange.Add(origin.MovedCoordinate(Directions.W, 1));
            if (showRangeIndicator) tile.SetAvailable();
        }
        else if (tile.TileData.tileType != TileType.Wall)
        {
            owner.UnavailableRange.Add(origin.MovedCoordinate(Directions.W, 1));
            if (showRangeIndicator) tile.SetUnavailable();
        }
    }

    AttackData MakeAttackData(Unit owner)
    {
        AttackData ad = new(owner, (int)(owner.UnitData.atk.Total() * (EffectValues[0] / 100.0f)), 0, 0);
        return ad;
    }

    public override List<Coordinate> ApplyingTargets(Unit owner, Coordinate coord)
    {
        List<Coordinate> targets = new();
        Directions direction = (coord - owner.UnitData.coord).ToDirection();
        if(direction == Directions.N || direction == Directions.NE)
        {
            targets.Add(owner.UnitData.coord + new Coordinate(0, 1));
            targets.Add(owner.UnitData.coord + new Coordinate(-1, 1));
            targets.Add(owner.UnitData.coord + new Coordinate(1, 1));
        }
        else if (direction == Directions.E || direction == Directions.SE)
        {
            targets.Add(owner.UnitData.coord + new Coordinate(1, 0));
            targets.Add(owner.UnitData.coord + new Coordinate(1, 1));
            targets.Add(owner.UnitData.coord + new Coordinate(1, -1));
        }
        else if (direction == Directions.S || direction == Directions.SW)
        {
            targets.Add(owner.UnitData.coord + new Coordinate(0, -1));
            targets.Add(owner.UnitData.coord + new Coordinate(-1, -1));
            targets.Add(owner.UnitData.coord + new Coordinate(1, -1));
        }
        else if (direction == Directions.W || direction == Directions.NW)
        {
            targets.Add(owner.UnitData.coord + new Coordinate(-1, 0));
            targets.Add(owner.UnitData.coord + new Coordinate(-1, 1));
            targets.Add(owner.UnitData.coord + new Coordinate(-1, -1));
        }
        return targets;
    }
}
