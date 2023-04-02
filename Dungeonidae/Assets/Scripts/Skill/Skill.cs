using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill
{
    protected Unit owner;
    protected DungeonManager dm;
    public SkillData SkillData { get; protected set; }

    public List<Coordinate> AvailableTilesInRange { get; protected set; } = new();
    public List<Coordinate> UnAvailableTilesInRange { get; protected set; } = new ();

    protected List<Coordinate> targetArea = new();

    protected bool showRange = false;

    public Skill(Unit owner, DungeonManager dm, SkillData skillData)
    {
        this.owner = owner;
        this.dm = dm;
        SkillData = skillData;
    }

    public abstract void Prepare();
    public abstract bool IsUsable();
    public abstract void SetRange(bool showRange);

    public void ResetTilesInRange()
    {
        if (showRange)
        {
            for (int i = 0; i < AvailableTilesInRange.Count; i++)
            {
                dm.GetTileByCoordinate(AvailableTilesInRange[i]).TurnOffRangeIndicator();
            }
            for (int i = 0; i < UnAvailableTilesInRange.Count; i++)
            {
                dm.GetTileByCoordinate(UnAvailableTilesInRange[i]).TurnOffRangeIndicator();
            }
        }

        AvailableTilesInRange.Clear();
        UnAvailableTilesInRange.Clear();
    }

    public abstract void StartSkill(Coordinate coord);

    protected abstract IEnumerator SkillEffect(Coordinate coord);

    public abstract void ShowTargetArea(Coordinate coord);

    public abstract List<Coordinate> SetTargetArea(Coordinate coord);

    public abstract Coordinate? SelectTargetAutomatically();

    public abstract AttackData MakeAttackData(Unit target);
}
