using UnityEngine;

public class HexTile
{
    public int TileID { get; set; }
    public Vector3 Position { get; set; }
    public HexLevelTile Tile { get; set; }

    public HexTile(int tileID, Vector3 position, HexLevelTile tile)
    {
        TileID = tileID;
        Tile = tile;
        Position = position;

    }
}
