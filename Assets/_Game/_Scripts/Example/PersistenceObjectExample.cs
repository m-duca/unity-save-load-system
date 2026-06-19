using UnityEngine;

public class PersistenceObjectExample : MonoBehaviour, IDataPersistence
{   
    [Header("Debug")]
    [SerializeField] private int _example;

    public void LoadData(GameData gameData)
    {
        _example = gameData.Example;
        Debug.Log($"Loaded Example value: {_example}");
    }

    public void SaveData(GameData gameData)
    {
        gameData.Example = _example;
        Debug.Log($"Saved Example value: {_example}");
    }
}
