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
        this._dataFileName = dataFileName;
    }

    public GameData Load()
    {
        
    }

    public void Save(GameData gameData)
    {
        string fullPath = Path.Combine(_dataDirPath, _dataFileName);

        try
        {
            // creating the directory if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // serializing the C# game data object into json format
            string dataToStore = JsonUtility.ToJson(gameData, true);

            // writing down the serialized values into the json
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error while trying to save data into the file: " + fullPath + "\n" + e);            
        }
    }
}
