using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BASIC_ATTACK", menuName = "Scriptable Object/Skills/BasicAttack")]
public class BasicAttack : SkillBase
{
    public override IEnumerator Skill(Unit owner, DungeonManager dm, Coordinate coord)
    {
        owner.isAnimationFinished = false;
        Tile tile = dm.GetTileByCoordinate(coord);
        owner.FlipSprite(coord);
        if (tile.unit != null)
        {
            Unit target = dm.GetTileByCoordinate(coord).unit;
            if (owner.MySpriteRenderer.enabled || target.MySpriteRenderer.enabled)
            {
                owner.MyAnimator.SetBool("Attack", false);
                owner.MyAnimator.SetBool("Attack", true);
                float animationLength = owner.MyAnimator.GetCurrentAnimatorStateInfo(0).length;
                yield return new WaitForSeconds(animationLength * 0.5f);
                target.GetDamage(MakeAttackData(owner));
                owner.EndSkill(1);
                while ((owner.MyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) || !target.isHitFinished)
                {
                    yield return Constants.ZeroPointOne;
                }
                owner.MyAnimator.SetBool("Attack", false);
                owner.isAnimationFinished = true;
            }
            else
            {
                target.GetDamage(MakeAttackData(owner));
                owner.EndSkill(1);
            }
        }
        else if (tile.GetTargetable() != null)
        {
            DungeonObject dunObj = tile.GetTargetable();
            if (dunObj.Durability == DungeonObjectDurability.Fragile)
            {
                owner.MyAnimator.SetBool("Attack", false);
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

    AttackData MakeAttackData(Unit owner)
    {
        AttackData ad = new(owner, owner.UnitData.atk.Total(), 0, 0);
        return ad;
    }
}
