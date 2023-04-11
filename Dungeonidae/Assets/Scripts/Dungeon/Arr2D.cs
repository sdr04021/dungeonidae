using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Arr2DRow<T>
{
    public List<T> y = new();
}


[System.Serializable]
public class Arr2D<T>
{
    public List<Arr2DRow<T>> x = new();
    public Coordinate arrSize = new();

    public T GetElementAt(int x, int y)
    {
        if (this.x.Count > 0)
        {
            if (this.x[x].y.Count > 0)
            {
                return this.x[x].y[y];
            }
        }
        return default;
    }

    public T GetElementAt(Coordinate c)
    {
        if (x.Count > 0)
        {
            if (x[c.x].y.Count > 0)
            {
                return x[c.x].y[c.y];
            }
        }
        return default;
    }
}
