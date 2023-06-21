using Unity.Jobs;
using UnityEngine;
using Unity.Collections;

public struct SightCheckJob2 : IJobParallelFor
{
    [ReadOnly] public NativeArray<NativeArray<bool>> wallMap;
    [ReadOnly] public NativeArray<Vector2Int> end;
    public NativeArray<bool> result;

    public void Execute(int index)
    {
        Vector2Int start = new(0, 0);

        int deltaX = Mathf.Abs(end[index].x);
        int deltaY = Mathf.Abs(end[index].y);
        int stepX = start.x < end[index].x ? 1 : -1;
        int stepY = start.y < end[index].y ? 1 : - 1;
        int error = deltaX - deltaY;

        result[index] = true;
        while (start.x != end[index].x || start.y != end[index].y)
        {
            if (wallMap[start.x][start.x])
            {
                result[index] = false;
                break;
            }

            int error2 = error * 2;

            if (error2 > -deltaY)
            {
                error -= deltaY;
                start.x += stepX;
            }

            if (error2 < deltaX)
            {
                error += deltaX;
                start.y += stepY;
            }
        }
    }
}
