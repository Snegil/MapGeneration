using UnityEngine;

public class HexTile
{
    public int TileID { get; set; }
    public Vector3 Position { get; set; }
    public LevelTile Tile { get; set; }

    public HexTile(int tileID, Vector3 position, LevelTile tile)
    {
        TileID = tileID;
        Tile = tile;
        Position = position;

    }
}
