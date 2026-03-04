using UnityEngine;

public class HexWalker
{
    public Vector2 Direction { get; set; }
    public Vector2 Position { get; set; }

    /// <summary>
    /// Walker Constructor
    /// </summary>
    /// <param name="direction">Direction the walker will go</param>
    /// <param name="position">Start position of walker</param>
    public HexWalker(Vector2 direction, Vector2 position)
    {
        Direction = direction;
        Position = position;
    }

    /// <summary>
    /// Walker Constructor
    /// </summary>
    /// <param name="position">Start Position of walker</param>
    public HexWalker(Vector2 position)
    {
        Direction = RandomDirection();
        Position = position;
    }

    public void MoveOneStep()
    {
        Position += Direction;
    }

    public Vector3 RandomDirection()
    {
        int randomNumber = Random.Range(0, 4);

        return randomNumber switch
        {
            1 => Direction = new Vector3(0, 0, 1.5f),
            2 => Direction = new Vector3(2, 0, 0),
            3 => Direction = new Vector3(0, 0, -1.5f),
            _ => Direction = new Vector3(-2, 0, 1.5f),
        };
    }
}
