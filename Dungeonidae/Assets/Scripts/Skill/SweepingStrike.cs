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
            AnimationEffect effect = Instantiate(GameManager.Instance.AnimationEffectPrefab, owner.transform.position, Quaternion.identity);
            owner.MyAnimator.speed = 0.5f;
            Directions direction = (coord - owner.UnitData.coord).ToDirection();
            if (direction == Directions.E || direction == Directions.SE)
            {
                effect.transform.Rotate(new Vector3(0,0,-90));
            }
            else if (direction == Directions.S || direction == Directions.SW)
            {
                effect.transform.Rotate(new Vector3(0, 0, 180));
            }
            else if (direction == Directions.W || direction == Directions.NW)
            {
                effect.transform.Rotate(new Vector3(0, 0, 90));
            }

            effect.ShowEffect(Key, owner);
            owner.MyAnimator.SetBool("Attack", true);
            yield return Constants.ZeroPointZeroOne;
            float animationLength = owner.MyAnimator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationLength * 0.5f);
            AttackData ad = MakeAttackData(owner);
            for (int i=0; i<targets.Count; i++)
            {
                targets[i].GetDamage(ad);
            }
            owner.EndSkill(1);
            while ((owner.MyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) || targets[0].passiveMotions > 0)
            {
                yield return Constants.ZeroPointOne;
            }
            owner.MyAnimator.speed = 1f;
            owner.MyAnimator.SetBool("Attack", false);
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
