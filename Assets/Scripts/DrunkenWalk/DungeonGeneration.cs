using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGeneration : MonoBehaviour
{
    [SerializeField]
    GameObject floorPrefab;

    [SerializeField]
    GameObject wallPrefab;

    List<Walker> walkers = new();

    [Space, SerializeField]
    int startingWalkers = 1;
    public float StartingWalkers { set { startingWalkers = (int)value; } }

    [SerializeField]
    int maxWalkers = 1;
    public float MaxWalkers { set { maxWalkers = (int)value; } }

    [SerializeField]
    int levelWidth;
    public float LevelWidth { get { return levelWidth; } set { levelWidth = (int)value; } }

    [SerializeField]
    int levelHeight;
    public float LevelHeight { get { return levelHeight; } set { levelHeight = (int)value; } }

    DungeonGrid grid;

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

    bool shouldDespeckle = true;
    public bool ShouldDespeckle { set { shouldDespeckle = value; } }

    [SerializeField]
    int despeckleNeighbourLimit = 4;

    Vector2Int gridXLimits;
    Vector2Int gridYLimits;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Setup();
        Generate();
    }

    void Setup()
    {
        grid = new DungeonGrid(levelWidth, levelHeight);

        gridXLimits = new(edgeOffset, levelWidth - 1 - edgeOffset);
        gridYLimits = new(edgeOffset, levelHeight - 1 - edgeOffset);
    }

    public void Generate()
    {
        if (grid.HasTiles()) ResetMap();

        CreateStartingWalkers();
        GenerateFloors();
        if (shouldDespeckle) Despeckle();
        GenerateWalls();
        GenerateMap();
    }

    void ResetMap()
    {
        grid.SetAllTilesAsEmpty();
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        walkers.Clear();
    }

    void CreateStartingWalkers()
    {
        for (int i = 0; i < startingWalkers; i++)
        {
            walkers.Add(new Walker(gridXLimits, gridYLimits));
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

                if (grid.GetTileAtPos(walkers[i].Position.x, walkers[i].Position.y) == LevelTiles.Empty)
                {
                    grid.SetTileAtPos(walkers[i].Position, LevelTiles.Floor);
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
                Walker walker = walkers[0];
                for (int i = 0; i < walkers.Count; i++)
                {
                    if (walkers[i].TimesWalked > walker.TimesWalked)
                    {
                        walker = walkers[i];
                    }
                }
                walkers.Remove(walker);
            }

            // CHECK IF A NEW WALKER SHOULD BE MADE
            if (Random.value < chanceOfNewWalker && walkers.Count < maxWalkers)
            {
                walkers.Add(new Walker(gridXLimits, gridYLimits));
            }

            // CHECK IF THE WALKERS SHOULD CHANGE DIRECTION
            for (int i = 0; i < walkers.Count; i++)
            {
                if (Random.value < chanceOfSwitchingDir)
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
        for (int x = edgeOffset; x < grid.GetGridLength().x - edgeOffset; x++)
        {
            for (int y = edgeOffset; y < grid.GetGridLength().y - edgeOffset; y++)
            {
                // Check all surrounding tiles, and check if they're leveltile.floor.
                List<bool> neighbouring = new();

                for (int ix = -1; ix < 2; ix++)
                {
                    for (int iy = -1; iy < 2; iy++)
                    {
                        if (ix == 0 && iy == 0) continue;

                        if (grid.GetTileAtPos(x + ix, y + iy) == LevelTiles.Floor)
                        {
                            neighbouring.Add(true);
                            continue;
                        }
                        neighbouring.Add(false);
                    }
                }

                // If the amount of neighbouring tiles are floors, change the targeted one, to floor.
                if (neighbouring.Count(c => c) >= despeckleNeighbourLimit)
                {
                    grid.SetTileAtPos(new Vector2Int(x, y), LevelTiles.Floor);
                }
            }
        }
    }

    // Check all tiles on the grid, and if a leveltile.floor has leveltile.empty next to it, add a wall there.
    void GenerateWalls()
    {
        for (int x = edgeOffset; x < grid.GetGridLength().x; x++)
        {
            for (int y = edgeOffset; y < grid.GetGridLength().y; y++)
            {
                if (grid.GetTileAtPos(x, y) != LevelTiles.Floor) continue;

                // Check all tiles around the X and Y coordinate, if they are empty, make them a wall.
                for (int ix = -1; ix < 2; ix++)
                {
                    for (int iy = -1; iy < 2; iy++)
                    {
                        if (ix == 0 && iy == 0) continue;

                        if (grid.GetTileAtPos(x + ix, y + iy) == LevelTiles.Empty)
                        {
                            grid.SetTileAtPos(new Vector2Int(x + ix, y + iy), LevelTiles.Wall);
                        }
                    }
                }
            }
        }
    }

    public void GenerateMap()
    {
        StopAllCoroutines();
        StartCoroutine(GenerateTiles());
        return;
    }

    // Instantiate tiles into game world at assigned locations, reasoning behind coroutine is to make it animated.
    IEnumerator GenerateTiles()
    {
        for (int z = levelHeight - 1; z > 0; z--)
        {

            for (int x = 0; x < levelWidth - 1; x++)
            {
                switch (grid.GetTileAtPos(x, z))
                {
                    case LevelTiles.Floor:
                        Instantiate(floorPrefab, new Vector3(x, 0.1f, z), Quaternion.identity, transform);
                        break;
                    case LevelTiles.Wall:
                        Instantiate(wallPrefab, new Vector3(x, 0.1f, z), Quaternion.identity, transform);
                        break;
                    default:
                        //Debug.Log("Empty.");
                        break;
                }
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    public DungeonGrid GetDungeonGrid()
    {
        return grid;
    }

    // Set all tiles into the grid and generate them. (This is called from LoadMapData.cs)
    public void LoadedMap(LevelTiles[,] levelTiles)
    {
        ResetMap();

        for (int x = 0; x < grid.GetGridLength().x; x++)
        {
            for (int y = 0; y < grid.GetGridLength().y; y++)
            {
                grid.SetTileAtPos(x, y, levelTiles[x, y]);
            }
        }
        GenerateMap();
    }
}
