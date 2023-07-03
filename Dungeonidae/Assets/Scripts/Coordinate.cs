using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Coordinate
{
    public int x;
    public int y;

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public Coordinate(Coordinate c)
    {
        x = c.x;
        y = c.y;
    }
    public Coordinate(Vector2 v)
    {
        x = (int)v.x;
        y = (int)v.y;
    }
    public Coordinate(Vector3 v)
    {
        x = (int)v.x;
        y = (int)v.y;
    }
    public Coordinate(Directions d)
    {
        switch (d)
        {
            case Directions.N:
                x = 0; y = 1;
                break;
            case Directions.NE:
                x = 1; y = 1;
                break;
            case Directions.E:
                x = 1; y = 0;
                break;
            case Directions.SE:
                x = 1; y = -1;
                break;
            case Directions.S:
                x = 0; y = -1;
                break;
            case Directions.SW:
                x = -1; y = -1;
                break;
            case Directions.W:
                x = -1; y = 0;
                break;
            case Directions.NW:
                x = -1; y = 1;
                break;
            default:
                x = 0; y = 0;
                break;
        }
    }

    public void Set(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public void Set(Coordinate c)
    {
        x = c.x;
        y = c.y;
    }
    public void Set(Directions d)
    {
        switch (d)
        {
            case Directions.N:
                x = 0; y = 1;
                break;
            case Directions.NE:
                x = 1; y = 1;
                break;
            case Directions.E:
                x = 1; y = 0;
                break;
            case Directions.SE:
                x = 1; y = -1;
                break;
            case Directions.S:
                x = 0; y = -1;
                break;
            case Directions.SW:
                x = -1; y = -1;
                break;
            case Directions.W:
                x = -1; y = 0;
                break;
            case Directions.NW:
                x = -1; y = 1;
                break;
        }
    }

    public void Translate(int x, int y)
    {
        this.x += x;
        this.y += y;
    }
    public void Translate(Coordinate c)
    {
        x += c.x;
        y += c.y;
    }
    public void Translate(Directions d)
    {
        switch (d)
        {
            case Directions.N:
                x = 0; y = 1;
                break;
            case Directions.NE:
                x = 1; y = 1;
                break;
            case Directions.E:
                x = 1; y = 0;
                break;
            case Directions.SE:
                x = 1; y = -1;
                break;
            case Directions.S:
                x = 0; y = -1;
                break;
            case Directions.SW:
                x = -1; y = -1;
                break;
            case Directions.W:
                x = -1; y = 0;
                break;
            case Directions.NW:
                x = -1; y = 1;
                break;
        }
    }

    public string CoordinateToString()
    {
        return x.ToString() + "," + y.ToString();
    }

    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }
    public Vector3 ToVector3(int z)
    {
        return new Vector3(x, y, z);
    }

    public Coordinate MovedCoordinate(int x, int y)
    {
        return new Coordinate(this.x + x, this.y + y);
    }
    public Coordinate MovedCoordinate(Directions direction, int amount)
    {
        return this + new Coordinate(direction) * amount;
    }

    public bool IsValidCoordForMap(List<List<TileData>> mapData)
    {
        if ((x >= 0 && x < mapData.Count) && (y >= 0 && y < mapData[0].Count))
            return true;
        else return false;
    }
    public bool IsValidCoordForMap(Tile[,] map)
    {
        if ((x >= 0 && x < map.GetLength(0)) && (y >= 0 && y < map.GetLength(1)))
            return true;
        else return false;
    }
    public bool IsValidCoordForMap(Arr2D<Tile> map)
    {
        if ((x >= 0 && x < map.arrSize.x) && (y >= 0 && y < map.arrSize.y))
            return true;
        else return false;
    }
    public bool IsTargetInRange(Coordinate target, int range)
    {
        if ((Mathf.Abs(x - target.x) <= range) && (Mathf.Abs(y - target.y) <= range))
            return true;
        else return false;
    }

    public float Magnitude()
    {
        return Mathf.Sqrt(x*x + y*y);
    }
    public static float Distance(Coordinate startPoint, Coordinate endPoint)
    {
        return Mathf.Sqrt((startPoint.x - endPoint.x)* (startPoint.x - endPoint.x) + (startPoint.y - endPoint.y)* (startPoint.y - endPoint.y));
    }
    public static bool InRange(Coordinate startPoint, Coordinate endPoint, int range)
    {
        return (((startPoint.x - endPoint.x) * (startPoint.x - endPoint.x) + (startPoint.y - endPoint.y) * (startPoint.y - endPoint.y)) <= (range * range));
    }
    public static bool InRange(Coordinate startPoint, Coordinate endPoint, float range)
    {
        return (((startPoint.x - endPoint.x) * (startPoint.x - endPoint.x) + (startPoint.y - endPoint.y) * (startPoint.y - endPoint.y)) <= (range * range));
    }

    public static Coordinate operator +(Coordinate a, Coordinate b)
        => new Coordinate(a.x + b.x, a.y + b.y);
    public static Coordinate operator -(Coordinate a, Coordinate b)
        => new Coordinate(a.x - b.x, a.y - b.y);
    public static Coordinate operator -(Coordinate c)
        => new Coordinate(-c.x, -c.y);
    public static Coordinate operator *(Coordinate a, int b)
        => new Coordinate(a.x * b, a.y * b);
    public static Coordinate operator /(Coordinate a, int b)
        => new Coordinate(a.x / b, a.y / b);
    public static Coordinate operator /(Coordinate a, Coordinate b)
        => new Coordinate(a.x / b.x, a.y / b.y);
    public static bool operator ==(Coordinate a, Coordinate b)
        => (a.x == b.x) && (a.y == b.y);
    public static bool operator !=(Coordinate a, Coordinate b)
        => (a.x != b.x) || (a.y != b.y);
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
