using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Buff : Skill
{
    public Skill_Buff(Unit owner, DungeonManager dm, SkillData skillData) : base(owner, dm, skillData)
    {
    }

    public override void Prepare()
    {
        StartSkill(owner.Coord);
    }

    public override bool IsUsable()
    {
        if ((owner.UnitData.Mp >= SkillData.EffectValues[0]) && (SkillData.currentCoolDown == 0))
            return true;
        else return false;
    }

    public override void SetRange(bool showRange)
    {
        throw new System.NotImplementedException();
    }

    public override void StartSkill(Coordinate coord)
    {
        owner.UnitData.Mp -= SkillData.EffectValues[0];
        owner.MyAnimator.SetBool("Attack", true);
        owner.StartCoroutine(SkillEffect(coord));
    }

    protected override IEnumerator SkillEffect(Coordinate coord)
    {
        float animationLength = owner.MyAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength * 0.5f);
        owner.BuffDirector.ApplyBuff(new BuffData(GameManager.Instance.buffBaseDict[SkillData.Key], SkillData));
        while (!owner.isAnimationFinished)
        {
            yield return Constants.ZeroPointOne;
        }
        owner.isAnimationFinished = false;
        SkillData.currentCoolDown = SkillData.EffectValues[^1] + 1;
        owner.EndSkill(1);
    }

    public override void ShowTargetArea(Coordinate coord)
    {
        throw new System.NotImplementedException();
    }

    public override List<Coordinate> SetTargetArea(Coordinate coord)
    {
        throw new System.NotImplementedException();
    }

    public override AttackData MakeAttackData(Unit target)
    {
        throw new System.NotImplementedException();
    }

    public override Coordinate? SelectTargetAutomatically()
    {
        return null;
    }
}