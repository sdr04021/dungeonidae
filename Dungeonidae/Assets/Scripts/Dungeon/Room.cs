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
    public int GrowthCount { get; private set; }

    public List<Coordinate> Entrances { get; private set; } = new();

    public Room(Coordinate center, int growthCount)
    {
        this.center = center;
        Top = center.y;
        Bottom = center.y;
        Right = center.x;
        Left = center.x;
        GrowthCount = growthCount;
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
        GrowthCount--;
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
        GrowthCount += room.GrowthCount;
    }

    public void FinishGrowth()
    {
        GrowthCount = 0;
    }

    public void ExcludeBorder()
    {
        Top -= 1;
        Bottom += 1;
        Left += 1;
        Right -= 1;
        SetCenter();
    }

    public void SetEntranceCandidates(Tile[,] map)
    {
        Entrances.Add(new Coordinate(Random.Range(Left, Right + 1), Bottom - 1));
        Entrances.Add(new Coordinate(Random.Range(Left, Right + 1), Top + 1));
        Entrances.Add(new Coordinate(Left - 1, Random.Range(Bottom, Top + 1)));
        Entrances.Add(new Coordinate(Right + 1, Random.Range(Bottom, Top + 1)));
        for(int i=0; i<Entrances.Count; i++)
        {
            map[Entrances[i].x, Entrances[i].y].SetAreaType(AreaType.None);
        }
    }

    public void SetBorderAndEntrance(Tile[,] map)
    {
        Coordinate navigation = new(Left - 1, Bottom - 1);
        for (int i = 0; i < Height + 2; i++)
        {
            map[navigation.x, navigation.y + i].SetAreaType(AreaType.Border);
            map[navigation.x + Width + 1, navigation.y + i].SetAreaType(AreaType.Border);
            map[navigation.x, navigation.y + i].SetTileType(TileType.Wall);
            map[navigation.x + Width + 1, navigation.y + i].SetTileType(TileType.Wall);
        }
        navigation = new(Left, Bottom - 1);
        for (int i = 0; i < Width; i++)
        {
            map[navigation.x + i, navigation.y].SetAreaType(AreaType.Border);
            map[navigation.x + i, navigation.y + Height + 1].SetAreaType(AreaType.Border);
            map[navigation.x + i, navigation.y].SetTileType(TileType.Wall);
            map[navigation.x + i, navigation.y + Height + 1].SetTileType(TileType.Wall);
        }
        map[Random.Range(Left, Right + 1), Bottom - 1].SetAreaType(AreaType.None);
        //map[Random.Range(Left, Right + 1), Bottom - 1].SetTileType(TileType.Floor);
        map[Random.Range(Left, Right + 1), Top + 1].SetAreaType(AreaType.None);
        //map[Random.Range(Left, Right + 1), Top + 1].SetTileType(TileType.Floor);
        map[Left - 1, Random.Range(Bottom, Top + 1)].SetAreaType(AreaType.None);
        //map[Left - 1, Random.Range(Bottom, Top + 1)].SetTileType(TileType.Floor);
        map[Right + 1, Random.Range(Bottom, Top + 1)].SetAreaType(AreaType.None);
        //map[Right + 1, Random.Range(Bottom, Top + 1)].SetTileType(TileType.Floor);
    }
}
