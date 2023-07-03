using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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
    readonly bool[] growthRestriction = { false, false, false, false };
    public bool FinishedGrowth { get; private set; } = false;
    
    public List<Coordinate> Entrances { get; private set; } = new();

    public Room(Coordinate center)
    {
        this.center = center;
        Top = center.y;
        Bottom = center.y;
        Right = center.x;
        Left = center.x;
    }

    public Room(int top,int bottom, int right,int left)
    {
        Top = top;
        Bottom = bottom;
        Right = right;
        Left = left;
        SetCenter();
    }
    public void SetSquare(int x, List<List<TileData>> mapData)
    {
        Top += Mathf.Min(x, mapData[0].Count - 3);
        Bottom -= Mathf.Max(x, 2);
        Right += Mathf.Min(x, mapData.Count - 3);
        Left -= Mathf.Max(x, 2);

        for (int i = Left; i <= Right; i++)
        {
            for (int j = Bottom; j <= Top; j++)
            {
                mapData[i][j].areaType = AreaType.Room;
            }
        }
    }

    public void Grow_Legacy(int randomNumber, List<List<TileData>> mapData)
    {
        switch (randomNumber % 5)
        {
            case 0:
                if (Top < mapData[0].Count - 3)
                    Top += 1;
                break;
            case 1:
                if (Bottom > 2)
                    Bottom -= 1;
                break;
            case 2:
                if (Right < mapData.Count - 3)
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

    public void Grow(System.Random rand, List<List<TileData>> mapData)
    {
        if (Width > 10 || Height > 10)
        {
            FinishedGrowth = true;
            return;
        }

        int randomNumber;
        List<int> deck = new() { 0, 1, 2, 3 };
        if (growthRestriction[0]) deck.Remove(0);
        if (growthRestriction[1]) deck.Remove(1);
        if (growthRestriction[2]) deck.Remove(2);
        if (growthRestriction[3]) deck.Remove(3);

        if (deck.Count == 0) return;
        else randomNumber = deck[rand.Next(0, deck.Count)];

        switch (randomNumber % 4)
        {
            case 0:
                if (Top < mapData[0].Count - 4)
                {
                    for(int i=Left; i<=Right; i++)
                    {
                        if (mapData[i][Top+2].areaType == AreaType.Room)
                        {
                            growthRestriction[0] = true;
                            break;
                        }
                    }
                    if (!growthRestriction[0])
                    {
                        Top += 1;
                        for (int i = Left; i <= Right; i++)
                            mapData[i][Top].areaType = AreaType.Room;
                    }
                }
                break;
            case 1:
                if (Bottom > 4)
                {
                    for (int i = Left; i <= Right; i++)
                    {
                        if (mapData[i][Bottom - 2].areaType == AreaType.Room)
                        {
                            growthRestriction[1] = true;
                            break;
                        }
                    }
                    if (!growthRestriction[1])
                    {
                        Bottom -= 1;
                        for (int i = Left; i <= Right; i++)
                            mapData[i][Bottom].areaType = AreaType.Room;
                    }
                }
                break;
            case 2:
                if (Right < mapData.Count - 4)
                {
                    for (int i = Bottom; i <= Top; i++)
                    {
                        if (mapData[Right + 2][i].areaType == AreaType.Room)
                        {
                            growthRestriction[2] = true;
                            break;
                        }
                    }
                    if (!growthRestriction[2])
                    {
                        Right += 1;
                        for (int i = Bottom; i <= Top; i++)
                            mapData[Right][i].areaType = AreaType.Room;
                    }

                }
                break;
            case 3:
                if (Left > 4)
                {
                    for (int i = Bottom; i <= Top; i++)
                    {
                        if (mapData[Left - 2][i].areaType == AreaType.Room)
                        {
                            growthRestriction[3] = true;
                            break;
                        }
                    }
                    if (!growthRestriction[3])
                    {
                        Left -= 1;
                        for (int i = Bottom; i <= Top; i++)
                            mapData[Left][i].areaType = AreaType.Room;
                    }
                }
                break;
        }
        int count = 0;
        for(int i=0; i<growthRestriction.Length; i++)
        {
            if (growthRestriction[i]) count++;
        }
        if(count>=2) FinishedGrowth = true;
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
        GrowthCount = room.GrowthCount + GrowthCount;
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

    public void SetEntranceCandidates(List<List<TileData>> mapData, System.Random random)
    {
        Entrances.Add(new Coordinate(random.Next(Left, Right + 1), Bottom - 1));
        Entrances.Add(new Coordinate(random.Next(Left, Right + 1), Top + 1));
        Entrances.Add(new Coordinate(Left - 1, random.Next(Bottom, Top + 1)));
        Entrances.Add(new Coordinate(Right + 1, random.Next(Bottom, Top + 1)));
        for(int i=0; i<Entrances.Count; i++)
        {
            mapData[Entrances[i].x][Entrances[i].y].areaType = AreaType.None;
        }
    }
    public void LeaveOnlyOneEntranceCandidate(System.Random random)
    {
        Coordinate left = Entrances[random.Next(0, Entrances.Count)];
        Entrances.Clear();
        Entrances.Add(left);
    }

    public Coordinate PickRandomCordinate()
    {
        return new Coordinate(Random.Range(Left, Right + 1), Random.Range(Bottom, Top + 1));
    }
}
