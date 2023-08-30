using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "POWER_STRIKE", menuName = "Scriptable Object/Skills/PowerStrike")]
public class PowerStrike : SkillBase
{
    public override IEnumerator Skill(Unit owner, DungeonManager dm, Coordinate coord)
    {
        owner.activeMotions++;
        owner.UnitData.Mp -= Cost;
        owner.FlipSprite(coord);
        Unit target = dm.GetTileByCoordinate(coord).unit;
        if (owner.MySpriteRenderer.enabled || target.MySpriteRenderer.enabled)
        {
            AnimationEffect effect = Instantiate(GameManager.Instance.AnimationEffectPrefab, owner.transform.position, Quaternion.identity);
            effect.ShowEffect(Key, owner);

            while ((effect != null) && (effect.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8f))
            {
                yield return Constants.ZeroPointZeroOne;
            }

            owner.MyAnimator.SetBool("Attack", true);
            yield return Constants.ZeroPointZeroOne;
            float animationLength = owner.MyAnimator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationLength * 0.5f);
            target.KnockBack((target.UnitData.coord - owner.UnitData.coord).ToDirection(), 3);
            target.GetDamage(MakeAttackData(owner));
            if (HitEffectPrefab != null)
            {
                Instantiate(HitEffectPrefab, target.Center);
            }
            while (target.passiveMotions > 0)
            {
                yield return Constants.ZeroPointOne;
            }
            owner.EndSkill(1);
            while ((owner.MyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1))
            {
                yield return Constants.ZeroPointOne;
            }
            owner.MyAnimator.SetBool("Attack", false);
            owner.activeMotions--;
        }
        else
        {
            target.GetDamage(MakeAttackData(owner));
            owner.EndSkill(1);
        }
    }

    AttackData MakeAttackData(Unit owner)
    {
        AttackData ad = new(owner, (int)(owner.UnitData.atk.Total() * (EffectValues[0] / 100.0f)), 0, 0);
        return ad;
    }
}
