using System.Collections.Generic;
using UnityEngine;

public class HexMap
{
    List<HexTile> tiles;

    [SerializeField]
    List<GameObject> tileObjects = new();

    /// <summary>
    /// Add new Tile to HexMap
    /// </summary>
    /// <param name="tileID">TileID</param>
    /// <param name="tilePosition">TilePosition</param>
    public void AddTile(int tileID, Vector3 tilePosition, HexLevelTile hexLevelTile)
    {
        tiles.Add(new HexTile(tileID, tilePosition, hexLevelTile));
    }

    /// <summary>
    /// Get tile at specified location
    /// </summary>
    /// <param name="position">tiles position in world space.</param>
    /// <returns>HexTile, if no tile at position, returns null</returns>
    public HexTile GetTile(Vector3 position)
    {
        if (tiles.Count == 0)
        {
            Debug.LogWarning("NO TILE AT REQUESTED POSITION\n" + position);
            return null;
        }
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].Position == position)
            {
                return tiles[i];
            }
        }
        Debug.LogWarning("NO TILE AT REQUESTED POSITION\n" + position);
        return null;
    }

    /// <summary>
    /// Get all tiles in HexMap.
    /// </summary>
    /// <returns>HexTile List</returns>
    public List<HexTile> GetAllTiles()
    {
        return tiles;
    }
}
