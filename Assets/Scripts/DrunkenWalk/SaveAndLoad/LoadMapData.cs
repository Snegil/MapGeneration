using System;
using System.IO;
using SFB;
using UnityEngine;

public class LoadMapData : MonoBehaviour
{
    DungeonGeneration dungeonGeneration;

    public void Load()
    {
        if (dungeonGeneration == null) dungeonGeneration = GetComponent<DungeonGeneration>();
        LevelTiles[,] levelTiles = new LevelTiles[(int)dungeonGeneration.LevelWidth, (int)dungeonGeneration.LevelHeight];
        string[] filepaths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);

        using (StreamReader sw = File.OpenText(filepaths[^1]))
        {
            sw.ReadLine();
            sw.ReadLine();

            string levelData = sw.ReadToEnd().Trim();

            if (levelData == null)
            {
                Debug.Log("LEVELDATA IS NULL");
            }

            // This splits the data with the char "|"
            /*
                Example outcome:
                3,53,1
                3,54,2
            */
            string[] initialSplit = levelData.Split("|");

            // Loop through initialSplit and split each one into separate strings in an array [X pos, Y pos, LevelTile].
            for (int j = 0; j < initialSplit.Length - 1; j++)
            {
                string[] finalSplit = initialSplit[j].Split(",");

                int x = int.Parse(finalSplit[0]);
                int y = int.Parse(finalSplit[1]);

                LevelTiles tile = (LevelTiles)Enum.Parse(typeof(LevelTiles), finalSplit[2]);

                levelTiles[x, y] = tile;
            }
            dungeonGeneration.LoadedMap(levelTiles);
        }
    }
}
