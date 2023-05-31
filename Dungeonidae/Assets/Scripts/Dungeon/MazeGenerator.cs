using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class MazeGenerator
{
    readonly int mapWidth = 100;
    readonly int mapHeight = 100;

    readonly DungeonData dungeonData;
    readonly System.Random rand;

    public Task task;

    public MazeGenerator(DungeonData dungeonData)
    {
        this.dungeonData = dungeonData;
        dungeonData.mapData = new();
        dungeonData.rooms = new();
        dungeonData.genArea = new();
        dungeonData.dungeonType = DungeonType.Maze;
        rand = new System.Random(GameManager.Instance.saveData.FloorSeeds[dungeonData.floor]);

        WilsonsMaze();
    }

    void WilsonsMaze()
    {
        for (int i = 0; i < mapWidth; i++)
        {
            List<TileData> temp = new();
            for (int j = 0; j < mapHeight; j++)
            {
                temp.Add(new TileData(rand.Next()));
            }
            dungeonData.mapData.Add(temp);
        }

        List<List<TileData>> mapData = dungeonData.mapData;
        Coordinate mazeSize = new((int)Mathf.Round((float)(mapWidth - 4) / 2), (int)Mathf.Round(((float)(mapHeight - 4) / 2)));

        List<Coordinate> notVisited = new();

        for(int i=0; i< mazeSize.x; i++)
        {
            for(int j=0; j< mazeSize.y; j++)
            {
                notVisited.Add(new Coordinate(i, j));
            }
        }

        HashSet<Coordinate> startEndRooms = new();
        for(int i=0; i<3; i++)
        {
            for(int j=0; j<3; j++)
            {
                startEndRooms.Add(new Coordinate(i, j));
                notVisited.Remove(new Coordinate(i, j));
            }
        }
        for (int i = mazeSize.x-10; i < mazeSize.x; i++)
        {
            for (int j = mazeSize.y-10; j < mazeSize.y; j++)
            {
                startEndRooms.Add(new Coordinate(i, j));
                notVisited.Remove(new Coordinate(i, j));
            }
        }

        startEndRooms.Remove(new Coordinate(1, 2));
        startEndRooms.Remove(new Coordinate(mazeSize.x - 5, mazeSize.y - 10));
        notVisited.Add(new Coordinate(mazeSize.x - 5, mazeSize.y - 10));

        Coordinate current = new Coordinate(mazeSize.x - 5, mazeSize.y - 10);
        List<Coordinate> route = new();
        List<List<Coordinate>> routes = new();
        route.Add(current);

        while (true)
        {
            List<Coordinate> deck = new();
            if ((current.x - 1 >= 0) && (!startEndRooms.Contains(new Coordinate(current.x - 1, current.y))))
                deck.Add(new Coordinate(-1, 0));
            if ((current.x + 1 < mazeSize.x) && (!startEndRooms.Contains(new Coordinate(current.x + 1, current.y))))
                deck.Add(new Coordinate(1, 0));
            if ((current.y - 1 >= 0) && (!startEndRooms.Contains(new Coordinate(current.x, current.y - 1))))
                deck.Add(new Coordinate(0, -1));
            if ((current.y + 1 < mazeSize.y) && (!startEndRooms.Contains(new Coordinate(current.x, current.y + 1))))
                deck.Add(new Coordinate(0, 1));
            current += deck[rand.Next(0, deck.Count)];
            if (route.Contains(current))
            {
                while (route[^1] != current)
                {
                    route.RemoveAt(route.Count - 1);
                }
            }
            else
            {
                if (!notVisited.Contains(current))
                {
                    for(int i=0; i < route.Count; i++)
                    {
                        notVisited.Remove(route[i]);
                    }
                    route.Add(current);
                    routes.Add(route);
                    if (notVisited.Count == 0) break;
                    route = new();
                    current = notVisited[rand.Next(0, notVisited.Count)];
                    route.Add(current);
                    dungeonData.genArea.Add(current);
                }
                else route.Add(current);
            }
        }

        for (int i = 0; i < routes.Count; i++)
        {
            List<Coordinate> r = routes[i];
            for (int j = 0; j < r.Count; j++)
            {
                mapData[r[j].x * 2 + 2][r[j].y * 2 + 2].tileType = TileType.Floor;
                if (j < r.Count - 1)
                {
                    Coordinate path = r[j + 1] - r[j];
                    mapData[r[j].x * 2 + 2 + path.x][r[j].y * 2 + 2 + path.y].tileType = TileType.Floor;
                }
            }
        }

        for(int i=0; i<dungeonData.genArea.Count; i++)
        {
            dungeonData.genArea[i] = new Coordinate(dungeonData.genArea[i].x * 2 + 2, dungeonData.genArea[i].y * 2 + 2);
        }

        dungeonData.rooms.Add(new Room(6, 2, 6, 2));
        dungeonData.rooms.Add(new Room(mapHeight - 3, mapHeight - 22, mapWidth - 3, mapWidth - 22));
        for (int i=0; i<5; i++)
        {
            for(int j=0; j<5; j++)
            {
                mapData[2 + i][2 + j].tileType = TileType.Floor;
            }
        }
        dungeonData.rooms[0].Entrances.Add(new Coordinate(4, 7));
        mapData[4][7].areaType = AreaType.Entrance;
        for(int i=0; i<20; i++)
        {
            for(int j=0; j<20; j++)
            {
                mapData[mapWidth - 3 - i][mapHeight - 3 - j].tileType = TileType.Floor;
            }
        }
        dungeonData.rooms[1].Entrances.Add(new Coordinate(mapWidth - 12, mapHeight - 23));
        mapData[mapWidth - 12][mapHeight - 23].areaType = AreaType.Entrance;
        dungeonData.stairRooms = new(0, 1);
    }
}
