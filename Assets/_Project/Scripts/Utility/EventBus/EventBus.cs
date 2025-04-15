using System;
using System.Collections.Generic;
using UnityEngine;

namespace NJG.Utilities.EventBus
{
    /// <summary>
    /// A global event system.
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> _eventDictionary = new();

        static EventBus()
        {
            Reset();
        }
        
        public static void StartListening<T>(Action<T> listener)
        {
            Type eventType = typeof(T);
            if (_eventDictionary.ContainsKey(eventType))
            {
                _eventDictionary[eventType] = Delegate.Combine(_eventDictionary[eventType], listener);
            }
            else
            {
                _eventDictionary[eventType] = listener;
            }
        }
        
        public static void StopListening<T>(Action<T> listener)
        {
            Type eventType = typeof(T);
            if (_eventDictionary.ContainsKey(eventType))
            {
                _eventDictionary[eventType] = Delegate.Remove(_eventDictionary[eventType], listener);
            }
            else
            {
                Debug.LogWarning("Tried to remove a listener that was not in the collection: " + nameof(listener));
            }
        }
        
        public static void TriggerEvent<T>(T e)
        {
            Type eventType = typeof(T);
            if (_eventDictionary.ContainsKey(eventType))
            {
                if (_eventDictionary.TryGetValue(eventType, out Delegate d))
                {
                    Action<T> action = d as Action<T>;
                    action?.Invoke(e);
                }
                else
                {
                    Debug.LogError("This error should not happen, something went terribly wrong in EventManager.");
                }
            }
            else
            {
                Debug.LogWarning("Tried to trigger an event that did not have any listeners: " + nameof(e));
            }
        }
        
        private static void Reset()
        {
            _eventDictionary.Clear();
        }
    }
}