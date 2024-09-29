using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<Tkey, TValue> : Dictionary<Tkey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<Tkey> _keys = new();
    [SerializeField] private List<TValue> _values = new();

    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();
        foreach (KeyValuePair<Tkey, TValue> kvp in this)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }
    public void OnAfterDeserialize()
    {
        if (_keys.Count != _values.Count)
        {
            Debug.LogError("Couldn't deserialize dictionary, keys (" + _keys.Count + "), values (" + _values.Count + ").");
        }

        for (int i = 0; i < _keys.Count; i++)
        {
            Add(_keys[i], _values[i]);
        }
    }
}
