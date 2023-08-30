using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "COMBO_ATTACK", menuName = "Scriptable Object/Skills/ComboAttack")]
public class ComboAttack : SkillBase
{
    public override IEnumerator Skill(Unit owner, DungeonManager dm, Coordinate coord)
    {
        owner.activeMotions++;
        owner.UnitData.ConsumeBuffStack("COMBO", 1);

        owner.FlipSprite(coord);
        Unit target = dm.GetTileByCoordinate(coord).unit;
        if (owner.MySpriteRenderer.enabled || target.MySpriteRenderer.enabled)
        {
            AnimationEffect effect = Instantiate(GameManager.Instance.AnimationEffectPrefab, owner.transform.position, Quaternion.identity);
            effect.ShowEffect(Key, owner);
            owner.MyAnimator.SetBool("Attack", true);
            yield return Constants.ZeroPointZeroOne;
            float animationLength = owner.MyAnimator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationLength * 0.5f);
            target.GetDamage(MakeAttackData(owner));
            owner.EndSkill(1);
            while ((owner.MyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) || target.passiveMotions > 0)
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
        AttackData ad = new(owner, (int)(owner.UnitData.atk.Total() * (EffectValues[1] / 100.0f)), 0, 0);
        return ad;
    }

    public override bool IsUseable(Unit owner)
    {
        return owner.UnitData.ContainsBuffStack("COMBO", EffectValues[0]);
    }
}
