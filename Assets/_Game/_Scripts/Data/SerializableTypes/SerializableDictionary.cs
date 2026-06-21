using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Utility class to manipulate dictionary values for the json game data
/// </summary>
[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> _keys = new ();
    [SerializeField] private List<TValue> _values = new();

    // saving the dictionary to  
    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();

        foreach(KeyValuePair<TKey, TValue> pair in this)
        {
            _keys.Add(pair.Key);
            _values.Add(pair.Value);
        }
    }
    
    // loading the dictionary from
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (_keys.Count != _values.Count) 
        {
            Debug.LogError("Tried to deserialize a SerializableDictionary, but the amount of keys ("
                + _keys.Count + ") does not match the number of values (" + _values.Count 
                + ") which indicates that something went wrong");
        }

        for (int i = 0; i < _keys.Count; i++)
        {
            this.Add(_keys[i], _values[i]);   
        }
    }
}
