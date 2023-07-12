using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CRITICAL_BLOW", menuName = "Scriptable Object/Skills/CriticalBlow")]
public class CriticalBlow : SkillBase
{
    public override IEnumerator Skill(Unit owner, DungeonManager dm, Coordinate coord)
    {
        owner.isAnimationFinished = false;
        owner.UnitData.Mp -= Cost;
        owner.FlipSprite(coord);
        Unit target = dm.GetTileByCoordinate(coord).unit;
        if (owner.MySpriteRenderer.enabled || target.MySpriteRenderer.enabled)
        {
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

    AttackData MakeAttackData(Unit owner)
    {
        AttackData ad = new(owner, owner.UnitData.atk.Total(), 0, 0, true);
        return ad;
    }
}
