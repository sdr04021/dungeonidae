using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine.UIElements;

public class AStar
{
    SimplePriorityQueue<Coordinate> openList = new();
    SimplePriorityQueue<Coordinate> closeList = new();
    Dictionary<Coordinate, int> gTable = new();
    public Dictionary<Coordinate, Coordinate> ParentTable { get; private set; } = new();
    Dictionary<Coordinate, Directions> directionTable = new();

    Tile[,] map;
    Coordinate start;
    Coordinate end;
    Fog[,] fogMap = null;
    public Stack<Directions> Path { get; private set; } = new();

    public AStar(Tile[,] map, Coordinate start, Coordinate end)
    {
        this.map = map;
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
            CheckNeighborArea(now, Directions.N);
            CheckNeighborArea(now, Directions.E);
            CheckNeighborArea(now, Directions.S);
            CheckNeighborArea(now, Directions.W);
        }
    }

    public AStar(Tile[,] map, Coordinate start, Coordinate end, Fog[,] fogMap)
    {
        this.map = map;
        this.start = start;
        this.end = end;
        this.fogMap = fogMap;
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
            CheckNeighborArea(now, Directions.N);
            CheckNeighborArea(now, Directions.E);
            CheckNeighborArea(now, Directions.S);
            CheckNeighborArea(now, Directions.W);
            CheckNeighborArea(now, Directions.SW);
            CheckNeighborArea(now, Directions.SE);
            CheckNeighborArea(now, Directions.NE);
            CheckNeighborArea(now, Directions.NW);
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

    void CheckNeighborArea(Coordinate now, Directions direction)
    {
        Coordinate neighbor = now + new Coordinate(direction);
        if ((neighbor.IsValidCoordForMap(map)) && (map[neighbor.x, neighbor.y].Area != AreaType.Border)) 
        {
            if (fogMap != null && (!map[neighbor.x, neighbor.y].IsReachableTile() || !fogMap[neighbor.x, neighbor.y].IsObserved))
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
                        if (fogMap != null) directionTable[neighbor] = direction;
                    }
                }
                else
                {
                    openList.Enqueue(neighbor, F);
                    gTable.Add(neighbor, G);
                    ParentTable.Add(neighbor, now);
                    if (fogMap != null) directionTable.Add(neighbor, direction);
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
        return Mathf.Max(Mathf.Abs(taregt.x-from.x), Mathf.Abs(taregt.y-from.y));
    }
    //https://gamedev.stackexchange.com/questions/64392/a-tile-costs-and-heuristic-how-to-approach
}
