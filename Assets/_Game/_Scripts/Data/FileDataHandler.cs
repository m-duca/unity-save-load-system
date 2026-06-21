using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string _dataDirPath = "";
    private string _dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this._dataDirPath = dataDirPath;
        this._dataFileName = dataFileName + ".json";
    }

    public GameData Load()
    {
        string fullPath = GetFullPath();

        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                // Loading the serialized data from the file
                string dataToLoad = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // Deserializing the data from JSON back to C# object
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception error)
            {
                Debug.LogError("Error while trying to load data from the file: " + fullPath + "\n" + error);
            }
        }

        return loadedData;
    }

    public void Save(GameData gameData)
    {
        string fullPath = GetFullPath();

        try
        {
            // Creating the directory if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Serializing the C# game data object into JSON format
            string dataToStore = JsonUtility.ToJson(gameData, true);

            // Writing down the serialized values into the JSON
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception error)
        {
            Debug.LogError("Error while trying to save data into the file: " + fullPath + "\n" + error);
        }
    }

    private string GetFullPath()
    {
        // Using Path.Combine to account different OS
        return Path.Combine(_dataDirPath, _dataFileName);
    }
}
