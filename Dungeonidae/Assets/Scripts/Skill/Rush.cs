using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RUSH", menuName = "Scriptable Object/Skills/Rush")]
public class Rush : SkillBase
{
    public override IEnumerator Skill(Unit owner, DungeonManager dm, Coordinate coord)
    {
        float dist = Coordinate.Distance(owner.UnitData.coord, coord);
        owner.isAnimationFinished = false;
        owner.UnitData.Mp -= Cost;
        owner.FlipSprite(coord);
        dm.map.GetElementAt(owner.UnitData.coord).unit = null;
        owner.UnitData.coord = coord;   
        bool moving = true;
        if (owner.MySpriteRenderer.enabled)
        {
            owner.transform.DOMove(coord.ToVector2(), 0.1f * dist).OnComplete(() => { moving = false; });
            while (moving)
            {
                yield return Constants.ZeroPointOne;
            }
            owner.SetPosition();
            owner.UpdateSightArea();
            owner.EndSkill(1);
            owner.isAnimationFinished = true;
        }
        else
        {
            owner.SetPosition();
            owner.EndSkill(1);
        }
    }

    public override int[] GetListForDescription()
    {
        return new int[] { Range };
    }
}
