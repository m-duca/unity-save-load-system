using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class DataPersistenceManager : MonoBehaviour
{
    // Singleton
    public static DataPersistenceManager Instance { get; private set; }

    // Not serialized
    private GameData _gameData;
    private List<IDataPersistence> _dataPersistenceObjects;

    private void Awake()
    {
        if (Instance == null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public void NewGame() => _gameData = new GameData();

    public void LoadGame()
    {
        // TODO: a Load data from a file into the current gameData instance

        // if no data was found, we need to create a new game data
        if (_gameData == null)
        {
            Debug.LogError("No saved Data was found. Initializing with default values...");
            NewGame();
        }

        foreach (IDataPersistence persistenceObject in _dataPersistenceObjects)
            persistenceObject.LoadData(_gameData);
    }

    public void SaveGame()
    {
        foreach (IDataPersistence persistenceObject in _dataPersistenceObjects)
            persistenceObject.SaveData(_gameData);

        // TODO: save the data into a file
    }

    private void OnApplicationQuit() => SaveGame();
}
