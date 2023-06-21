using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGenerator_G_Three
{
    readonly int mapWidth = 100;
    readonly int mapHeight = 100;

    readonly DungeonData dungeonData;
    readonly System.Random rand;

    bool[,] visited;
    readonly float perlinScale = 0.1f; // Scale for perlin noise
    readonly float biomeThreshold = 0.5f; // Threshold for biome assignment
    readonly int minFloorAreaSize = 10;

    public FieldGenerator_G_Three(DungeonData dungeonData)
    {
        this.dungeonData = dungeonData;
        dungeonData.mapData = new();
        dungeonData.rooms = new();
        dungeonData.genArea = new();
        dungeonData.dungeonType = DungeonType.Maze;
        rand = new System.Random(GameManager.Instance.saveData.FloorSeeds[dungeonData.floor]);

        GenerateField();
        ConnectFloorAreas();
    }

    void GenerateField()
    {
        visited = new bool[mapWidth, mapHeight];
        for (int i = 0; i < mapWidth; i++)
        {
            List<TileData> temp = new();
            for (int j = 0; j < mapHeight; j++)
            {
                temp.Add(new TileData(rand.Next()));
            }
            dungeonData.mapData.Add(temp);
        }

        for (int x = 3; x < mapWidth - 3; x++)
        {
            for (int y = 3; y < mapHeight - 3; y++)
            {
                float perlinValue = Mathf.PerlinNoise(x * perlinScale, y * perlinScale);

                if (perlinValue > biomeThreshold)
                {
                    // Set floor tile
                    dungeonData.mapData[x][y].tileType = TileType.Floor;
                }
                else
                {
                    // Set wall tile
                    dungeonData.mapData[x][y].tileType = TileType.Wall;
                }
            }
        }
    }

    void ConnectFloorAreas()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                visited[x, y] = false;
            }
        }

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (dungeonData.mapData[x][y].tileType == TileType.Floor && !visited[x, y])
                {
                    int floorAreaSize = FloodFill(x, y);

                    if (floorAreaSize < minFloorAreaSize)
                    {
                        for (int i = 0; i < mapWidth; i++)
                        {
                            for (int j = 0; j < mapHeight; j++)
                            {
                                if (visited[i, j])
                                {
                                    dungeonData.mapData[x][y].tileType = TileType.Wall; // Convert small floor areas to walls
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    int FloodFill(int x, int y)
    {
        if (x < 3 || x >= mapWidth - 3 || y < 3 || y >= mapHeight - 3)
        {
            return 0;
        }

        if (visited[x, y] || dungeonData.mapData[x][y].tileType != TileType.Floor)
        {
            return 0;
        }

        visited[x, y] = true;

        return 1 + FloodFill(x - 1, y) + FloodFill(x + 1, y) + FloodFill(x, y - 1) + FloodFill(x, y + 1);
    }
}

