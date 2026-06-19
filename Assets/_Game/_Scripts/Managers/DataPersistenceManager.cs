using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    // Singleton
    public static DataPersistenceManager Instance {get; private set;}

    // Not serialized
    private GameData _gameData;

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

    public void NewGame()
    {
        _gameData = new GameData();
    }

    public void LoadGame()
    {
        // TODO: a Load data from a file into the current gameData instance

        // if no data was found, we need to create a new game data
        if (_gameData == null)
        {
            Debug.LogError("No saved Data was found. Initializing with default values...");
            NewGame();
        }

        // TODO: send the data to all scripts that need it
    }

    public void SaveGame()
    {
        // TODO: pass the data to other scripts so they can update it
        
        // TODO: save the data into a file
    }
}
