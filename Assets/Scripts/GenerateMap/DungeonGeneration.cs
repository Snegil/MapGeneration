using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneration : MonoBehaviour
{
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

    HexMap hexMap = new();

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

        GenerateFloor();
        //GenerateWalls();
        //LakeFill();
        GenerateMap();
    }

    void Setup()
    {
        for (int x = 0; x < levelWidth; x++)
        {
            for (int z = 0; z < levelHeight; z++)
            {
                grid[x, z] = LevelTile.Water;
            }
        }

        // INITIALISE WALKERS
        walkers.Add(new Walker(new(grid.GetLength(0) / 2, grid.GetLength(1) / 2)));
        walkers.Add(new Walker(new(Random.Range(4, grid.GetLength(0) - 4), Random.Range(4, grid.GetLength(1) - 4))));
        walkers.Add(new Walker(new(Random.Range(4, grid.GetLength(0) - 4), Random.Range(4, grid.GetLength(1) - 4))));
    }

    void GenerateFloor()
    {
        do
        {
            for (int i = 0; i < walkers.Count; i++)
            {
                if (walkers[i] == null) break;

                if (grid[walkers[i].Position.x, walkers[i].Position.y] == LevelTile.Water || grid[walkers[i].Position.x, walkers[i].Position.y] == LevelTile.Empty)
                {
                    hexMap.AddTile((int)LevelTile.Floor, walkers[i].Position);
                    //AddToGrid(walkers[i].Position, LevelTile.Floor);
                }
            }

            // MOVE WALKERS +1 THEIR DIRECTION
            for (int i = 0; i < walkers.Count; i++)
            {
                walkers[i].MoveOneStep();
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
                walkers.Add(new Walker(new(grid.GetLength(0) / 2, grid.GetLength(0) / 2)));
            }

            // CHECK IF THE WALKERS SHOULD CHANGE DIRECTION
            for (int i = 0; i < walkers.Count; i++)
            {
                if (Random.value > chanceOfSwitchingDir)
                {
                    walkers[i].RandomDirection();
                }
            }

            iterations++;

        } while (iterations < maxIterations);
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

    public void GenerateMap()
    {
        for (int x = 0; x < levelWidth - 1; x++)
        {
            for (int z = 0; z < levelHeight - 1; z++)
            {
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
                        Debug.Log("Empty.");
                        break;
                }
            }
        }
    }
}
