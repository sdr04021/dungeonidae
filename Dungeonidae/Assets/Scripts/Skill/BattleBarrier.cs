using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BATTLE_BARRIER", menuName = "Scriptable Object/Skills/BattleBarrier")]
public class BattleBarrier : SkillBase
{
    public override IEnumerator Skill(Unit owner, DungeonManager dm, Coordinate coord)
    {
        owner.activeMotions++;
        owner.UnitData.Mp -= Cost;
        Unit target = dm.GetTileByCoordinate(coord).unit;
        if (owner.MySpriteRenderer.enabled || target.MySpriteRenderer.enabled)
        {
            owner.MyAnimator.SetBool("Attack", true);
            Instantiate(GameManager.Instance.GetPrefab(PrefabAssetType.ParticleEffect, "BATTLE_BARRIER"), owner.Center);
            yield return Constants.ZeroPointZeroOne;
            float animationLength = owner.MyAnimator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationLength * 0.5f);
            owner.UnitData.Barrier += (int)(owner.UnitData.maxHp.Total() * 0.2f);
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
            owner.UnitData.Barrier += (int)(owner.UnitData.maxHp.Total() * 0.2f);
            owner.EndSkill(1);
        }
    }
}
