using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "INTERACTION", menuName = "Scriptable Object/Skills/Interaction")]

public class Interaction : SkillBase
{
    public override IEnumerator Skill(Unit owner, DungeonManager dm, Coordinate coord)
    {
        owner.isAnimationFinished = false;
        Tile tile = dm.GetTileByCoordinate(coord);
        owner.FlipSprite(coord);
        if (tile.GetTargetable() != null)
        {
            DungeonObject dunObj = tile.GetTargetable();
            if (dunObj.Durability == DungeonObjectDurability.Fragile)
            {
                owner.MyAnimator.SetBool("Attack", true);
                float animationLength = owner.MyAnimator.GetCurrentAnimatorStateInfo(0).length;
                yield return new WaitForSeconds(animationLength * 0.5f);
                tile.GetTargetable().Activate(owner);
                owner.EndSkill(1);
                while ((owner.MyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1))
                {
                    yield return Constants.ZeroPointOne;
                }
                owner.MyAnimator.SetBool("Attack", false);
                owner.isAnimationFinished = true;
            }
            else
            {
                dunObj.Activate(owner);
                owner.EndSkill(1);
                owner.isAnimationFinished = true;
            }
        }
    }
}
