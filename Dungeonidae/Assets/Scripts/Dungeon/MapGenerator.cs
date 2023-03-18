using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DelaunatorSharp;

public class MapGenerator
{
    public Tile[,] map;

    int roomSeedAmount = 300;
    int growCount = 20;

    List<Room> rooms = new List<Room>();
    public List<Room> Rooms {  get { return rooms; } }

    void Start()
    {
        //GenerateRooms(map);
    }

    public void GenerateRooms(Tile[,] map)
    {
        this.map = map;

        for(int i=0; i<roomSeedAmount; i++)
        {
            rooms.Add(new Room(new Coordinate(Random.Range(2, map.GetLength(0) - 2), Random.Range(2, map.GetLength(1) - 2))));
        }

        for(int i=0; i<growCount; i++)
        {
            for (int j = rooms.Count-1; j>=0; j--)
            {
                if (!rooms[j].finished)
                {
                    rooms[j].Grow(Random.Range(0, 5), map);
                    for (int k = 0; k < rooms.Count; k++)
                    {
                        if ((j != k) && CheckOverlap(rooms[j], rooms[k]))
                        {
                            if (rooms[j].Width < 9)
                            {
                                rooms[k].Average(rooms[j]);
                                rooms.RemoveAt(j);
                            }
                            else
                            {
                                rooms[j].finished = true;
                                rooms[k].finished = true;
                            }
                            break;
                        }
                    }
                }
            }
        }

        for (int i = rooms.Count - 1; i >= 0; i--)
        {
            if ((rooms[i].Width < 3) || (rooms[i].Height<3))
                rooms.RemoveAt(i);
            else rooms[i].SetCenter();
        }

        for (int i = 0; i < rooms.Count; i++) 
        {
            for (int j = rooms[i].Left; j <= rooms[i].Right; j++) 
            {
                for(int k = rooms[i].Bottom; k <= rooms[i].Top; k++)
                {
                    map[j, k].SetTileType(TileType.Floor);
                    map[j, k].SetAreaType(AreaType.Room);
                }
            }
        }

        for (int i = 0; i < rooms.Count; i++)
        {
            if(Random.Range(4,5)==0)
                SetBigPillarRoom(rooms[i]);
        }

        int[,] graph = new int[rooms.Count, rooms.Count];
        Dictionary<Coordinate,int> roomIndexDict = new();
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
        foreach(IEdge item in iEdges)
        {
            Coordinate P = new((int)item.P.X, (int)item.P.Y);
            Coordinate Q = new((int)item.Q.X, (int)item.Q.Y);

            int dist = (int)Coordinate.Distance(P, Q);
            graph[roomIndexDict[P], roomIndexDict[Q]] = dist;
            graph[roomIndexDict[Q], roomIndexDict[P]] = dist;
        }

        PriorityQueue<Coordinate> pQueue = new();
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
                    pQueue.Enqueue(new Coordinate(current, i), -graph[current, i]);
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
        for (int i=0; i<hallways.Count; i++)
        {
            graph[hallways[i].x, hallways[i].y] = -1;
            graph[hallways[i].y, hallways[i].x] = -1;
        }
        for(int i=0; i<graph.GetLength(0)-1; i++)
        {
            for(int j=i+1; j<graph.GetLength(1); j++)
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
            Coordinate amount = end - start;
            bool foundStartEntrance = false;
            int stepDir = Random.Range(0, 1);

            while (amount.x != 0 || amount.y != 0)
            {
                Coordinate step = new(0, 0);

                if (stepDir == 0)
                {
                    if (amount.x != 0)
                        step.x = (int)Mathf.Sign(amount.x) * 1;
                    else
                        step.y += (int)Mathf.Sign(amount.y) * 1;
                }
                else
                {
                    if (amount.y != 0)
                        step.y = (int)Mathf.Sign(amount.y) * 1;
                    else
                        step.x += (int)Mathf.Sign(amount.x) * 1;
                }

                start += step;
                amount -= step;
                if (map[start.x, start.y].Area != AreaType.Room)
                {
                    if (!foundStartEntrance)
                    {
                        map[start.x, start.y].SetAreaType(AreaType.Entrance);
                        map[start.x, start.y].SetTileType(TileType.Floor);
                        start += step;
                        //amount -= step;
                        foundStartEntrance = true;
                    }
                    map[start.x, start.y].SetAreaType(AreaType.Hallway);
                    map[start.x, start.y].SetTileType(TileType.Floor);
                    if (rooms[hallways[i].y].IsAttached(start))
                    {
                        map[start.x, start.y].SetAreaType(AreaType.Entrance);
                        break;
                    }
                }
            }
        }
        
        /*
        for(int i=0; i<rooms.Count; i++)
        {
            map[rooms[i].center.x, rooms[i].center.y].SetTileType(TileType.Wall);
        }
        */
    }

    bool CheckOverlap (Room a, Room b)
    {
        int top = Mathf.Min(a.Top, b.Top);
        int bottom = Mathf.Max(a.Bottom, b.Bottom);
        int left = Mathf.Max(a.Left,b.Left);
        int right = Mathf.Min(a.Right, b.Right);

        if (((top - bottom) >= -3) && ((right - left) >= -3))
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
}
