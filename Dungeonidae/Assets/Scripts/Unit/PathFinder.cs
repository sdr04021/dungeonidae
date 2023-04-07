using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class PathFinder
{
    class Node
    {
        public Coordinate coord;
        public Node parent = null;
        public Directions moved;
        public Directions firstStep = Directions.NONE;
        public int G;
        public float H;

        public Node(Coordinate coord)
        {
            this.coord = coord;
        }
    }

    //PriorityQueue<Node> openList = new();
    SimplePriorityQueue<Node> openList = new();
    List<Node> closeList = new();
    Coordinate target;
    Tile[,] map;
    Fog[,] fogMap;
    Stack<Directions> path = new();
    public Stack<Directions> Path { get { return path; } }
    Dictionary<Coordinate, bool> inCloseList = new();
    Dictionary<Coordinate, bool> isVisited = new();

    public Directions FindPath(Coordinate start, Coordinate target, Tile[,] map, Fog[,] fogMap)
    {
        this.target = target;
        openList.Clear();
        closeList.Clear();
        inCloseList.Clear();
        isVisited.Clear();
        this.map = map;
        this.fogMap = fogMap;

        Node startNode = new(start);
        startNode.G = 0;
        startNode.H = 0;
        closeList.Add(startNode);

        if (!map[target.x,target.y].IsReachableTile()&&(Coordinate.Distance(start,target)<2))
            return Directions.NONE;

        FindNeighbors(closeList[0]);

        bool reachable = false;
        while (true)
        {
            //Debug.Log(openList[0].coord.CoordinateToString());
            if (openList.Count > 0)
            {
                if(!isVisited.ContainsKey(openList.First.coord))
                {
                    closeList.Add(openList.First);
                    inCloseList.Add(openList.First.coord, true);
                    if (openList.First.coord == target)
                    {
                        reachable = true;
                        break;
                    }
                    isVisited.Add(openList.First.coord, true);
                    openList.Dequeue();
                    FindNeighbors(closeList[^1]);
                }
                else openList.Dequeue();
            }
            else
            {
                reachable = false;
                break;
            }
        }
        if (reachable)
        {
            Node routeNode = closeList[^1];
            while (routeNode.parent!=null)
            {
                path.Push(routeNode.moved);
                routeNode = routeNode.parent;
            }
            return closeList[^1].firstStep;
        }
        else
        {
            closeList.Sort(SortByF);
            if (closeList.Count <= 1)
                return Directions.NONE;
            else
            {
                Node routeNode = closeList[1];
                while (routeNode.parent != null)
                {
                    path.Push(routeNode.moved);
                    routeNode = routeNode.parent;
                }
                return closeList[1].firstStep;
            }
        }
    }

    void FindNeighbors(Node start)
    {
        FindNeighbor(start, Directions.N);
        FindNeighbor(start, Directions.NE);
        FindNeighbor(start, Directions.E);
        FindNeighbor(start, Directions.SE);
        FindNeighbor(start, Directions.S);
        FindNeighbor(start, Directions.SW);
        FindNeighbor(start, Directions.W);
        FindNeighbor(start, Directions.NW);
    }

    void FindNeighbor(Node start, Directions dir)
    {
        Coordinate neighbor = start.coord.ToMovedCoordinate(dir, 1);
        if(neighbor.IsValidCoordForMap(map))
        {
            Tile tile = map[neighbor.x, neighbor.y];
            if ((tile.IsReachableTile()) && (fogMap[neighbor.x,neighbor.y].IsObserved))
            {
                if (inCloseList.ContainsKey(neighbor))
                    return;

                Node node = new(neighbor);
                node.G = start.G + 1;
                node.H = Coordinate.Distance(node.coord, target);
                node.parent = start;
                node.moved = dir;
                if (start.parent == null)
                    node.firstStep = dir;
                else node.firstStep = start.firstStep;

                float priority = node.G + node.H;

                if (openList.Contains(node))
                {
                    if (priority < openList.GetPriority(node))
                        openList.UpdatePriority(node, priority);
                }
                else
                    openList.Enqueue(node, priority);
            }   
        }
    }

    int SortByF(Node a, Node b)
    {
        if ((a.G + a.H) < (b.G + b.H)) return -1;
        else if ((a.G + a.H) > (b.G + b.H)) return 1;
        else return 0;
    }
}
