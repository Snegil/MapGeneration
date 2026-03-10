using UnityEngine;

public class DungeonGrid
{
    public LevelTiles[,] grid;

    int levelWidth;
    int levelHeight;


    /// <summary>
    /// DungeonGrid Constructor
    /// </summary>
    /// <param name="levelWidth"></param>
    /// <param name="levelHeight"></param>
    public DungeonGrid(int levelWidth, int levelHeight)
    {
        this.levelWidth = levelWidth;
        this.levelHeight = levelHeight;
        grid = new LevelTiles[levelWidth, levelHeight];
    }

    /// <summary>
    /// Get the entire grid as is.
    /// </summary>
    /// <returns>LevelTiles[,]</returns>
    public LevelTiles[,] GetGrid()
    {
        return grid;
    }

    /// <summary>
    /// Set tile at specified position.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="levelTile"></param>    
    public void SetTileAtPos(int x, int y, LevelTiles levelTile)
    {
        grid[x, y] = levelTile;
    }
    /// <summary>
    /// Set tile at specified position.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="levelTile"></param>
    public void SetTileAtPos(Vector2Int pos, LevelTiles levelTile)
    {
        grid[pos.x, pos.y] = levelTile;
    }

    /// <summary>
    /// Get tile at specified position.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>LevelTiles</returns>
    public LevelTiles GetTileAtPos(int x, int y)
    {
        return grid[x, y];
    }

    /// <summary>
    /// Get tile at specified position.
    /// </summary>
    /// <param name="position"></param>
    /// <returns>Leveltiles</returns>
    public LevelTiles GetTileAtPos(Vector2Int position)
    {
        return grid[position.x, position.y];
    }

    /// <summary>
    /// Get grid length
    /// </summary>
    /// <returns>Vector2Int</returns>
    public Vector2Int GetGridLength()
    {
        return new Vector2Int(grid.GetLength(0), grid.GetLength(1));
    }

    /// <summary>
    /// Reset all tiles to LevelTiles.Empty.
    /// </summary>
    public void SetAllTilesAsEmpty()
    {
        for (int x = 0; x < levelWidth; x++)
        {
            for (int z = 0; z < levelHeight; z++)
            {
                SetTileAtPos(x, z, LevelTiles.Empty);
            }
        }
    }
    public bool HasTiles()
    {
        for (int x = 0; x < levelWidth; x++)
        {
            for (int y = 0; y < levelHeight; y++)
            {
                if (grid[x, y] != LevelTiles.Empty || grid[x, y] != LevelTiles.Wall || grid[x, y] != LevelTiles.Floor)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void WriteOutGrid()
    {
        string gridString = "";
        for (int x = 0; x < levelWidth; x++)
        {
            for (int y = 0; y < levelHeight; y++)
            {
                gridString += (int)grid[x, y];
            }
            gridString += "\n";
        }
        Debug.Log(gridString);
    }
}
