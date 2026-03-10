using System.IO;
using SFB;
using UnityEngine;

public class SaveMapData : MonoBehaviour
{
    DungeonGeneration dungeonGeneration;



    public void Save()
    {
        if (dungeonGeneration == null) dungeonGeneration = GetComponent<DungeonGeneration>();

        // Get the dungeongrid to later write to a file
        DungeonGrid grid = dungeonGeneration.GetDungeonGrid();

        // Open save file panel to get the directory and filename that the user wants
        var filePath = StandaloneFileBrowser.SaveFilePanel("Save File", "", "Generated-Map", "");


        if (filePath.Length != 0)
        {
            // Create a file to write to
            using (StreamWriter sw = File.CreateText(filePath))
            {
                // General information on line #1, and empty line #2, then write the entire grid on the next line
                sw.WriteLine("Map from https://github.com/Snegil/MapGeneration, saved at " + "UTC:" + System.DateTime.UtcNow);
                sw.WriteLine();

                // Bool to keep track if anything was written to a line
                bool writtenOnLine;

                for (int y = 0; y < grid.GetGridLength().x; y++)
                {
                    writtenOnLine = false;
                    for (int x = 0; x < grid.GetGridLength().y; x++)
                    {
                        // If leveltiles is empty, skip turning writtenOnLine to true, to not create unnecessary whitespace
                        if (grid.GetTileAtPos(y, x) != LevelTiles.Empty) writtenOnLine = true;

                        // Check if leveltiles is empty, if so, write nothing, else write it's info
                        sw.Write(grid.GetTileAtPos(y, x) == LevelTiles.Empty ? "" : y + "," + x + "," + (int)grid.GetTileAtPos(y, x) + "|");
                    }
                    // If the line has been written on, don't start a new one.
                    if (writtenOnLine) sw.WriteLine();
                }
            }
        }
    }
}
