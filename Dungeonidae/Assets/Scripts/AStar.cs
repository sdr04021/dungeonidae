using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;


public class AStar
{
    SimplePriorityQueue<Coordinate> openList = new();
    SimplePriorityQueue<Coordinate> closeList = new();
    Dictionary<Coordinate, int> gTable = new();
    public Dictionary<Coordinate, Coordinate> ParentTable { get; private set; } = new();
    Dictionary<Coordinate, Directions> directionTable = new();

    Coordinate start;
    Coordinate end;
    public Stack<Directions> Path { get; private set; } = new();

    public AStar(List<List<TileData>> mapData, Coordinate start, Coordinate end)
    {
        this.start = start;
        this.end = end;
        openList.Enqueue(start, 0);
        gTable.Add(start, 0);

        while (openList.Count > 0)
        {
            closeList.Enqueue(openList.First, NonDiagonalHeuristic(openList.First, end));
            Coordinate now = openList.Dequeue();
            if(now == end)
            {
                return;
            }
            CheckNeighborArea(mapData, now, Directions.N);
            CheckNeighborArea(mapData, now, Directions.E);
            CheckNeighborArea(mapData, now, Directions.S);
            CheckNeighborArea(mapData, now, Directions.W);
        }
    }

    public AStar(Arr2D<Tile> map, Coordinate start, Coordinate end, Arr2D<Fog> fogMap)
    {
        this.start = start;
        this.end = end;
        openList.Enqueue(start, 0);
        gTable.Add(start, 0);

        while (openList.Count > 0)
        {
            closeList.Enqueue(openList.First, DiagonalHeuristic(openList.First, end));
            Coordinate now = openList.Dequeue();
            if (now == end)
            {
                break;
            }
            CheckNeighborArea(map, fogMap, now, Directions.N);
            CheckNeighborArea(map, fogMap, now, Directions.E);
            CheckNeighborArea(map, fogMap, now, Directions.S);
            CheckNeighborArea(map, fogMap, now, Directions.W);
            CheckNeighborArea(map, fogMap, now, Directions.SW);
            CheckNeighborArea(map, fogMap, now, Directions.SE);
            CheckNeighborArea(map, fogMap, now, Directions.NE);
            CheckNeighborArea(map, fogMap, now, Directions.NW);
        }

        if (closeList.Count > 0)
        {
            Coordinate key = closeList.First;
            while (ParentTable.ContainsKey(key))
            {
                Path.Push(directionTable[key]);
                key = ParentTable[key];
            }
        }
    }

    void CheckNeighborArea(List<List<TileData>> mapData, Coordinate now, Directions direction)
    {
        Coordinate neighbor = now + new Coordinate(direction);
        if ((neighbor.IsValidCoordForMap(mapData)) && (mapData[neighbor.x][neighbor.y].areaType != AreaType.Border))
        {
            int G = gTable[now] + 1;
            float H = NonDiagonalHeuristic(neighbor, end);
            float F = G + H;

            if (!gTable.ContainsKey(neighbor))
            {
                if (openList.Contains(neighbor))
                {
                    if (F < openList.GetPriority(neighbor))
                    {
                        openList.UpdatePriority(neighbor, F);
                        gTable[neighbor] = G;
                        ParentTable[neighbor] = now;
                    }
                }
                else
                {
                    openList.Enqueue(neighbor, F);
                    gTable.Add(neighbor, G);
                    ParentTable.Add(neighbor, now);
                }
            }
        }
    }

    void CheckNeighborArea(Arr2D<Tile> map, Arr2D<Fog> fogMap, Coordinate now, Directions direction)
    {
        Coordinate neighbor = now + new Coordinate(direction);
        if ((neighbor.IsValidCoordForMap(map)) && (map.GetElementAt(neighbor.x, neighbor.y).TileData.areaType != AreaType.Border)) 
        {
            if (!map.GetElementAt(neighbor.x, neighbor.y).IsReachableTile())
                return;
            if ((fogMap != null) && !fogMap.GetElementAt(neighbor.x, neighbor.y).IsObserved)
                return;

            int G = gTable[now] + 1;
            float H = (fogMap == null) ? NonDiagonalHeuristic(neighbor, end) : DiagonalHeuristic(neighbor, end);
            float F = G + H;

            if (!gTable.ContainsKey(neighbor))
            {
                if (openList.Contains(neighbor))
                {
                    if (F < openList.GetPriority(neighbor))
                    {
                        openList.UpdatePriority(neighbor, F);
                        gTable[neighbor] = G;
                        ParentTable[neighbor] = now;
                        directionTable[neighbor] = direction;
                    }
                }
                else
                {
                    openList.Enqueue(neighbor, F);
                    gTable.Add(neighbor, G);
                    ParentTable.Add(neighbor, now);
                    directionTable.Add(neighbor, direction);
                }
            }
        }
    }

    int NonDiagonalHeuristic(Coordinate from, Coordinate taregt)
    {
        return Mathf.Abs(from.x - taregt.x) + Mathf.Abs(from.y - taregt.y);
    }
    float DiagonalHeuristic(Coordinate from, Coordinate taregt)
    {
        //return Mathf.Max(Mathf.Abs(taregt.x-from.x), Mathf.Abs(taregt.y-from.y));
        int x = Mathf.Abs(from.x - taregt.x);
        int y = Mathf.Abs(from.y - taregt.y);
        int d = Mathf.Min(x, y);
        return x + y - d;
    }
}
