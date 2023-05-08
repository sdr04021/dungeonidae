using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMethods
{
    public static List<Coordinate> RangeByStep(Coordinate startPoint, int range)
    {
        List<Coordinate> list = new();
        for (int i = 0; i <= range; i++)
        {
            int x = i;
            int y = i;
            if (i == 0)
            {
                list.Add(startPoint.MovedCoordinate(x, y));
                continue;
            }
            for (int j = 0; j < (2 * i); j++)
            {
                y -= 1;
                list.Add(startPoint.MovedCoordinate(x, y));
            }
            for (int j = 0; j < (2 * i); j++)
            {
                x -= 1;
                list.Add(startPoint.MovedCoordinate(x, y));
            }
            for (int j = 0; j < (2 * i); j++)
            {
                y += 1;
                list.Add(startPoint.MovedCoordinate(x, y));
            }
            for (int j = 0; j < (2 * i); j++)
            {
                x += 1;
                list.Add(startPoint.MovedCoordinate(x, y));
            }
        }
        return list;
    }
}
