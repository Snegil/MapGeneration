using UnityEngine;

public class Walker
{
    public Vector2 Direction { get; set; }
    public Vector2 Position { get; set; }
    bool yOffset = false;

    public Walker(Vector2Int direction, Vector2Int position)
    {
        Direction = direction;
        Position = position;
    }
    public Walker(Vector2Int position)
    {
        Direction = RandomDirection();
        Position = position;
    }

    public void MoveOneStep()
    {
        yOffset = false;
        //Position += Direction;
        Vector2 newPos = new(Position.x + Direction.x, yOffset ? 2 : 1);
    }

    public Vector2 RandomDirection()
    {
        int randomNumber = Random.Range(0, 4);

        return randomNumber switch
        {
            1 => Direction = Vector2.up,
            2 => Direction = Vector2.right,
            3 => Direction = Vector2.down,
            _ => Direction = Vector2.left
        };
    }
}
