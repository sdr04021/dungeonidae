using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct SightCheckJob : IJobParallelFor
{
    [ReadOnly] public int blockCounter;
    [ReadOnly] public NativeArray<Vector2> blockInSight;
    [ReadOnly] public Vector2 start;
    [ReadOnly] public int endCounter;
    [ReadOnly] public NativeArray<Vector2> end;
    public NativeArray<bool> result;

    public void Execute(int index)
    {
        result[index] = true;
        for(int i=0; i<blockCounter; i++)
        {
            if (end[index] == blockInSight[i]) return;

            Vector2[] points = new Vector2[4];
            points[0] = new Vector2(blockInSight[i].x + 0.495f, blockInSight[i].y + 0.495f);
            points[1] = new Vector2(blockInSight[i].x + 0.495f, blockInSight[i].y - 0.495f);
            points[2] = new Vector2(blockInSight[i].x - 0.495f, blockInSight[i].y - 0.495f);
            points[3] = new Vector2(blockInSight[i].x - 0.495f, blockInSight[i].y + 0.495f);

            if (DoIntersect(start, end[index], points[0], points[1]) ||
            DoIntersect(start, end[index], points[1], points[2]) ||
            DoIntersect(start, end[index], points[2], points[3]) ||
            DoIntersect(start, end[index], points[3], points[0]))
            {
                result[index] = false;
                break;
            }
        }
    }

    bool OnSegment(Vector2 p, Vector2 q, Vector2 r)
    {
        if (q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) &&
            q.y <= Mathf.Max(p.y, r.y) && q.y >= Mathf.Min(p.y, r.y))
            return true;
        return false;
    }

    int Orientation(Vector2 p, Vector2 q, Vector2 r)
    {
        float val = (q.y - p.y) * (r.x - q.x) -
                (q.x - p.x) * (r.y - q.y);

        if (val == 0) return 0;

        return (val > 0) ? 1 : 2;
    }

    bool DoIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
    {
        float o1 = Orientation(p1, q1, p2);
        float o2 = Orientation(p1, q1, q2);
        float o3 = Orientation(p2, q2, p1);
        float o4 = Orientation(p2, q2, q1);

        if (o1 != o2 && o3 != o4)
            return true;

        if (o1 == 0 && OnSegment(p1, p2, q1)) return true;
        if (o2 == 0 && OnSegment(p1, q2, q1)) return true;
        if (o3 == 0 && OnSegment(p2, p1, q2)) return true;
        if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

        return false;
    }
}
