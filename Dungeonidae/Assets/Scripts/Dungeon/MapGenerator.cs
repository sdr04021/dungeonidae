using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DelaunatorSharp;
using Priority_Queue;

public class MapGenerator
{
    public Tile[,] map;

    int generationArea = 50;
    int roomSeedAmount = 300;

    List<Room> rooms = new();
    public List<Room> Rooms {  get { return rooms; } }

    public MapGenerator()
    {
        //generationArea = Mathf.Min(100, generationArea);
        //roomSeedAmount = generationArea * 6;
    }

    public void GenerateRooms(Tile[,] map)
    {
        this.map = map;

        for(int i=0; i<roomSeedAmount; i++)
        {
            //rooms.Add(new Room(new Coordinate(Random.Range(2, map.GetLength(0) - 2), Random.Range(2, map.GetLength(1) - 2)),10));
            rooms.Add(new Room(new Coordinate(Random.Range(2, generationArea), Random.Range(2, generationArea)), Random.Range(8,12)));
        }

        while (true)
        {
            int finished = 0;
            for (int i = rooms.Count - 1; i >= 0; i--)
            {
                if (rooms[i].GrowthCount > 0)
                {
                    rooms[i].Grow(Random.Range(0, 5), map);
                    for (int j = 0; j < rooms.Count; j++)
                    {
                        if ((i != j) && CheckOverlap(rooms[i], rooms[j]))
                        {
                            if (rooms[i].Width < 9)
                            {
                                rooms[j].Average(rooms[i]);
                                rooms.RemoveAt(i);
                            }
                            else
                            {
                                rooms[i].ExcludeBorder();
                                rooms[i].FinishGrowth();
                                rooms[j].ExcludeBorder();
                                rooms[j].FinishGrowth();
                            }
                            break;
                        }
                    }
                }
                else finished++;
            }
            if (finished == rooms.Count)
                break;
        }

        for (int i = rooms.Count - 1; i >= 0; i--)
        {
            if ((rooms[i].Width < 5) || (rooms[i].Height<5))
                rooms.RemoveAt(i);
            else rooms[i].SetCenter();
        }

        for (int i = 0; i < rooms.Count; i++) 
        {
            for (int j = rooms[i].Left; j <= rooms[i].Right; j++)
            {
                for (int k = rooms[i].Bottom; k <= rooms[i].Top; k++)
                {
                    map[j, k].SetTileType(TileType.Wall);
                    map[j, k].SetAreaType(AreaType.Border);
                }
            }
            rooms[i].ExcludeBorder();
            for (int j = rooms[i].Left; j <= rooms[i].Right; j++) 
            {
                for(int k = rooms[i].Bottom; k <= rooms[i].Top; k++)
                {
                    map[j, k].SetTileType(TileType.Floor);
                    map[j, k].SetAreaType(AreaType.Room);
                }
            }
            rooms[i].SetEntranceCandidates(map);
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

        ConnectRooms();
    }

    void ConnectRooms()
    {
        int[,] graph = new int[rooms.Count, rooms.Count];
        Dictionary<Coordinate, int> roomIndexDict = new();
        for (int i = 0; i < rooms.Count; i++)
        {
            roomIndexDict.Add(rooms[i].center, i);
            for (int j = 0; j < rooms.Count; j++)
            {
                graph[i, j] = -1;
            }
        }

        IPoint[] points = new IPoint[rooms.Count];
        for (int i = 0; i < rooms.Count; i++)
        {
            points[i] = new Point(rooms[i].center.x, rooms[i].center.y);
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
        bool[] visitFlag = new bool[rooms.Count];
        System.Array.Fill(visitFlag, false);
        visitFlag[current] = true;
        List<Coordinate> hallways = new();

        while (visited < rooms.Count)
        {
            for (int i = 0; i < rooms.Count; i++)
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
        int pickAmount = hallways.Count / 4;
        for (int i = 0; i < pickAmount; i++)
        {
            int pick = Random.Range(0, additional.Count);
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
            Coordinate start = rooms[hallways[i].x].center;
            Coordinate end = rooms[hallways[i].y].center;

            AStar aStar = new(map, start, end);

            Coordinate key = end;
            while(aStar.ParentTable.ContainsKey(key))
            {
                key = aStar.ParentTable[key];
                map[key.x, key.y].SetTileType(TileType.Floor);
                if (map[key.x, key.y].Area != AreaType.Room)
                    map[key.x, key.y].SetAreaType(AreaType.Hallway);
            }
        }

        for(int i=0; i<rooms.Count; i++)
        {
            for(int j = rooms[i].Entrances.Count-1; j>=0; j--)
            {
                Coordinate c = rooms[i].Entrances[j];
                if (map[c.x, c.y].Area != AreaType.Hallway)
                    rooms[i].Entrances.RemoveAt(j);
            }
        }

        SetRoomType();
    }

    void SetRoomType()
    {
        for(int i=0; i<rooms.Count; i++)
        {
            if (Random.Range(0, 2) == 1)
                SetComplicatedRoom(rooms[i]);
        }
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
                    map[i, j].SetTileType(TileType.Wall);
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
                map[i, j].SetTileType(TileType.Wall);
            }
        }
    }

    void SetComplicatedRoom(Room room)
    {

    }
}
