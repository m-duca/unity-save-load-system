using System;
using System.IO;
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

    public GameData Load()
    {
        string fullPath = GetFullPath();

        if (!File.Exists(fullPath))
            return null;

        // try JSON without decrypt first
        try
        {
            string dataToLoad = File.ReadAllText(fullPath);

            GameData data = JsonUtility.FromJson<GameData>(dataToLoad);

            if (data != null)
            {
                Debug.Log("<color=green>Save loaded successfully without decrypt!</color>");
                return data;
            }
        }
        catch (Exception error)
        {
            Debug.Log("<color=yellow>Fail loading JSON. Trying to decrypt...</color>" + "\n" + error);
        }

        // Try decrypt if pure JSON was failed
        try
        {
            string encryptedData = File.ReadAllText(fullPath);

            string decryptedData = EncryptOrDecrypt(encryptedData);

            GameData data = JsonUtility.FromJson<GameData>(decryptedData);

            Debug.Log("<color=green>Save loaded successfully after decrypt!</color>");
            return data;
        }
        catch (Exception error)
        {
            Debug.LogError("Failed to load gameData" + "\n" + error);
            return null;
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
