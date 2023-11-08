using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSingleton<T> : MonoBehaviour where T: SimpleSingleton<T>
{
    public static T Instance { get; private set; }
    
    protected virtual void Awake()
    {
        if (Instance == null)
            Instance = this as T;
        else
            Destroy(this.gameObject);
    }
}
