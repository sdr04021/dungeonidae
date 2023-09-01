using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RUSH", menuName = "Scriptable Object/Skills/Rush")]
public class Rush : SkillBase
{
    public override IEnumerator Skill(Unit owner, DungeonManager dm, Coordinate coord)
    {
        owner.UnitData.Mp -= Cost;
        owner.FlipSprite(coord);
        bool moving = true;
        if (owner.MySpriteRenderer.enabled)
        {
            owner.activeMotions++;
            Sequence rushSequence = DOTween.Sequence().Pause();
            Coordinate destination = owner.UnitData.coord;
            Coordinate delta = coord - owner.UnitData.coord;
            int amount = Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
            Coordinate step = new(System.Math.Sign(delta.x), System.Math.Sign(delta.y));
            for(int i = 1; i <= amount; i++)
            {
                destination += step;
                rushSequence.Append(owner.transform.DOMove(destination.ToVector2(), 0.12f).OnComplete(() =>
                {
                    if (dm.map.GetElementAt(new Coordinate(owner.transform.position)).HasCollideable())
                    {
                        dm.map.GetElementAt(new Coordinate(owner.transform.position)).GetCollideable().Activate(owner);
                    }
                })).SetEase(Ease.Linear);
            }
            rushSequence.AppendCallback(() => moving = false);
            rushSequence.Play();

            while (moving)
            {
                yield return Constants.ZeroPointOne;
            }
            dm.map.GetElementAt(owner.UnitData.coord).unit = null;
            owner.UnitData.coord = coord;
            owner.SetPosition();
            owner.SetSortingOrder();
            owner.CheckSightArea();
            owner.EndSkill(1);
            owner.activeMotions--;
        }
        else
        {
            owner.SetPosition();
            owner.SetSortingOrder();
            owner.EndSkill(1);
        }
    }

    public override int[] GetListForDescription()
    {
        return new int[] { Range };
    }
}
