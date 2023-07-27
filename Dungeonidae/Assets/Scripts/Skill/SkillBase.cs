using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillBase", menuName = "Scriptable Object/SkillBase")]
public abstract class SkillBase : ScriptableObject
{
    [field:SerializeField] public string Key { get; private set; }
    [field: SerializeField] public SkillType Type { get; private set; }
    [field: SerializeField] public int Cost { get; private set; }
    [field: SerializeField] public int Range { get; private set; }
    [field: SerializeField] public int[] EffectValues { get; private set; }
    [field: SerializeField] public bool NeedTarget { get; private set; }
    [field: SerializeField] public bool IgnoreBlock { get; private set; }
    [field: SerializeField] public bool TargetReachableTile { get; private set; }
    [field: SerializeField] public bool TargetDungeonObjects { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }

    public abstract IEnumerator Skill(Unit owner, DungeonManager dm, Coordinate coord);

    public void SetRange(Unit owner, DungeonManager dm, bool showRangeIndicator)
    {
        List<Coordinate> skillRange = new();
        int range = Range;
        if (range == 0) range = owner.UnitData.atkRange.Total();

        int wallMapSize = range * 2 + 1;
        NativeArray<bool> wallMap = new(wallMapSize * wallMapSize, Allocator.TempJob);
        Coordinate origin = new(owner.UnitData.coord.x - range, owner.UnitData.coord.y - range);
        for (int i = 0; i < wallMapSize; i++)
        {
            for (int j = 0; j < wallMapSize; j++)
            {
                wallMap[j + i * wallMapSize] = false;
                if (dm.IsValidIndexForMap(origin.x + i, origin.y + j))
                {
                    Tile tile = dm.map.GetElementAt(origin.x + i, origin.y + j);
                    if (IgnoreBlock)
                    {
                        if(tile.TileData.tileType == TileType.Wall)
                            wallMap[j + i * wallMapSize] = true;
                    }
                    else if (tile.IsBlockingSight() || tile.unit != null || !tile.IsReachableTile(false))
                    {
                        wallMap[j + i * wallMapSize] = true;
                    }
                }
            }
        }
        wallMap[range + range * wallMapSize] = false;
        List<Coordinate> inRange = GlobalMethods.RangeByStep(owner.UnitData.coord, range);
        NativeArray<Coordinate> end = new(inRange.Count, Allocator.TempJob);
        NativeArray<bool> result = new(inRange.Count, Allocator.TempJob);
        int endCounter = 0;
        for (int i = 0; i < inRange.Count; i++)
        {
            if (dm.IsValidCoordForMap(inRange[i]))
            {
                if (Coordinate.InRange(owner.UnitData.coord, inRange[i], range + 0.5f))
                {
                    end[endCounter] = (inRange[i] - origin);
                    endCounter++;
                }
            }
        }

        SightCheckJob2 sightJob = new()
        {
            wallMap = wallMap,
            wallMapSize = wallMapSize,
            start = owner.UnitData.coord - origin,
            end = end,
            result = result
        };

        JobHandle handle = sightJob.Schedule(endCounter, 1);
        handle.Complete();

        for (int i = 0; i < endCounter; i++)
        {
            if (result[i])
            {
                skillRange.Add(end[i] + origin);
            }
        }

        wallMap.Dispose();
        end.Dispose();
        result.Dispose();

        skillRange.Remove(owner.UnitData.coord);

        if (TargetReachableTile)
        {
            for (int i = 0; i < skillRange.Count; i++)
            {
                Tile tile = dm.map.GetElementAt(skillRange[i]);
                if (tile.IsReachableTile(false))
                {
                    owner.AvailableRange.Add(skillRange[i]);
                    if (showRangeIndicator) tile.SetAvailable();
                }
                else if (tile.TileData.tileType != TileType.Wall)
                {
                    owner.UnavailableRange.Add(skillRange[i]);
                    if (showRangeIndicator) tile.SetUnavailable();
                }
            }
        }
        else
        {
            for (int i = 0; i < skillRange.Count; i++)
            {
                Tile tile = dm.map.GetElementAt(skillRange[i]);
                if (tile.unit != null)
                {
                    owner.AvailableRange.Add(skillRange[i]);
                    if (showRangeIndicator) tile.SetAvailable();
                }
                else if (tile.TileData.tileType != TileType.Wall)
                {
                    owner.UnavailableRange.Add(skillRange[i]);
                    if (showRangeIndicator) tile.SetUnavailable();
                }
            }
            if (TargetDungeonObjects)
            {
                for (int i = 0; i < skillRange.Count; i++)
                {
                    Tile tile = dm.map.GetElementAt(skillRange[i]);
                    if (tile.ContainsTargetableDungeonObject() && !owner.AvailableRange.Contains(skillRange[i]))
                    {
                        owner.AvailableRange.Add(skillRange[i]);
                        owner.UnavailableRange.Remove(skillRange[i]);
                        if (showRangeIndicator) tile.SetAvailable();
                    }
                }
            }
        }
    }

    public virtual bool IsUseable(Unit owner)
    {
        if (owner.UnitData.Mp >= Cost) return true;
        else return false;
    }

    public virtual int[] GetListForDescription()
    {
        return EffectValues;
    }
}
