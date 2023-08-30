using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "BUFF_SKILL", menuName = "Scriptable Object/Skills/BuffSkill")]
public class BuffSkill : SkillBase
{
    [field: SerializeField] public int Duration { get; private set; }

    public override IEnumerator Skill(Unit owner, DungeonManager dm, Coordinate coord)
    {
        owner.UnitData.Mp -= Cost;
        if (owner.MySpriteRenderer.enabled)
        {
            owner.activeMotions++;
            owner.MyAnimator.SetBool("Attack", true);
            yield return Constants.ZeroPointZeroOne;
            float animationLength = owner.MyAnimator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationLength * 0.5f);
            owner.BuffDirector.ApplyBuff(new BuffData(Key, Duration));
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
            owner.BuffDirector.ApplyBuff(new BuffData(Key, Duration));
            owner.EndSkill(1);
        }
    }

    public override int[] GetListForDescription()
    {
        List<int> list = new()
        {
            Duration
        };
        list.AddRange(EffectValues);
        return list.ToArray();
    }
}
