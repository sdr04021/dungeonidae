using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DelaunatorSharp;
using Priority_Queue;
using Gists;

public class MapGenerator
{
    int mapWidth = 40;
    int mapHeight = 40;

    DungeonData dungeonData;
    System.Random rand;


    public MapGenerator(DungeonData dungeonData)
    {
        //generationArea = Mathf.Min(100, generationArea);
        //roomSeedAmount = generationArea * 6;
        this.dungeonData = dungeonData;
        dungeonData.mapData = new();
        dungeonData.fogData = new();
        dungeonData.rooms = new();
        rand = new System.Random(GameManager.Instance.saveData.FloorSeeds[dungeonData.floor]);
        GenereateBase();
        GenerateRooms();
        ConnectRooms();
        SetRoomElements();
    }

    void GenereateBase()
    {
        for (int i = 0; i < mapWidth; i++)
        {
            List<TileData> temp = new();
            List<FogData> tempFog = new();
            for (int j = 0; j < mapHeight; j++)
            {
                temp.Add(new TileData(rand.Next()));
                tempFog.Add(new FogData());
            }
            dungeonData.mapData.Add(temp);
            dungeonData.fogData.Add(tempFog);
        }
    }

    void GenerateRooms()
    {
        List<Vector2> poissonPoints = FastPoissonDiskSampling.Sampling(new Vector2(5, 5), new Vector2(mapWidth - 5, mapHeight - 5), 8, rand);
        for (int i = 0; i < poissonPoints.Count; i++)
        {
            dungeonData.rooms.Add(new Room(new Coordinate(poissonPoints[i])));
        }


        for (int i = dungeonData.rooms.Count - 1; i >= 0; i--)
        {
            dungeonData.rooms[i].SetSquare(2, dungeonData.mapData);
            dungeonData.rooms[i].SetCenter();
            if ((dungeonData.rooms[i].Width < 3) || (dungeonData.rooms[i].Height < 3))
                dungeonData.rooms.RemoveAt(i);
        }
        int count = 0;
        while (true)
        {
            int finished = 0;
            for (int i = 0; i < dungeonData.rooms.Count; i++)
            {
                if (dungeonData.rooms[i].FinishedGrowth)
                    finished++;
                else
                    dungeonData.rooms[i].Grow(rand, dungeonData.mapData);
            }
            if (finished == dungeonData.rooms.Count)
                break;
            count++;
            if (count >= 10000000) break;
        }


        for (int i = 0; i < dungeonData.rooms.Count; i++)
        {
            for (int j = dungeonData.rooms[i].Left; j <= dungeonData.rooms[i].Right; j++)
            {
                for (int k = dungeonData.rooms[i].Bottom; k <= dungeonData.rooms[i].Top; k++)
                {
                    dungeonData.mapData[j][k].tileType = TileType.Wall;
                    dungeonData.mapData[j][k].areaType = AreaType.Border;
                }
            }
            dungeonData.rooms[i].ExcludeBorder();
            for (int j = dungeonData.rooms[i].Left; j <= dungeonData.rooms[i].Right; j++)
            {
                for (int k = dungeonData.rooms[i].Bottom; k <= dungeonData.rooms[i].Top; k++)
                {
                    dungeonData.mapData[j][k].tileType = TileType.Floor;
                    dungeonData.mapData[j][k].areaType = AreaType.Room;
                }
            }
            dungeonData.rooms[i].SetEntranceCandidates(dungeonData.mapData, rand);
            //if (i == 0) dungeonData.rooms[i].LeaveOnlyOneEntranceCandidate(rand);
        }

        for (int i = 0; i < dungeonData.rooms.Count; i++)
        {
            dungeonData.rooms[i].SetCenter();
        }


        Vector2 meanCenter = Vector2.zero;
        for (int i = 0; i < dungeonData.rooms.Count; i++)
        {
            meanCenter += dungeonData.rooms[i].center.ToVector2();
        }
        meanCenter /= dungeonData.rooms.Count;
        SimplePriorityQueue<Room> roomsByDist = new();
        for (int i = 0; i < dungeonData.rooms.Count; i++)
        {
            roomsByDist.Enqueue(dungeonData.rooms[i], -Vector2.Distance(meanCenter, dungeonData.rooms[i].center.ToVector2()));
        }

        int amountToRemove = rand.Next(Mathf.Max(0, dungeonData.rooms.Count - 12), Mathf.Max(1, dungeonData.rooms.Count - 8));
        while (amountToRemove > 0)
        {
            if (dungeonData.rooms.Count <= 3) break;
            dungeonData.rooms.Remove(roomsByDist.Dequeue());
            amountToRemove--;
        }

        /*
        for (int i = 1; i < rooms.Count; i++)
        {
            int dice = Random.Range(0, 10);
            if (dice == 0)
                SetBigPillarRoom(rooms[i]);
            else if (dice == 1)
                SetPillarRoom(rooms[i]);
        }
        */

        /*
        for(int i=0; i<rooms.Count; i++)
        {
            rooms[i].SetBorderAndEntrance(map);
            //map[rooms[i].center.x, rooms[i].center.y].SetTileType(TileType.Wall);
        }
        */

        /*
        for(int i=0; i<map.GetLength(0); i++)
        {
            for(int j=0; j<map.GetLength(0); j++)
            {
                if (map[i,j].Area == AreaType.Border)
                {
                    map[i,j].gameObject.SetActive(false);
                }
            }
        }
        */
    }

    void ConnectRooms()
    {
        int[,] graph = new int[dungeonData.rooms.Count, dungeonData.rooms.Count];
        Dictionary<Coordinate, int> roomIndexDict = new();
        for (int i = 0; i < dungeonData.rooms.Count; i++)
        {
            roomIndexDict.Add(dungeonData.rooms[i].center, i);
            for (int j = 0; j < dungeonData.rooms.Count; j++)
            {
                graph[i, j] = -1;
            }
        }

        IPoint[] points = new IPoint[dungeonData.rooms.Count];
        for (int i = 0; i < dungeonData.rooms.Count; i++)
        {
            points[i] = new Point(dungeonData.rooms[i].center.x, dungeonData.rooms[i].center.y);
        }
        Delaunator delaunator = new Delaunator(points);
        IEnumerable<IEdge> iEdges = delaunator.GetEdges();
        foreach (IEdge item in iEdges)
        {
            Coordinate P = new((int)item.P.X, (int)item.P.Y);
            Coordinate Q = new((int)item.Q.X, (int)item.Q.Y);

            int dist = (int)Coordinate.Distance(P, Q);
            graph[roomIndexDict[P], roomIndexDict[Q]] = dist;
            graph[roomIndexDict[Q], roomIndexDict[P]] = dist;
        }

        SimplePriorityQueue<Coordinate> pQueue = new();
        int current = 0;
        int visited = 1;
        bool[] visitFlag = new bool[dungeonData.rooms.Count];
        System.Array.Fill(visitFlag, false);
        visitFlag[current] = true;
        List<Coordinate> hallways = new();

        while (visited < dungeonData.rooms.Count)
        {
            for (int i = 0; i < dungeonData.rooms.Count; i++)
            {
                if ((graph[current, i] > 0) && !visitFlag[i])
                {
                    pQueue.Enqueue(new Coordinate(current, i), graph[current, i]);
                }
            }
            while (pQueue.Count > 0)
            {
                Coordinate shortestRoute = pQueue.Dequeue();
                if (!visitFlag[shortestRoute.y])
                {
                    hallways.Add(shortestRoute);
                    current = shortestRoute.y;
                    visitFlag[current] = true;
                    visited++;
                    break;
                }
            }
        }

        List<Coordinate> additional = new();
        for (int i = 0; i < hallways.Count; i++)
        {
            graph[hallways[i].x, hallways[i].y] = -1;
            graph[hallways[i].y, hallways[i].x] = -1;
        }
        for (int i = 0; i < graph.GetLength(0) - 1; i++)
        {
            for (int j = i + 1; j < graph.GetLength(1); j++)
            {
                if (graph[i, j] > 0)
                    additional.Add(new Coordinate(i, j));
            }
        }
        int pickAmount = 1 + (hallways.Count / 4);
        for (int i = 0; i < pickAmount; i++)
        {
            int pick = rand.Next(0, additional.Count);
            hallways.Add(additional[pick]);
            additional.RemoveAt(pick);
        }

        /*
        for(int i=0; i<hallways.Count; i++)
        {
            Debug.Log(hallways[i].CoordinateToString());
        }
        */

        for (int i = 0; i < hallways.Count; i++)
        {
            Coordinate start = dungeonData.rooms[hallways[i].x].center;
            Coordinate end = dungeonData.rooms[hallways[i].y].center;

            AStar aStar = new(dungeonData.mapData, start, end);

            Coordinate key = end;
            while(aStar.ParentTable.ContainsKey(key))
            {
                key = aStar.ParentTable[key];
                dungeonData.mapData[key.x][key.y].tileType = TileType.Floor;
                if (dungeonData.mapData[key.x][key.y].areaType != AreaType.Room)
                    dungeonData.mapData[key.x][key.y].areaType = AreaType.Hallway;
            }
        }

        for(int i=0; i<dungeonData.rooms.Count; i++)
        {
            for(int j = dungeonData.rooms[i].Entrances.Count-1; j>=0; j--)
            {
                Coordinate c = dungeonData.rooms[i].Entrances[j];
                if (dungeonData.mapData[c.x][c.y].areaType != AreaType.Hallway)
                    dungeonData.rooms[i].Entrances.RemoveAt(j);
            }
        }
    }

    void SetRoomElements()
    {
        List<int> deck = new();
        for(int i=0; i<dungeonData.rooms.Count; i++)
        {
            deck.Add(i);
        }
        int pick = rand.Next(0, deck.Count);
        int first = deck[pick];
        deck.RemoveAt(pick);
        pick = rand.Next(0, deck.Count);
        int second = deck[pick];
        dungeonData.stairRooms = new(first, second);
    }

    bool CheckOverlap (Room a, Room b)
    {
        int top = Mathf.Min(a.Top, b.Top);
        int bottom = Mathf.Max(a.Bottom, b.Bottom);
        int left = Mathf.Max(a.Left,b.Left);
        int right = Mathf.Min(a.Right, b.Right);

        if (((top - bottom) >= 0) && ((right - left) >= 0))
            return true;
        else return false;
    }

    void SetPillarRoom(Room room)
    {
        for (int i = room.Left + 1; i <= room.Right - 1; i++)
        {
            for (int j = room.Bottom + 1; j <= room.Top - 1; j++)
            {
                if (i % 2 == 0 && j % 2 == 0)
                {
                    dungeonData.mapData[i][j].tileType = TileType.Wall;
                }
            }
        }
    }

    void SetBigPillarRoom(Room room)
    {
        int bezel = room.Width / 3;
        bezel = Mathf.Max(1, bezel);

        for (int i = room.Left + bezel; i <= room.Right - bezel; i++)
        {
            for (int j = room.Bottom + bezel; j <= room.Top - bezel; j++)
            {
                dungeonData.mapData[i][j].tileType = TileType.Wall;
            }
        }
    }

    void SetComplicatedRoom(Room room)
    {

    }
}
