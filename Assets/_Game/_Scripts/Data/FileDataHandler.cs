using System;
using System.IO;
using Unity.Mathematics;
using UnityEngine;

public class FileDataHandler
{
    private string _dataDirPath = "";
    private string _dataFileName = "";
    private bool _useEncryption = false;
    private readonly string _encryptionWord = "encryption";

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this._dataDirPath = dataDirPath;
        this._dataFileName = dataFileName + ".json";
        this._useEncryption = useEncryption;
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

                // if has been encrypted we need decrypt
                if (_useEncryption)
                    dataToLoad = EncryptOrDecrypt(dataToLoad);
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

            // if needs to encrypt
            if (_useEncryption)
                dataToStore = EncryptOrDecrypt(dataToStore);

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

    private string EncryptOrDecrypt(string data)
    {
        string modifiedData = "";

        for (int i = 0; i < data.Length; i++)
            modifiedData += (char)(data[i] ^ _encryptionWord[i % _encryptionWord.Length]); // XOR Encryption Method

        return modifiedData;
    }
}
