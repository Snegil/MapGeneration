using System.IO;
using UnityEngine;

public class Walker
{
    Vector2Int direction;
    public Vector2Int Direction { get { return direction; } }

    Vector2Int position;
    public Vector2Int Position { get { return position; } }

    // Did these separate instead of in a vector2int for readability
    int minX;
    int maxX;
    int minY;
    int maxY;

    /// <summary>
    /// Walker Constructor.
    /// </summary>
    /// <param name="direction">Direction the walker will go towards</param>
    /// <param name="position">Start position</param>
    public Walker(Vector2Int direction, Vector2Int position, Vector2Int gridXSize, Vector2Int gridYSize)
    {
        minX = gridXSize.x;
        maxX = gridXSize.y;
        minY = gridYSize.x;
        maxY = gridYSize.y;

        this.direction = direction;
        this.position = position;
    }
    /// <summary>
    /// Walker Constructor, randomises direction.
    /// </summary>
    /// <param name="position">Start position</param>
    public Walker(Vector2Int position, Vector2Int gridXSize, Vector2Int gridYSize)
    {
        minX = gridXSize.x;
        maxX = gridXSize.y;
        minY = gridYSize.x;
        maxY = gridYSize.y;

        direction = RandomDirection();
        this.position = position;
    }
    /// <summary>
    /// Walker Constructor, randomises starting position and direction (within limits).
    /// </summary>
    /// <param name="gridXSize"></param>
    /// <param name="gridYSize"></param>
    public Walker(Vector2Int gridXSize, Vector2Int gridYSize)
    {
        minX = gridXSize.x;
        maxX = gridXSize.y;
        minY = gridYSize.x;
        maxY = gridYSize.y;

        direction = RandomDirection();
        position = RandomPosition();
    }


    /// <summary>
    /// Move the walker one step in it's direction.
    /// </summary>
    public void Move()
    {
        position += direction;
        position = new Vector2Int(Mathf.Clamp(position.x, minX, maxX), Mathf.Clamp(position.y, minY, maxY));
    }

    /// <summary>
    /// Change the direction of the walker.
    /// </summary>
    public void ChangeDirection()
    {
        Vector2Int newDirection;
        do
        {
            newDirection = RandomDirection();
        } while (Direction == newDirection);
        direction = newDirection;
    }

    Vector2Int RandomPosition()
    {
        return new Vector2Int(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }
    Vector2Int RandomDirection()
    {
        int randomNumber = Random.Range(0, 4);

        return randomNumber switch
        {
            0 => Vector2Int.up,
            1 => Vector2Int.right,
            2 => Vector2Int.down,
            _ => Vector2Int.left,
        };
    }
}
