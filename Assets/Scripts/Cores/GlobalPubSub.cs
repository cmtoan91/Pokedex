using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GlobalPubSub
{
    static Dictionary<Type, List<Delegate>> _eventBag = new Dictionary<Type, List<Delegate>>();

    public static void SubcribeEvent<T>(Action<T> action) where T : GlobalMessage
    {
        Type type = typeof(T);
        if (!_eventBag.TryGetValue(type, out var actionBag))
        {
            actionBag = new List<Delegate>();
        }
        actionBag.Add(action);
        _eventBag[type] = actionBag;
    }

    public static void UnsubcribeEvent<T>(Action<T> action) where T : GlobalMessage
    {
        Type type = typeof(T);
        if (_eventBag.TryGetValue(type, out var actionBag))
        {
            actionBag.Remove(action);
        }
        if (actionBag != null)
            _eventBag[type] = actionBag;
        else
            _eventBag.Remove(type);
    }

    public static void PublishEvent<T>(T message) where T : GlobalMessage
    {
        Type type = typeof(T);
        if (_eventBag.TryGetValue(type, out var actionBag))
        {
            foreach(Delegate del in actionBag)
            {
                switch (del)
                {
                    case Action<T> action:
                        action(message);
                        break;
                    case Action meth:
                        meth();
                        break;
                }
            }
        }
    }
}
public abstract class GlobalMessage
{
}

public class Handler
{
    public Delegate Action { get; set; }
    public Type Type { get; set; }

}
