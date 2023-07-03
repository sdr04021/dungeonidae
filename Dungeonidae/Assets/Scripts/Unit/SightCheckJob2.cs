using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;

public struct SightCheckJob2 : IJobParallelFor
{
    [ReadOnly] public NativeArray<bool> wallMap;
    [ReadOnly] public int wallMapSize;
    [ReadOnly] public Coordinate start;
    [ReadOnly] public NativeArray<Coordinate> end;
    public NativeArray<bool> result;

    Coordinate delta;

    public void Execute(int index)
    {
        delta = new Coordinate(end[index].x - start.x, end[index].y - start.y);  
        int step = Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y));

        Coordinate stepA = new(step * (int)Mathf.Sign(delta.x), 0);
        Coordinate stepB = new(0, step * (int)Mathf.Sign(delta.y));
        Coordinate stepC = stepA + stepB;

        Coordinate current = new(0, 0);
        
        result[index] = true;
        while (current.x != delta.x * step || current.y != delta.y * step)
        {
            if (wallMap[(start.x + current.x / step) * wallMapSize + (start.y + current.y / step)])
            {
                result[index] = false;
                break;
            }

            Coordinate a = current + stepA;
            Coordinate b = current + stepB;
            int ia = GetInterval(a);
            int ib = GetInterval(b);
            if (ia < ib)
            {
                current = a;
            }
            else if (ia == ib)
            {
                current += stepC;
            }
            else
            {
                current = b;
            }
        }
    }

    int GetInterval(Coordinate point)
    {
        int a = (delta.x == 0) ? int.MaxValue : Mathf.Abs(point.y - (point.x / delta.x) * delta.y);

        int b = (delta.y == 0) ? int.MaxValue : Mathf.Abs(point.x - (point.y / delta.y) * delta.x);
        return (a < b) ? a : b;
    }
}
