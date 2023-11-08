using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SerializedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver, IReadOnlyDictionary<TKey, TValue>
{
    [SerializeField]
    List<KVP> list = new List<KVP>();
    [SerializeField, HideInInspector]
    Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
    public Dictionary<TKey, TValue> Dictionary => dict;

    [SerializeField]
    Dictionary<TKey, int> indexByKey = new Dictionary<TKey, int>();

    public ICollection<TKey> Keys => dict.Keys;

    public ICollection<TValue> Values => dict.Values;

    public int Count => dict.Count;

    public bool IsReadOnly { get; set; }

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => dict.Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => dict.Values;

    [Serializable]
    struct KVP
    {
        public TKey Key;
        public TValue Value;

        public KVP(TKey key, TValue val)
        {
            this.Key = key;
            this.Value = val;
        }
    }

    public TValue this[TKey key]
    {
        get => dict[key];
        set
        {
            dict[key] = value;
            if (indexByKey.ContainsKey(key))
            {
                var index = indexByKey[key];
                list[index] = new KVP(key, value);
            }
            else
            {
                list.Add(new KVP(key, value));
                indexByKey.Add(key, list.Count - 1);
            }
        }
    }

    public void Add(TKey key, TValue value)
    {
        dict.Add(key, value);
        list.Add(new KVP(key, value));
        indexByKey.Add(key, list.Count - 1);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        dict.Clear();
        list.Clear();
        indexByKey.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        TValue value;
        if (dict.TryGetValue(item.Key, out value))
        {
            return EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }
        else
        {
            return false;
        }
    }

    public bool ContainsKey(TKey key)
    {
        return dict.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentException("The array cannot be null.");
        if (arrayIndex < 0)
            throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
        if (array.Length - arrayIndex < dict.Count)
            throw new ArgumentException("The destination array has fewer elements than the collection.");

        foreach (var pair in dict)
        {
            array[arrayIndex] = pair;
            arrayIndex++;
        }
    }


    public void OnAfterDeserialize()
    {
        dict.Clear();
        indexByKey.Clear();

        for (int i = 0; i < list.Count; i++)
        {
            var key = list[i].Key;
            if (key != null && !ContainsKey(key))
            {
                dict.Add(key, list[i].Value);
                indexByKey.Add(key, i);
            }
        }
    }

    public void OnBeforeSerialize()
    {

    }

    public bool Remove(TKey key)
    {
        if (dict.Remove(key))
        {
            var index = indexByKey[key];
            list.RemoveAt(index);
            UpdateIndexes(index);
            indexByKey.Remove(key);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        TValue value;
        if (dict.TryGetValue(item.Key, out value))
        {
            bool valueMatch = EqualityComparer<TValue>.Default.Equals(value, item.Value);
            if (valueMatch)
            {
                return Remove(item.Key);
            }
        }
        return false;
    }

    void UpdateIndexes(int removedIndex)
    {
        for (int i = removedIndex; i < list.Count; i++)
        {
            var key = list[i].Key;
            indexByKey[key]--;
        }
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return dict.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator() => dict.GetEnumerator();
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dict.GetEnumerator();

}
