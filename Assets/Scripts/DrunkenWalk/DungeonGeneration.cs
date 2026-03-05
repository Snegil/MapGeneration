using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public float MaxWalkers { set { maxWalkers = (int)value; } }

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

    [SerializeField]
    int maxIterations;
    public float MaxIterations { set { maxIterations = (int)value; } }

    [SerializeField]
    float chanceOfDying = 0.25f;
    public float ChanceOfDying { set { chanceOfDying = value; } }

    [SerializeField]
    float chanceOfNewWalker = 0.5f;
    public float ChanceOfNewWalker { set { chanceOfNewWalker = value; } }

    [SerializeField]
    float chanceOfSwitchingDir = 0.5f;
    public float ChanceOfSwitchingDir { set { chanceOfSwitchingDir = value; } }

    [SerializeField]
    int despeckleNeighbourLimit = 4;

    Vector2Int gridXLimits;
    Vector2Int gridYLimits;

    public void RegenerateMap(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        StopAllCoroutines();
        StartCoroutine(RegenerateMapCoroutine());
    }
    // Regenerate map to get a completely new one.
    public void RegenerateMap()
    {
        StopAllCoroutines();
        StartCoroutine(RegenerateMapCoroutine());
    }

    IEnumerator RegenerateMapCoroutine()
    {
        ResetMap();
        yield return new WaitForSeconds(0.1f);
        Setup();

        GenerateFloors();
        Despeckle();
        GenerateWalls();
        GenerateMap();
    }
    // Clear all child-objects and all walkers.
    void ResetMap()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        walkers.Clear();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RegenerateMap();
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
        int iterations = 0;
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

    // Despeckle checks all surrounding tiles and if the surrounding tiles match, change the target to floor.
    void Despeckle()
    {
        for (int x = edgeOffset; x < grid.GetLength(0) - edgeOffset; x++)
        {
            for (int y = edgeOffset; y < grid.GetLength(1) - edgeOffset; y++)
            {
                // Check all surrounding tiles, and check if they're leveltile.floor.
                bool[] neighbouring = new bool[] { grid[x, y + 1] == LevelTile.Floor,
                                                   grid[x + 1, y + 1] == LevelTile.Floor,
                                                   grid[x + 1, y] == LevelTile.Floor,
                                                   grid[x + 1, y - 1] == LevelTile.Floor,
                                                   grid[x, y - 1] == LevelTile.Floor,
                                                   grid[x - 1, y - 1] == LevelTile.Floor,
                                                   grid[x - 1, y] == LevelTile.Floor,
                                                   grid[x - 1, y + 1] == LevelTile.Floor
                                                 };
                // If the amount of neighbouring tiles are floors, change the targeted one, to floor.
                if (neighbouring.Count(c => c) >= despeckleNeighbourLimit)
                {
                    grid[x, y] = LevelTile.Floor;
                }
            }
        }
    }

    // Check all tiles on the grid, and if a leveltile.floor has leveltile.empty next to it, add a wall there.
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
        StartCoroutine(GenerateTiles());
        return;
    }
    // Instantiate tiles into game world at assigned locations, reasoning behind coroutine is to make it animated.
    IEnumerator GenerateTiles()
    {
        for (int z = levelHeight - 1; z > 0; z--)
        {
            yield return new WaitForSeconds(0.01f);
            for (int x = 0; x < levelWidth - 1; x++)
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
