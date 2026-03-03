using UnityEngine;

public class Walker
{
    public Vector2Int Direction { get; set; }
    public Vector2Int Position { get; set; }


    public Walker(Vector2Int direction, Vector2Int position)
    {
        Direction = direction;
        Position = position;
    }
}
