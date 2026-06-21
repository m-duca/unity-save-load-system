using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class DataPersistenceManager : MonoBehaviour
{
    // Singleton
    public static DataPersistenceManager Instance { get; private set; }

    // Inspector
    [Header("Settings")]
    [SerializeField] private string _fileName;
    [SerializeField] private bool _useEncryption;

    // Not serialized
    private GameData _gameData;
    private List<IDataPersistence> _dataPersistenceObjects;
    private FileDataHandler _fileDataHandler;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // persistentDataPath == OS standard directory for saving persistence data
        _fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileName, _useEncryption);
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
        _gameData = _fileDataHandler.Load();

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

        _fileDataHandler.Save(_gameData);
    }

    private void OnApplicationQuit() => SaveGame();
}
