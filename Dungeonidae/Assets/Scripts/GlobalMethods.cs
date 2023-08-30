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
    public static List<Coordinate> RangeByEightDirections(Coordinate startPoint, int range)
    {
        List<Coordinate> list = new()
        {
            startPoint
        };

        for (int i = 1; i <= range; i++)
        {
            list.Add(startPoint + (new Coordinate(0, 1) * i));
            list.Add(startPoint + (new Coordinate(1, 1) * i));
            list.Add(startPoint + (new Coordinate(1, 0) * i));
            list.Add(startPoint + (new Coordinate(1, -1) * i));
            list.Add(startPoint + (new Coordinate(0, -1) * i));
            list.Add(startPoint + (new Coordinate(-1, -1) * i));
            list.Add(startPoint + (new Coordinate(-1, 0) * i));
            list.Add(startPoint + (new Coordinate(-1, 1) * i));
        }

        return list;
    }

    public static IEnumerable<int> AddArrays(IEnumerable<int> original, IEnumerable<int> additional)
    {
        var e1 = original.GetEnumerator();
        var e2 = additional.GetEnumerator();

        while (e1.MoveNext() && e2.MoveNext())
            yield return e1.Current + e2.Current;
    }
    public static IEnumerable<int> AddArrays(IEnumerable<int> original, IEnumerable<int> additional, IEnumerable<int> bonus)
    {
        var e1 = original.GetEnumerator();
        var e2 = additional.GetEnumerator();
        var e3 = bonus.GetEnumerator();

        while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
            yield return e1.Current + e2.Current + e3.Current;
    }
    public static IEnumerable<int> SubstractArrays(IEnumerable<int> a1, IEnumerable<int> a2)
    {
        var e1 = a1.GetEnumerator();
        var e2 = a2.GetEnumerator();

        while (e1.MoveNext() && e2.MoveNext())
            yield return e1.Current - e2.Current;
    }
    public static IEnumerable<int> SubstractArrays(IEnumerable<int> original, IEnumerable<int> additional, IEnumerable<int> bonus)
    {
        var e1 = original.GetEnumerator();
        var e2 = additional.GetEnumerator();
        var e3 = bonus.GetEnumerator();

        while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
            yield return e1.Current - e2.Current - e3.Current;
    }
}
