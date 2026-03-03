using UnityEngine;

public class TileGeneration : MonoBehaviour
{
    int iterations;
    [SerializeField]
    int maxIterations;

    DungeonGeneration dungeonGen;
    public DungeonGeneration DungeonGeneration
    {
        get;
        set;
    }

    [SerializeField]
    float chanceOfDying = 0.25f;

    [SerializeField]
    float chanceOfSwitchingDir = 0.5f;

    Vector2Int pos = Vector2Int.zero;
    Vector2Int dir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dir = RandomDirection();
    }

    void MainLoop()
    {
        do
        {
            if (Random.value > chanceOfSwitchingDir)
            {
                dir = RandomDirection();
            }
        } while (iterations < maxIterations);
    }

    Vector2Int RandomDirection()
    {
        int randomNumber = Random.Range(0, 3);

        return randomNumber switch
        {
            0 => Vector2Int.up,
            1 => Vector2Int.right,
            2 => Vector2Int.down,
            _ => Vector2Int.left,
        };
    }
}