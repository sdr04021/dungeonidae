using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Coordinate center;
    public int Top { get; private set; }
    public int Bottom { get; private set; }
    public int Right { get; private set; }
    public int Left { get; private set; }

    public int Width { get { return Right - Left; } }
    public int Height { get { return Top - Bottom; } }
    public int Area { get { return Width * Height; } }
    public bool finished = false;

    public Room(Coordinate center)
    {
        this.center = center;
        Top = center.y;
        Bottom = center.y;
        Right = center.x;
        Left = center.x;
    }

    public void Grow(int randomNumber, Tile[,] map)
    {
        switch (randomNumber % 5)
        {
            case 0:
                if (Top < map.GetLength(1) - 3)
                    Top += 1;
                break;
            case 1:
                if (Bottom > 2)
                    Bottom -= 1;
                break;
            case 2:
                if (Right < map.GetLength(0) - 3)
                    Right += 1;
                break;
            case 3:
                if (Left > 2)
                    Left -= 1;
                break;
            case 4:
                break;
        }
    }
    
    public int AreaSize()
    {
        return (Top - Bottom) * (Right - Left);
    }

    public void SetCenter()
    {
        center.x = (Left + Right) / 2;
        center.y = (Top + Bottom) / 2;
    }

    public bool IsAttached(Coordinate c)
    {
        if (c.x >= Left - 1 && c.x <= Right + 1)
        {
            if(c.y >= Bottom && c.y <= Top)
                return true;
        }


        if (c.y >= Bottom - 1 && c.y <= Top + 1)
        {
            if (c.x >= Left && c.x <= Right)
                return true;
        }

        return false;
    }

    public void Average(Room room)
    {
        Left = (room.Left + Left) / 2;
        Right = (room.Right + Right) / 2;
        Top = (room.Top + Top) / 2;
        Bottom = (room.Bottom + Bottom) / 2;
    }
}
