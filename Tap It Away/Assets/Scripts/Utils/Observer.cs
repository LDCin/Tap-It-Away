using System;
using System.Collections.Generic;
using UnityEngine;

public enum ObserverEvent
{
    CubeBlocked,
    CubeRemoved,
    OnCubeMove,
    TapCube,
    CubeCountChanged,
    HeartCountChanged,
    LevelCompleted,
    LevelFailed,
    LevelLoaded,
    PlayGame,
    OnOpenSettingInGame,
    OnCloseSettingInGame,
    OnBackToMenu,
    OnSettingChanged,
    CoinCountChanged,
    ThemeChanged,
    BoosterActive,
    BoosterCountChanged
}

public static class Observer
{
    private static readonly Dictionary<ObserverEvent, Delegate> eventTable = new();

    public static void Subscribe(ObserverEvent eventType, Action listener)
    {
        AddListener(eventType, listener);
    }

    public static void Subscribe<T>(ObserverEvent eventType, Action<T> listener)
    {
        AddListener(eventType, listener);
    }

    public static void Unsubscribe(ObserverEvent eventType, Action listener)
    {
        RemoveListener(eventType, listener);
    }

    public static void Unsubscribe<T>(ObserverEvent eventType, Action<T> listener)
    {
        RemoveListener(eventType, listener);
    }

    public static void Publish(ObserverEvent eventType)
    {
        if (!eventTable.TryGetValue(eventType, out Delegate callback))
        {
            return;
        }

        if (callback is Action action)
        {
            action.Invoke();
            return;
        }

        Debug.LogWarning($"Observer event {eventType} was published without payload, but listeners expect payload.");
    }

    public static void Publish<T>(ObserverEvent eventType, T payload)
    {
        if (!eventTable.TryGetValue(eventType, out Delegate callback))
        {
            return;
        }

        if (callback is Action<T> action)
        {
            action.Invoke(payload);
            return;
        }

        Debug.LogWarning($"Observer event {eventType} was published with payload {typeof(T).Name}, but listener signature does not match.");
    }

    private static void AddListener(ObserverEvent eventType, Delegate listener)
    {
        if (listener == null)
        {
            return;
        }

        eventTable.TryGetValue(eventType, out Delegate currentDelegate);
        eventTable[eventType] = Delegate.Combine(currentDelegate, listener);
    }

    private static void RemoveListener(ObserverEvent eventType, Delegate listener)
    {
        if (listener == null || !eventTable.TryGetValue(eventType, out Delegate currentDelegate))
        {
            return;
        }

        Delegate newDelegate = Delegate.Remove(currentDelegate, listener);
        if (newDelegate == null)
        {
            eventTable.Remove(eventType);
            return;
        }

        eventTable[eventType] = newDelegate;
    }
}
