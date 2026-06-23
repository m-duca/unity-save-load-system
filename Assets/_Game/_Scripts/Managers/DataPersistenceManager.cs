using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    // Singleton
    public static DataPersistenceManager Instance { get; private set; }

    // Inspector
    [Header("Settings")]
    [SerializeField] private string _fileName;
    [SerializeField] private bool _useEncryption;

    [Header("Debug")]
    [SerializeField] private bool _createDataIfNull;

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

        // persistentDataPath == OS standard directory for saving persistence data
        _fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileName, _useEncryption);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
        _dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void OnSceneUnloaded (Scene scene)
    {
        SaveGame();
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
        _gameData = _fileDataHandler.Load();

        // if no data was found, we need to create a new game data
        if (_gameData == null && _createDataIfNull)
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
