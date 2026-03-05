using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGeneration : MonoBehaviour
{
    public enum LevelTile { Empty, Floor, Wall }

    [SerializeField]
    GameObject floorPrefab;

    [SerializeField]
    GameObject wallPrefab;

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
        if (grid[pos.x, pos.y] == LevelTile.Empty || grid[pos.x, pos.y] == LevelTile.Empty)
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

    [SerializeField]
    int despeckleNeighbourLimit = 4;

    Vector2Int gridXLimits;
    Vector2Int gridYLimits;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Setup();

        GenerateFloors();
        Despeckle();
        GenerateWalls();
        GenerateMap();
    }

    void Setup()
    {
        // INITIALISE GRID
        grid = new LevelTile[levelWidth, levelHeight];
        gridXLimits = new(edgeOffset, grid.GetLength(0) - 1 - edgeOffset);
        gridYLimits = new(edgeOffset, grid.GetLength(1) - 1 - edgeOffset);

        SetAllTilesAsEmpty();

        // INITIALISE WALKER
        walkers.Add(new Walker(gridXLimits, gridYLimits));
        // walkers.Add(new Walker(gridXLimits, gridYLimits));
        // walkers.Add(new Walker(gridXLimits, gridYLimits));
    }

    void SetAllTilesAsEmpty()
    {
        for (int x = 0; x < levelWidth; x++)
        {
            for (int z = 0; z < levelHeight; z++)
            {
                grid[x, z] = LevelTile.Empty;
            }
        }
    }

    void GenerateFloors()
    {
        do
        {
            for (int i = 0; i < walkers.Count; i++)
            {
                if (walkers[i] == null) break;

                if (grid[walkers[i].Position.x, walkers[i].Position.y] == LevelTile.Empty)
                {
                    AddToGrid(walkers[i].Position, LevelTile.Floor);
                }
            }

            // MOVE WALKERS
            for (int i = 0; i < walkers.Count; i++)
            {
                walkers[i].Move();
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
                walkers.Add(new Walker(gridXLimits, gridYLimits));
            }

            // CHECK IF THE WALKERS SHOULD CHANGE DIRECTION
            for (int i = 0; i < walkers.Count; i++)
            {
                if (Random.value > chanceOfSwitchingDir)
                {
                    walkers[i].ChangeDirection();
                }
            }

            iterations++;

        } while (iterations < maxIterations);
    }

    void Despeckle()
    {
        for (int x = edgeOffset; x < grid.GetLength(0) - edgeOffset; x++)
        {
            for (int y = edgeOffset; y < grid.GetLength(1) - edgeOffset; y++)
            {
                bool[] neighbouring = new bool[] { grid[x, y + 1] == LevelTile.Floor, grid[x + 1, y + 1] == LevelTile.Floor, grid[x + 1, y] == LevelTile.Floor, grid[x + 1, y - 1] == LevelTile.Floor, grid[x, y - 1] == LevelTile.Floor, grid[x - 1, y - 1] == LevelTile.Floor, grid[x - 1, y] == LevelTile.Floor, grid[x - 1, y + 1] == LevelTile.Floor };
                Debug.Log(neighbouring.Count(c => c));
                if (neighbouring.Count(c => c) >= despeckleNeighbourLimit)
                {
                    grid[x, y] = LevelTile.Floor;
                }
            }
        }
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
                    case LevelTile.Floor:
                        Instantiate(floorPrefab, new Vector3(x, 0.1f, z), Quaternion.identity, transform);
                        break;
                    case LevelTile.Wall:
                        Instantiate(wallPrefab, new Vector3(x, 0.1f, z), Quaternion.identity, transform);
                        break;
                    default:
                        //Debug.Log("Empty.");
                        break;
                }
            }
        }
    }
}
