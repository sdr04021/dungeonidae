using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill
{
    protected Unit owner;
    protected DungeonManager dm;
    public SkillData skillData { get; protected set; }

    public List<Coordinate> availableTilesInRange = new();
    public List<Coordinate> unAvailableTilesInRange = new();

    public Skill(Unit owner, DungeonManager dm, SkillData skillData)
    {
        this.owner = owner;
        this.dm = dm;
        this.skillData = skillData;
    }

    public abstract void Prepare();
    public abstract bool IsUsable();
    public abstract void SetRange();

    public abstract void ResetTilesInRange();

    public abstract void StartSkill(Coordinate coord);

    protected abstract IEnumerator SkillEffect(Unit target);

    public abstract void ShowTargetArea(Coordinate coord);

    public abstract List<Coordinate> SetTargetArea(Coordinate coord);

    public abstract AttackData MakeAttackData(Unit target);
}
