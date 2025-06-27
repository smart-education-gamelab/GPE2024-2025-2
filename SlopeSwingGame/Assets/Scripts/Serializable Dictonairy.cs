using System;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue>
{
    public List<TKey> keys = new List<TKey>();
    public List<TValue> values = new List<TValue>();

    public void Add(TKey key, TValue value)
    {
        keys.Add(key);
        values.Add(value);
    }

    public void Remove(TKey key)
    {
        int index = keys.IndexOf(key);
        if (index >= 0)
        {
            keys.RemoveAt(index);
            values.RemoveAt(index);
        }
    }

    public TValue Get(TKey key)
    {
        int index = keys.IndexOf(key);
        if (index >= 0)
        {
            return values[index];
        }
        throw new KeyNotFoundException($"Key '{key}' not found in dictionary.");
    }

    public bool ContainsKey(TKey key)
    {
        return keys.Contains(key);
    }

    public int Count => keys.Count;

    public void Clear()
    {
        keys.Clear();
        values.Clear();
    }
}