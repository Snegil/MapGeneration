using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DungeonGeneration : MonoBehaviour
{
    public enum LevelTile { Empty, Water, Floor, Wall }

    [SerializeField]
    GameObject floorPrefab;

    [SerializeField]
    GameObject wallPrefab;

    [SerializeField]
    GameObject waterPrefab;

    List<Walker> walkers = new();
    [SerializeField]
    int maxWalkers = 1;

    [SerializeField]
    int levelWidth;
    [SerializeField]
    int levelHeight;

    LevelTile[,] grid;

    public void AddToGrid(Vector2Int pos, LevelTile levelTile)
    {
        if (grid[pos.x, pos.y] == LevelTile.Empty || grid[pos.x, pos.y] == LevelTile.Water)
        {
            grid[pos.x, pos.y] = levelTile;
        }
    }

    [SerializeField]
    int edgeOffset = 3;

    int iterations;
    [SerializeField]
    int maxIterations;

    [SerializeField]
    float chanceOfDying = 0.25f;

    [SerializeField]
    float chanceOfNewWalker = 0.5f;

    [SerializeField]
    float chanceOfSwitchingDir = 0.5f;

    float[,] noiseMap;
    [Space, Header("Perlin Noise Settings --")]
    [SerializeField]
    int perlinLayers;
    [SerializeField]
    int heightScale;
    [SerializeField]
    int sharpness;
    [SerializeField]
    float scale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Setup();

        GenerateFloors();
        //GenerateWalls();
        LakeFill();
        GenerateMap();
    }

    void Setup()
    {
        // INITIALISE GRID
        grid = new LevelTile[levelWidth, levelHeight];
        noiseMap = new float[levelWidth, levelHeight];

        SaveNoiseValueInArray();

        for (int x = 0; x < levelWidth; x++)
        {
            for (int z = 0; z < levelHeight; z++)
            {
                grid[x, z] = LevelTile.Water;
            }
        }

        // INITIALISE WALKER
        walkers.Add(new Walker(RandomDirection(), new(grid.GetLength(0) / 2, grid.GetLength(1) / 2)));
        walkers.Add(new Walker(RandomDirection(), new(Random.Range(4, grid.GetLength(0) - 4), Random.Range(4, grid.GetLength(1) - 4))));
        walkers.Add(new Walker(RandomDirection(), new(Random.Range(4, grid.GetLength(0) - 4), Random.Range(4, grid.GetLength(1) - 4))));
        //walkers.Add(new Walker(RandomDirection(), new(grid.GetLength(0) - 4, grid.GetLength(1) - 4)));
        //walkers.Add(new Walker(RandomDirection(), new(4, 4)));
    }

    void GenerateFloors()
    {
        do
        {
            for (int i = 0; i < walkers.Count; i++)
            {
                if (walkers[i] == null) break;

                if (grid[walkers[i].Position.x, walkers[i].Position.y] == LevelTile.Water || grid[walkers[i].Position.x, walkers[i].Position.y] == LevelTile.Empty)
                {
                    AddToGrid(walkers[i].Position, LevelTile.Floor);
                }
            }

            // MOVE WALKERS +1 THEIR DIRECTION
            for (int i = 0; i < walkers.Count; i++)
            {
                walkers[i].Position += walkers[i].Direction;
            }

            // AVOID GRID EDGE AND KEEP WALKERS WITHIN GRID SIZE
            for (int i = 0; i < walkers.Count; i++)
            {
                walkers[i].Position = new(Mathf.Clamp(walkers[i].Position.x, edgeOffset, grid.GetLength(0) - edgeOffset), Mathf.Clamp(walkers[i].Position.y, edgeOffset, grid.GetLength(1) - edgeOffset));
            }

            // CHECK IF A WALKER SHOULD DIE
            if (Random.value < chanceOfDying && walkers.Count > 0)
            {
                if (walkers[0] == null) break;
                walkers.RemoveAt(Random.Range(0, walkers.Count - 1));
            }

            // CHECK IF A NEW WALKER SHOULD BE MADE
            if (Random.value > chanceOfNewWalker && walkers.Count < maxWalkers)
            {
                //Debug.Log("ADDED NEW WALKER BUDDY");
                walkers.Add(new Walker(RandomDirection(), new(grid.GetLength(0) / 2, grid.GetLength(0) / 2)));
            }

            // CHECK IF THE WALKERS SHOULD CHANGE DIRECTION
            for (int i = 0; i < walkers.Count; i++)
            {
                if (Random.value > chanceOfSwitchingDir)
                {
                    walkers[i].Direction = RandomDirection();
                }
            }

            iterations++;

        } while (iterations < maxIterations);
    }

    Vector2Int RandomDirection()
    {
        int randomNumber = Random.Range(0, 4);

        return randomNumber switch
        {
            0 => Vector2Int.up,
            1 => Vector2Int.right,
            2 => Vector2Int.down,
            _ => Vector2Int.left,
        };
    }

    void GenerateWalls()
    {
        for (int x = edgeOffset; x < grid.GetLength(0); x++)
        {
            for (int y = edgeOffset; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] != LevelTile.Floor) continue;


                // NORTH
                if (grid[x, y + 1] == LevelTile.Empty)
                {
                    grid[x, y + 1] = LevelTile.Wall;
                }

                // NORTH-EAST
                if (grid[x + 1, y + 1] == LevelTile.Empty)
                {
                    grid[x + 1, y + 1] = LevelTile.Wall;
                }

                // EAST 
                if (grid[x + 1, y] == LevelTile.Empty)
                {
                    grid[x + 1, y] = LevelTile.Wall;
                }

                // SOUTH-EAST
                if (grid[x + 1, y - 1] == LevelTile.Empty)
                {
                    grid[x + 1, y - 1] = LevelTile.Wall;
                }

                // SOUTH
                if (grid[x, y - 1] == LevelTile.Empty)
                {
                    grid[x, y - 1] = LevelTile.Wall;
                }

                // SOUTH-WEST
                if (grid[x - 1, y - 1] == LevelTile.Empty)
                {
                    grid[x - 1, y - 1] = LevelTile.Wall;
                }

                // WEST
                if (grid[x - 1, y] == LevelTile.Empty)
                {
                    grid[x - 1, y] = LevelTile.Wall;
                }

                // NORTH-WEST
                if (grid[x - 1, y + 1] == LevelTile.Empty)
                {
                    grid[x - 1, y + 1] = LevelTile.Wall;
                }
            }
        }
    }

    void LakeFill()
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        bool[,] visited = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!visited[x, y] && grid[x, y] == LevelTile.Water)
                {
                    List<Vector2Int> lakeTiles = new();
                    bool touchesEdge = FloodFillWater(x, y, ref visited, ref lakeTiles);

                    if (!touchesEdge)
                    {
                        foreach (var tile in lakeTiles)
                        {
                            grid[tile.x, tile.y] = LevelTile.Floor; // fill enclosed lake
                        }
                    }
                }
            }
        }
    }

    bool FloodFillWater(int startX, int startY, ref bool[,] visited, ref List<Vector2Int> lakeTiles)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        bool touchesEdge = false;

        Queue<Vector2Int> queue = new();
        queue.Enqueue(new Vector2Int(startX, startY));
        visited[startX, startY] = true;

        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();
            lakeTiles.Add(pos);

            if (pos.x == 0 || pos.y == 0 || pos.x == width - 1 || pos.y == height - 1)
                touchesEdge = true;

            foreach (var dir in new Vector2Int[] {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                var n = pos + dir;
                if (n.x >= 0 && n.y >= 0 && n.x < width && n.y < height && !visited[n.x, n.y])
                {
                    if (grid[n.x, n.y] == LevelTile.Water)
                    {
                        visited[n.x, n.y] = true;
                        queue.Enqueue(n);
                    }
                }
            }
        }

        return touchesEdge;
    }


    void SaveNoiseValueInArray()
    {
        float xOffset = Random.Range(-10000f, 10000f);
        float yOffset = Random.Range(-10000f, 10000f);

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                float noiseValue = 0f;

                for (int k = 0; k < perlinLayers; k++)
                {
                    float frequency = Mathf.Pow(2f, k);
                    float amplitude = Mathf.Pow(0.5f, k);
                    noiseValue += Mathf.PerlinNoise(i * scale * frequency + xOffset, j * scale * frequency + yOffset) * amplitude;
                }

                noiseValue *= heightScale;
                noiseValue = Mathf.Pow(noiseValue, sharpness);

                noiseMap[i, j] = noiseValue;
            }
        }
    }

    public void GenerateMap()
    {
        // GameObject floorCollider = new();
        // floorCollider.name = "FloorCollider";
        // floorCollider.transform.position = new(levelWidth / 2, 0, levelHeight / 2);
        // floorCollider.AddComponent<BoxCollider>();
        // floorCollider.GetComponent<BoxCollider>().size = new Vector3(levelWidth, 0.2f, levelHeight);

        for (int x = 0; x < levelWidth - 1; x++)
        {
            for (int z = 0; z < levelHeight - 1; z++)
            {
                Debug.Log(noiseMap[x, z]);
                switch (grid[x, z])
                {
                    case LevelTile.Water:
                        Instantiate(waterPrefab, new Vector3(x * 4, 0, z * 4), Quaternion.identity, transform);
                        break;
                    case LevelTile.Floor:
                        Instantiate(floorPrefab, new Vector3(x * 4, 1/*noiseMap[x, z]*/, z * 4), Quaternion.identity, transform);
                        break;
                    case LevelTile.Wall:
                        Instantiate(wallPrefab, new Vector3(x * 4, 1/*noiseMap[x, z]*/, z * 4), Quaternion.identity, transform);
                        break;
                    default:
                        //Debug.Log("Empty.");
                        break;
                }
            }
        }
    }
}
